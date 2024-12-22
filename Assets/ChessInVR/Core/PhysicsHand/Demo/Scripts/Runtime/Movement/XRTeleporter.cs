using System;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PhysicsHand.Demo.Movement
{
    /// <summary>
    /// A componet that adds teleportation functionality for XR players.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class XRTeleporter : MonoBehaviour
    {
        // TeleportUnityEvent.
        /// <summary>
        /// Arg0: Vector3 - from position in world space.
        /// Arg1: Vector3 - to position in world space.
        /// </summary>
        [Serializable]
        public class TeleportUnityEvent : UnityEvent<Vector3, Vector3> { }

        // XRTeleporter.
        [Header("Settings")]
        [Tooltip("A reference to the Transform whose forward direction is used to point towards teleport location. (If null falls back to 'transform'.)")]
        [SerializeField] Transform m_TeleportPointer;
        [Min(0)]
        [Tooltip("The maximum slope that can be teleported onto.")]
        public float maxSlope = 45f;
        [Min(0)]
        [Tooltip("The maximum distance the teleporter can teleport.")]
        public float maxDistance = 15f;
        [Min(0)]
        [Tooltip("Controls how quickly the teleport line curves.")]
        public float curveStrength = 0.5f;
        [Tooltip("A LayerMask that allows you to specify what layers can be teleported onto.")]
        public LayerMask teleportLayers = Physics.DefaultRaycastLayers;

        [Header("Visualization - Line")]
        [Tooltip("(Optional) A reference to the LineRenderer that this component will use to visualize teleports.")]
        public LineRenderer lineRenderer;
        [Min(0)]
        [Tooltip("The number of segments that make up the teleport visualization line.")]
        public int lineSegments = 60;
        [Tooltip("The color of the line visualization when the target space can be teleported to.")]
        public Color canTeleportColor = Color.green;
        [Tooltip("The color of the line visualization when the target space cannot be teleported to.")]
        public Color cantTeleportColor = Color.red;

        [Header("Visualization - Indicator")]
        [Tooltip("(Optional) A reference to a GameObject that will only be active while selecting a teleport location that will move to the teleport location as a visualization.")]
        public GameObject teleportIndicator;

#if ENABLE_INPUT_SYSTEM
        [Header("Inputs")]
        [Range(0f, 1f)]
        [Tooltip("The minimum axis input value required to trigger a teleport.")]
        public float teleportThreshold = 0.15f;
        [Tooltip("The input action property for teleporting. (On the 'started' event the teleportation location choosing is activated, on 'cancelled' the teleportation is carried out.)")]
        public InputActionProperty teleportInput;
#elif ENABLE_LEGACY_INPUT_MANAGER
        [Header("Inputs")]
        [Tooltip("The KeyCode that triggers a teleport. (On press down the teleportation location choosing is activated, on release the teleportation is carried out.)")]
        public KeyCode teleportInput = KeyCode.Mouse2;
#endif

        [Header("Events")]
        [Tooltip("An event that is invoked whenever a teleport selection is started.")]
        public UnityEvent StartedTeleportSelection;
        [Tooltip("An event that is invoked whenever the teleporter completes a teleport.  A teleport can end either with this even or 'TeleportCanceled' but not both.")]
        public TeleportUnityEvent Teleported;
        [Tooltip("An event that is invoked whenever a teleport is canceled. A teleport can end either with this even or 'Teleported' but not both.")]
        public UnityEvent TeleportCanceled;
        [Tooltip("An event that is invoked whenever a teleport has been stopped either by being completed or canceled.")]
        public UnityEvent TeleportStopped;
        [Tooltip("An event that is invoked each frame where 'IsTeleportValid' flips from false to true while selecting a teleport.")]
        public UnityEvent TeleportValid;
        [Tooltip("An event that is invoked each frame where 'IsTeleportValid' flips from true to false while selecting a teleport.")]
        public UnityEvent TeleportInvalid;

        /// <summary>Returns true if this component is currently selecting a teleport location, otherwise false.</summary>
        public bool IsSelectingTeleport { get; private set; }
        /// <summary>Returns true if the selected teleport location is valid, otherwise false.</summary>
        public bool IsTeleportValid { get; private set; }
        /// <summary>The currently selected teleport position.</summary>
        public Vector3 TeleportPosition { get; private set; }
        /// <summary>The normal from the last teleport selection test hit.</summary>
        public Vector3 TeleportNormal { get; private set; }
        /// <summary>The teleport pointer whose forward direction is used to point towards the teleport location.</summary>
        public Transform TeleportPointer { get { return m_TeleportPointer != null ? m_TeleportPointer : transform; } }

        /// <summary>An array that holds all of the line segments for this XR teleporter.</summary>
        Vector3[] m_LineSegments;
        /// <summary>Tracks whether or not the teleport was valid last frame.</summary>
        bool m_WasTeleportValid;

        // Unity callback(s).
        void Awake()
        {
            // Force any valid 'lineRenderer' into world space mode.
            if (lineRenderer != null && !lineRenderer.useWorldSpace)
            {
                lineRenderer.useWorldSpace = true;
                Debug.LogWarning("Forced LineRenderer into world space mode! XRTeleporter requires the line renderer to be in 'useWorldSpace' mode.", lineRenderer.gameObject);
            }

            // Deactivate 'teleportIndicator'.
            if (teleportIndicator != null)
                teleportIndicator.SetActive(false);
        }

        void Update()
        {
            // Ensure the 'line segments' array is both valid for the current settings and exists.
            if (lineRenderer != null && (m_LineSegments == null || m_LineSegments.Length < lineSegments))
            {
                if (m_LineSegments != null)
                {
                    Array.Resize(ref m_LineSegments, lineSegments);
                }
                else
                { m_LineSegments = new Vector3[lineSegments]; }
            }

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER

            // Check if teleport input was pressed.
            if (Input.GetKeyDown(teleportInput))
            {
                // Only begin teleport selection if not already selecting.
                if (!IsSelectingTeleport)
                    BeginTeleportSelection();
            }

            // Check if teleport input was released.
            if (Input.GetKeyUp(teleportInput))
            {
                // Perform teleport.
                Teleport();
            }
#endif

            // Handle teleport selection.
            if (IsSelectingTeleport)
            {
                // Calculate line and test teleport locations.
                int i;
                IsTeleportValid = false;
                for (i = 0; i < m_LineSegments.Length; ++i)
                {
                    float step = i / (m_LineSegments.Length - 1);
                    m_LineSegments[i] = TeleportPointer.position + (maxDistance * step * TeleportPointer.forward);
                    m_LineSegments[i].y += curveStrength * (step - Mathf.Pow(9.81f * 0.5f * step, 2));
                   
                    // If we aren't at the first line segment test from the previous line segment to the current segment.
                    if (i > 0)
                    {
                        // Perform linecast to test for teleport hit spots.
                        if (Physics.Linecast(m_LineSegments[i - 1], m_LineSegments[i], out RaycastHit hitInfo, teleportLayers, QueryTriggerInteraction.Ignore))
                        {
                            // Check slope.
                            if (Vector3.Angle(hitInfo.normal, Vector3.up) <= maxSlope)
                            {
                                TeleportPosition = hitInfo.point;
                                TeleportNormal = hitInfo.normal;
                                IsTeleportValid = true;
                            }

                            // Stop testing after a collision.
                            break;
                        }
                    }
                }

                // Invoke 'teleport valid' swap events.
                if (IsTeleportValid)
                {
                    if (!m_WasTeleportValid)
                    {
                        // Teleport became valid! Invoke 'TeleportValid' Unity event.
                        TeleportValid?.Invoke();
                    }
                }
                else
                {
                    if (m_WasTeleportValid)
                    {
                        // Teleport became invalid! Invoke 'TeleportInvalid' Unity event.
                        TeleportInvalid?.Invoke();
                    }
                }

                // Recompute the line visualization.
                if (lineRenderer != null)
                {
                    Color lineColor = IsTeleportValid ? canTeleportColor : cantTeleportColor;
                    lineRenderer.startColor = lineColor;
                    lineRenderer.endColor = lineColor;
                    lineRenderer.positionCount = i; // 'i' is used so only a partial line is drawn when a hit occurs.
                    lineRenderer.SetPositions(m_LineSegments);
                }

                // Move and orientate the indicator.
                if (teleportIndicator != null)
                {
                    teleportIndicator.transform.position = TeleportPosition;
                    teleportIndicator.transform.up = TeleportNormal;
                }

                // Update 'was teleport valid'.
                m_WasTeleportValid = IsTeleportValid;
            }
            // No teleport selection occuring.
            else
            {
                // Disable line renderer.
                if (lineRenderer != null)
                    lineRenderer.positionCount = 0;
            }
        }

        void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            // Subscribe to relevant inputs.
            SubscribeToInputs();
#endif
        }

        void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            // Unsubscribe from relevant inputs.
            UnsubscribeFromInputs();
#endif
        }

        // Shared Public Method(s).
        #region Shared Public Methods
        /// <summary>Begins teleport selection.</summary>
        public void BeginTeleportSelection()
        {
            // Activate 'teleportIndicator'.
            if (teleportIndicator != null)
                teleportIndicator.SetActive(true);

            // Selecting teleport...
            IsSelectingTeleport = true;
            IsTeleportValid = false;

            // Invoke the 'StartedTeleportSelection' Unity event.
            StartedTeleportSelection?.Invoke();
        }

        /// <summary>Carries out the current teleport selection.</summary>
        public void Teleport()
        {
            // Only carry out a teleport if we are selecting a teleport currently.
            if (IsSelectingTeleport)
            {
                // Invoke the 'OnStoppedTeleporting' callback.
                OnStoppedTeleporting();

                // Only teleport if valid.
                if (IsTeleportValid)
                {
                    // Teleport no longer valid.
                    IsTeleportValid = false;

                    // Cache 'from pos'.
                    Vector3 fromPos = transform.position;

                    // Teleport the transform.
                    transform.position = TeleportPosition;

                    // Invoke the 'Teleported' Unity event.
                    Teleported?.Invoke(fromPos, transform.position);
                }
            }
        }

        /// <summary>Cancels a teleport selection.</summary>
        public void CancelTeleport()
        {
            // Invoke the 'OnStoppedTeleporting' callback.
            OnStoppedTeleporting();

            // Invoke the 'TeleportCanceled' Unity event.
            TeleportCanceled?.Invoke();
        }
        #endregion

        // Shared private callback(s).
        #region Shared Private Callback(s)
        void OnStoppedTeleporting()
        {
            // No longer selecting teleport.
            IsSelectingTeleport = false;

            // Disable line renderer.
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;

            // Deactivate 'teleportIndicator'.
            if (teleportIndicator != null)
                teleportIndicator.SetActive(false);

            // Invoke the 'TeleportStopped' Unity event.
            TeleportStopped?.Invoke();
        }
        #endregion

        // New Input System.
        #region New Input System
#if ENABLE_INPUT_SYSTEM
        // Private new input system method(s).
        /// <summary>Subscribes the teleporter to its inputs.</summary>
        void SubscribeToInputs()
        {
            // Subscribe to 'teleport input'.
            if (teleportInput != null && teleportInput.action != null && teleportInput.action.bindings.Count > 0)
            {
                teleportInput.action.Enable();
                teleportInput.action.started += NewInput_OnTeleportInput;
                teleportInput.action.performed += NewInput_OnTeleportInput;
                teleportInput.action.canceled += NewInput_OnTeleportInput;
            }
        }

        /// <summary>Unsubscribes the teleporter from its inputs.</summary>
        void UnsubscribeFromInputs()
        {
            // Unsubscribe from 'teleport input'.
            if (teleportInput != null && teleportInput.action != null && teleportInput.action.bindings.Count > 0)
            {
                teleportInput.action.started -= NewInput_OnTeleportInput;
                teleportInput.action.performed -= NewInput_OnTeleportInput;
                teleportInput.action.canceled -= NewInput_OnTeleportInput;
            }
        }

        // Private new input system callback(s).
        /// <summary>Invoked whenever the 'rotate left input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnTeleportInput(InputAction.CallbackContext pContext)
        {
            // If the input has started attempt to begin a teleport.
            if (pContext.started || pContext.performed)
            {
                // Only consider starting teleport selection if not already in one.
                if (!IsSelectingTeleport)
                {
                    // Read thumbstick axis value.
                    Vector2 value = pContext.ReadValue<Vector2>();
                    // If the thumbstick is moved at least teleportThreshold units in either direction on the y axis begin a teleport.
                    if (value.y > teleportThreshold || value.y < -teleportThreshold)
                    {
                        BeginTeleportSelection();
                    }
                }
            }
            else if (pContext.canceled)
            {
                // Complete the teleport.
                Teleport();
            }
        }
#endif
        #endregion
    }
}
