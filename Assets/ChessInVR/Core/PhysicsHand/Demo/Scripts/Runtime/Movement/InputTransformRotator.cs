using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace PhysicsHand.Demo.Movement
{
    /// <summary>
    /// A component that implements kinematic rotation using either the new or old Unity input system.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class InputTransformRotator : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Rotate continunously?")]
        public bool continuousRotate;
        [Tooltip("Only applies to non-continuous rotation. The number of degrees to rotate per rotation step.")]
        public float rotateDegrees = 35f;
        [Tooltip("Only applies to continuous rotation. The rotation speed for continuous rotation in degrees per second. (degrees/sec)")]
        public float rotateSpeed = 270f;
        [Tooltip("The local space axis to rotate around.")]
        public Vector3 rotateAxis = Vector3.up;

#if ENABLE_INPUT_SYSTEM
        [Header("Inputs")]
        [Range(0f, 1f)]
        [Tooltip("The minimum axis input value required to trigger a rotate.")]
        public float rotateThreshold = 0.15f;
        [Tooltip("The input action property for rotating.")]
        public InputActionProperty rotateInput;
#elif ENABLE_LEGACY_INPUT_MANAGER
        [Header("Inputs")]
        [Tooltip("The KeyCode that rotates the Transform left.")]
        public KeyCode rotateLeftInput = KeyCode.LeftArrow;
        [Tooltip("The KeyCode that rotates the Transform right.")]
        public KeyCode rotateRightInput = KeyCode.RightArrow;
#endif

        // Unity callback(s).
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
            // Check if rotate right input should be fired.
            if (Input.GetKeyDown(rotateRightInput) || (continuousRotate && Input.GetKey(rotateRightInput)))
                OnRotateRightInput();
            // Check if rotate left input should be fired.
            if (Input.GetKeyDown(rotateLeftInput) || (continuousRotate && Input.GetKey(rotateLeftInput)))
                OnRotateLeftInput();
        }
#endif

        // Public method(s).
        /// <summary>Rotates the relevant Transform using the given (positive or negative) step size.</summary>
        /// <param name="pStep"></param>
        public void RotateStep(float pStep)
        {
            transform.Rotate(rotateAxis, pStep * rotateDegrees);
        }

        // Both Input Systems.
        #region Shared Input System Input Callback(s)
        /// <summary>Invoked whenever the 'rotate left' input has been fired by either input system.</summary>
        void OnRotateLeftInput()
        {
            // Handle non-continuous left rotation input.
            if (!continuousRotate)
            {
                RotateStep(-1f);
            }
            // Otherwise handle non-continuous left rotation input.
            else { transform.Rotate(rotateAxis, (-rotateSpeed) * Time.deltaTime); }
        }

        /// <summary>Invoked whenever the 'rotate right' input has been fired by either input system.</summary>
        void OnRotateRightInput()
        {
            // Handle non-continuous right rotation input.
            if (!continuousRotate)
            {
                RotateStep(1f);
            }
            // Otherwise handle non-continuous right rotation input.
            else { transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime); }
        }
        #endregion

        // New Input System.
        #region New Input System
#if ENABLE_INPUT_SYSTEM
        // Private new input system method(s).
        /// <summary>Subscribes the rotator to its inputs.</summary>
        void SubscribeToInputs()
        {
            // Subscribe to 'rotate input'.
            if (rotateInput != null && rotateInput.action != null && rotateInput.action.bindings.Count > 0)
            {
                rotateInput.action.Enable();
                rotateInput.action.started += NewInput_OnRotateInput;
                rotateInput.action.performed += NewInput_OnRotateInput;
                rotateInput.action.canceled += NewInput_OnRotateInput;
            }
        }

        /// <summary>Unsubscribes the rotator from its inputs.</summary>
        void UnsubscribeFromInputs()
        {
            // Unsubscribe from 'rotate input'.
            if (rotateInput != null && rotateInput.action != null && rotateInput.action.bindings.Count > 0)
            {
                rotateInput.action.started -= NewInput_OnRotateInput;
                rotateInput.action.performed -= NewInput_OnRotateInput;
                rotateInput.action.canceled -= NewInput_OnRotateInput;
            }
        }

        // Private new input system callback(s).
        /// <summary>Invoked whenever the 'rotate left input' is fired by the new input system.</summary>
        /// <param name="pContext"></param>
        void NewInput_OnRotateInput(InputAction.CallbackContext pContext) 
        { 
            if (pContext.started || (continuousRotate && pContext.performed))
            {
                // Read thumbstick axis value.
                Vector2 value = pContext.ReadValue<Vector2>();

                // If the thumbstick is moved right rotate right.
                if (value.x > rotateThreshold)
                {
                    OnRotateRightInput();
                }
                // Otherwise rotate left if thumbstick is moved left.
                else if (value.x < -rotateThreshold)
                {
                    OnRotateLeftInput();
                }
            }
        }
#endif
        #endregion
    }
}
