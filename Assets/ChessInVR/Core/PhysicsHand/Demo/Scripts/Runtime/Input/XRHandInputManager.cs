using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using PhysicsHand.UI;

namespace PhysicsHand.XR
{
    /// <summary>
    /// A component that lets you define inputs for an XR hand using Unity's new input system.
    /// This component fires Unity events whenever these inputs are fired allowing for powerful behaviours to be configured using Unity editor events.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(RigidbodyHand))]
    public class XRHandInputManager : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("(Optional) A reference to the RigidbodyHandUIPointer associated with this hand. When specified this will prevent distance grabbing while the UI pointer has a valid target to allow the same control to be shared.")]
        public RigidbodyHandUIPointer uiPointer;

#if ENABLE_INPUT_SYSTEM
        [Header("Inputs")]
        [Tooltip("The grab action for the XR hand.")]
        public InputActionProperty grabAction;
        [Tooltip("The release action for the XR hand.")]
        public InputActionProperty releaseAction;
        [Tooltip("The UI select action for the XR hand.")]
        public InputActionProperty uiSelectAction;
        [Tooltip("The distance grab action for the XR hand.")]
        public InputActionProperty distanceGrabAction;
#elif ENABLE_LEGACY_INPUT_MANAGER
        [Header("Inputs")]
        [Tooltip("The KeyCode that fires the grab input for the XR hand.")]
        public KeyCode grabInput = KeyCode.Mouse0;
        [Tooltip("The KeyCode that fires the release input for the XR hand.")]
        public KeyCode releaseInput = KeyCode.Mouse1;
        [Tooltip("The KeyCode that fires the UI select input for the XR hand.")]
        public KeyCode uiSelectInput = KeyCode.Return;
        [Tooltip("The KeyCode that fires the distance grab input for the XR hand.")]
        public KeyCode distanceGrabInput = KeyCode.Mouse2;
#endif

        [Header("Events - Input")]
        [Tooltip("An event that is invoked when the grab input is fired.")]
        public UnityEvent Grabbed;
        [Tooltip("An event that is invoked when the release input is fired.")]
        public UnityEvent Released;
        [Tooltip("An event that is invoked when the UI selected input is pressed.")]
        public UnityEvent UISelectStarted;
        [Tooltip("An event that is invoked when the UI selected input is released.")]
        public UnityEvent UISelectStopped;
        [Tooltip("An event that is invoked when the distance grab input is pressed.")]
        public UnityEvent DistanceGrabStarted;
        [Tooltip("An event that is invoked when the distance grab input is released.")]
        public UnityEvent DistanceGrabStopped;

        /// <summary>Returns a reference to the RigidbodyHand component associated with this XR hand input manager.</summary>
        public RigidbodyHand Hand
        {
            get
            {
                // Ensure the 'Hand' reference is not null.
                if (m_Hand == null)
                    m_Hand = GetComponent<RigidbodyHand>();
                return m_Hand;
            }
        }
        /// <summary>Returns true if the 'UI Select' input is currently pressed, otherwise false.</summary>
        public bool IsUISelectPressed { get; private set; }

        // Hidden backing field(s).
        /// <summary>The hidden backing field for the 'Hand' property.</summary>
        RigidbodyHand m_Hand;

        // Unity callback(s).
        void Reset()
        {
            // Look for UI pointer.
            if (uiPointer == null)
                uiPointer = GetComponent<RigidbodyHandUIPointer>();
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

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
        void Update()
        {
            // Check if grab input should fire.
            if (Input.GetKeyDown(grabInput))
                OnGrabInput();
            // Check if release input should fire.
            if (Input.GetKeyDown(releaseInput))
                OnReleaseInput();
            // Check if UI select started input should fire.
            if (Input.GetKeyDown(uiSelectInput))
                OnUISelectStartedInput();
            // Check if UI select stopped input should fire.
            if (Input.GetKeyUp(uiSelectInput))
                OnUISelectStoppedInput();
            // Check if distance grab started input should fire.
            if (Input.GetKeyDown(distanceGrabInput))
                OnDistanceGrabStartedInput();
            // Check if distance grab stopped input should fire.
            if (Input.GetKeyUp(distanceGrabInput))
                OnDistanceGrabStoppedInput();
        }
#endif

        // Private callback(s)>
        #region Input Callbacks
        /// <summary>
        /// Invoked whenever the grab input is fired.
        /// Note that this event is only fired if the 'Grabber' is enabled.
        /// </summary>
        void OnGrabInput()
        {
            // Only fire the event if the grabber is enabled.
            if (Hand.Grabber.enabled)
            {
                // Invoke the 'Grabbed' Unity event.
                Grabbed?.Invoke();
            }
        }

        /// <summary>
        /// Invoked whenever the release input is fired.
        /// Note that this event is only fired if the 'Grabber' is enabled.
        /// </summary>
        void OnReleaseInput()
        {
            // Only fire the event if the grabber is enabled.
            if (Hand.Grabber.enabled)
            {
                // Invoke the 'Released' Unity event.
                Released?.Invoke();
            }
        }

        /// <summary>
        /// Invoked whenever the UI select input is pressed.
        /// Note that this event is only fired if the 'Grabber' is not currently grabbing something.
        /// </summary>
        void OnUISelectStartedInput()
        {
            // Only fire the event if the grabber is not grabbing anything.
            if (Hand.Grabber.Grabbing == null)
            {
                // Invoke the 'UISelectStarted' Unity event.
                UISelectStarted?.Invoke();

                // UI selecting.
                IsUISelectPressed = true;
            }
        }

        /// <summary>
        /// Invoked whenever the UI select input is pressed.
        /// Note that this event is only fired if 'IsUISelectPressed' is true.
        /// </summary>
        void OnUISelectStoppedInput()
        {
            // Only fire the event if 'IsUISelectPressed'.
            if (IsUISelectPressed)
            {
                // Invoke the 'UISelectStarted' Unity event.
                UISelectStopped?.Invoke();

                // No longer UI selecting.
                IsUISelectPressed = false;
            }
        }

        /// <summary>
        /// Invoked whenever the distance grab input is pressed.
        /// Note that this event is only fired if the 'Grabber' is enabled, not currently grabbing something, and there is no 'UI Pointer' specified or the specified pointer has no target.
        /// </summary>
        void OnDistanceGrabStartedInput()
        {
            // Only fire the event if the grabber is enabled and not grabbing anything, also do not fire if there is a reference 'uiPointer' with a vlaid target.
            if (Hand.Grabber.enabled && Hand.Grabber.Grabbing == null && (uiPointer == null || uiPointer.Target == null))
            {
                // Invoke the 'DistanceGrabStarted' Unity event.
                DistanceGrabStarted?.Invoke();
            }
        }

        /// <summary>
        /// Invoked whenever the distance grab input is released.
        /// Note that this event is only fired if the 'Grabber' is enabled.
        /// </summary>
        void OnDistanceGrabStoppedInput()
        {
            // Only fire the event if the grabber is enabled.
            if (Hand.Grabber.enabled)
            {
                // Invoke the 'DistanceGrabStopped' Unity event.
                DistanceGrabStopped?.Invoke();
            }
        }
        #endregion

        #region New Input System
#if ENABLE_INPUT_SYSTEM
        // Private new input system method(s).
        /// <summary>Subscribes the XR hand input manager to its inputs.</summary>
        void SubscribeToInputs()
        {
            // Subscribe to 'Grab action'.
            if (grabAction != null && grabAction.action != null && grabAction.action.bindings.Count > 0)
            {
                grabAction.action.Enable();
                grabAction.action.started += NewInput_OnGrabInput;
            }

            // Subscribe to 'Release action'.
            if (releaseAction != null && releaseAction.action != null && releaseAction.action.bindings.Count > 0)
            {
                releaseAction.action.Enable();
                releaseAction.action.canceled += NewInput_OnReleaseInput;
            }

            // Subscribe to 'UI select action'.
            if (uiSelectAction != null && uiSelectAction.action != null && uiSelectAction.action.bindings.Count > 0)
            {
                uiSelectAction.action.Enable();
                uiSelectAction.action.started += NewInput_OnUISelectInput;
                uiSelectAction.action.canceled += NewInput_OnUISelectInput;
            }

            // Subscribe to 'Distance Grab action'.
            if (distanceGrabAction != null && distanceGrabAction.action != null && distanceGrabAction.action.bindings.Count > 0)
            {
                distanceGrabAction.action.Enable();
                distanceGrabAction.action.started += NewInput_OnDistanceGrabInput;
                distanceGrabAction.action.canceled += NewInput_OnDistanceGrabInput;
            }
        }

        /// <summary>Unsubscribes the XR hand input manager from its inputs.</summary>
        void UnsubscribeFromInputs()
        {
            // Unsubscribe from 'Grab action'.
            if (grabAction != null && grabAction.action != null && grabAction.action.bindings.Count > 0)
            {
                grabAction.action.started -= NewInput_OnGrabInput;
            }

            // Unsubscribe from 'Release action'.
            if (releaseAction != null && releaseAction.action != null && releaseAction.action.bindings.Count > 0)
            {
                releaseAction.action.canceled -= NewInput_OnReleaseInput;
            }

            // Unsubscribe from 'UI select action'.
            if (uiSelectAction != null && uiSelectAction.action != null && uiSelectAction.action.bindings.Count > 0)
            {
                uiSelectAction.action.started -= NewInput_OnUISelectInput;
                uiSelectAction.action.canceled -= NewInput_OnUISelectInput;
            }

            // Unsubscribe from 'Distance Grab action'.
            if (distanceGrabAction != null && distanceGrabAction.action != null && distanceGrabAction.action.bindings.Count > 0)
            {
                distanceGrabAction.action.started -= NewInput_OnDistanceGrabInput;
                distanceGrabAction.action.canceled -= NewInput_OnDistanceGrabInput;
            }
        }

        // Private new input system callback(s).
        /// <summary>Invoked whenever the 'grab input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnGrabInput(InputAction.CallbackContext pContext) { if (pContext.started) { OnGrabInput(); } }
        /// <summary>Invoked whenever the 'release input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnReleaseInput(InputAction.CallbackContext pContext) { if (pContext.canceled) { OnReleaseInput(); } }
        /// <summary>Invoked whenever the 'UI select input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnUISelectInput(InputAction.CallbackContext pContext)
        {
            if (pContext.started)
            {
                OnUISelectStartedInput();
            }
            else if (pContext.canceled) { OnUISelectStoppedInput(); }
        }
        /// <summary>Invoked whenever the 'distance grab input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnDistanceGrabInput(InputAction.CallbackContext pContext)
        {
            if (pContext.started)
            {
                OnDistanceGrabStartedInput();
            }
            else if (pContext.canceled) { OnDistanceGrabStoppedInput(); }
        }
#endif
        #endregion
    }
}
