using System;
using UnityEngine;
using UnityEngine.Events;
using GrabSystem;
using TimeSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a DistanceGrabber that implements ray casting behaviour to determine which distance grabbers to select.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(DistanceGrabber))]
    public class DistanceGrabController : MonoBehaviour
    {
        // PointUnityEvent.
        /// <summary>
        /// Arg0: Vector3 - the point in space associated with this event.
        /// </summary>
        [Serializable]
        public class PointUnityEvent : UnityEvent<Vector3> {}

        // DistanceGrabController.
        #region Editor Serialized Settings
        [Header("Settings")]
        [Tooltip("A reference to a Transform whose forward direction (blue axis) points 'forward'. (If null 'transform' will be fallbacked to.)")]
        [SerializeField] Transform m_PointerTransform;
        [Min(0f)]
        [Tooltip("The radius of the grab pointer sphere cast. If 0 a raycast is used.")]
        public float pointerRadius = 0f;
        [Min(0f)]
        [Tooltip("The maximum distance a distance grabbable can be from 'PointerTransform' before it can no longer be grabbed.")]
        public float maxGrabDistance = 10f;
        [Min(0f)]
        [Tooltip("The flick rotation speed required to 'flick pull' the distance grabbable that is highlighted. If <= 0 the pull will be instantaneous.")]
        public float flickThreshold = 6f;
        [Min(0f)]
        [Tooltip("The number of seconds the last highlighted item will remain highlighted for.")]
        public float unhighlightDelay = 1.5f;

        [Header("Events - Selection")]
        [Tooltip("An event that is invoked when this component starts selecting possible distance grabbables.")]
        public UnityEvent StartedSelecting;
        [Tooltip("An event that is invoked when this component stops selecting possible distance grabbables.")]
        public UnityEvent StoppedSelecting;
        [Tooltip("An event that is invoked whenever a selection cast hits any point in space.")]
        public PointUnityEvent SelectionCastHit;
        [Tooltip("An event that is invoked whenever a selection cast misses and hits nothing.")]
        public UnityEvent SelectionCastMissed;
        #endregion
        #region Public Properties
        /// <summary>A reference to the DistanceGrabbable being 'highlighted' by this distance grabber.</summary>
        public DistanceGrabbable Highlighting
        {
            get { return m_Highlighting; }
            set
            {
                // If already highlighting something then unhighlight it.
                if (m_Highlighting != null)
                {
                    m_Highlighting.Unhighlight(DistanceGrabber.Grabber);
                }

                // Update 'highlighting'.
                m_Highlighting = value;

                // If highlighting a valid distance grabbable highlight it.
                if (m_Highlighting != null)
                {
                    LastHighlightTime = TimeManager.GetTime();
                    m_Highlighting.Highlight(DistanceGrabber.Grabber);
                }
            }
        }
        /// <summary>Returns true if this component is currently trying to select a distance grabbable, otherwise false.</summary>
        public bool Selecting { get; private set; }
        /// <summary>The last TimeManager.GetTime() this component highlighted something</summary>
        public float LastHighlightTime { get; private set; }
        /// <summary>A reference to the DistanceGrabber associated with this component.</summary>
        public DistanceGrabber DistanceGrabber { get; private set; }
        /// <summary>Returns the pointer Transform used to test distance grabs for this component.</summary>
        public Transform PointerTransform { get { return m_PointerTransform != null ? m_PointerTransform : transform; } }
        #endregion
        #region Hidden Backing Field(s)
        /// <summary>The hidden backing field for the 'Highlighting' property.</summary>
        DistanceGrabbable m_Highlighting;
        /// <summary>The rotation of the pointer last frame.</summary>
        Quaternion m_PointerRotationLastFrame;
        #endregion

        // Unity Callback(s).
        #region Unity Callback(s)
        void Awake()
        {
            // Find 'DistanceGrabber' reference.
            DistanceGrabber = GetComponent<DistanceGrabber>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            DistanceGrabber.StartedPull.AddListener(OnStartedPull);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            DistanceGrabber.StartedPull.RemoveListener(OnStartedPull);

            // If highlighting something ensure we are not pulling it and stop highlighting it.
            if (Highlighting != null)
            {
                // Only stop pulling if 'Highlighting' is equal to the DistanceGrabbable being pulled.
                if (DistanceGrabber.Pulling == Highlighting)
                    DistanceGrabber.StopPull();
                Highlighting = null;
            }

        }

        void Update()
        {
            // If grabber is already grabbin stop selecting and highlighting.
            if (DistanceGrabber.Grabber.Grabbing != null)
            {
                if (Selecting)
                    StopSelecting();
                if (Highlighting != null)
                    Highlighting = null;
            }
            // Otherwise perform normal behaviour.
            else
            {
                // If selecting update selection...
                if (Selecting)
                {
                    UpdateSelection();
                }
                // Otherwise deselect anything we're highlighting.
                else if (Highlighting != null) { Highlighting = null; }

                // If highlighting check for pull...
                if (Highlighting != null)
                {
                    // Try flick pull if flickThreshold is > 0.
                    if (flickThreshold > 0)
                    {
                        // Check if flick was activated.
                        Quaternion deltaRot = PointerTransform.rotation * Quaternion.Inverse(m_PointerRotationLastFrame);
                        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
                        angle *= Mathf.Deg2Rad;
                        float flickSpeed = ((1f / Time.deltaTime) * angle * axis).magnitude;
                        if (flickSpeed > flickThreshold)
                        {
                            // Start pulling and stop highlighting.
                            DistanceGrabber.StartPull(Highlighting);
                            Highlighting = null;
                        }
                    }
                    // Otherwise do instant pull and stop highlightings.
                    else
                    {
                        DistanceGrabber.StartPull(Highlighting);
                        Highlighting = null;
                    }
                }
            }

            // Update 'pointer rotation last frame'.
            m_PointerRotationLastFrame = PointerTransform.rotation;
        }
        #endregion

        // Public method(s).
        #region Selecting Method(s)
        /// <summary>Tells this component to start selecting distance grabbables.</summary>
        public void StartSelecting()
        {
            // Selecting distance grabbables.
            Selecting = true;

            // Invoke the 'started selecting' Unity event.
            StartedSelecting?.Invoke();
        }

        /// <summary>Tells this component to stop selecting distance grabbables.</summary>
        public void StopSelecting()
        {
            // No longer selecting.
            Selecting = false;

            // Invoke the 'stopped selecting' Unity event.
            StoppedSelecting?.Invoke();
        }
        #endregion

        // Private  method(s).
        #region Private Selection Method(s).
        /// <summary>Updates the selection for this distance grab controller.</summary>
        void UpdateSelection()
        {
            // Determine layer mask for grab.
            LayerMask layerMask = Physics.DefaultRaycastLayers;
            if (DistanceGrabber.Grabber is RayGrabber rayGrabber)
                layerMask = ~rayGrabber.ignoreGrabLayers;

            // Check for hit.
            RaycastHit hitInfo;
            bool castResult = pointerRadius == 0 ? Physics.Raycast(PointerTransform.position, PointerTransform.forward, out hitInfo, maxGrabDistance, layerMask, QueryTriggerInteraction.Ignore) : Physics.SphereCast(PointerTransform.position, pointerRadius, PointerTransform.forward, out hitInfo, maxGrabDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (castResult)
            {
                // Invoke the 'SelectionCastHit' Unity event.
                SelectionCastHit?.Invoke(hitInfo.point);

                // Determine if grabbable was hit.
                GrabbableObject grabbableHit = hitInfo.collider.GetComponent<GrabbableObject>();
                if (grabbableHit == null)
                {
                    GrabbableChildObject grabbableChildHit = hitInfo.collider.GetComponent<GrabbableChildObject>();
                    if (grabbableChildHit != null)
                        grabbableHit = grabbableChildHit.grabbable;
                }

                // If a grabbable was hit check for a distance grabbable component.
                if (grabbableHit != null)
                {
                    // Ensure there is a DistanceGrabbable component on the grabbable and that it is enabled.
                    DistanceGrabbable distanceGrabbable = grabbableHit.GetComponent<DistanceGrabbable>();
                    if (distanceGrabbable != null && distanceGrabbable.enabled)
                    {
                        // Highlight the distance grabbable if its underlying grabbable is grabbable by 'DistanceGrabber.Grabber'.
                        if (DistanceGrabber.CanDistanceGrab(distanceGrabbable))
                        {
                            // Highlight the distance grabbable.
                            Highlighting = distanceGrabbable;
                        }
                    }
                }
            }
            else
            {
                // Missed, deselect anything being highlighted if atleast unhighlightDelay seconds have passed..
                if (Highlighting != null)
                {
                    // Unhighlight if enough time has elapsed.
                    if (unhighlightDelay == 0 || Time.time - LastHighlightTime >= unhighlightDelay)
                        Highlighting = null;
                }

                // Invoke the 'SelectionCastMissed' Unity event.
                SelectionCastMissed?.Invoke();
            }
        }
        #endregion

        // Private callback(s)
        #region Distance Grabber Callback(s)
        void OnStartedPull(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Stop selecting when a pull starts.
            StopSelecting();
        }
        #endregion
    }
}
