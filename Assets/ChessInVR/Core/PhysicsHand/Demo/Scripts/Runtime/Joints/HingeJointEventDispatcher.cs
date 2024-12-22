using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.Demo.Joints
{
    /// <summary>
    /// A component that derives from HingeJointAngleReader that dispatches events when a hinge joint reaches certain thresholds.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HingeJointEventDispatcher : HingeJointAngleReader
    {
        [Header("Settings")]   
        [Tooltip("The joint needs to be opened by this percent to open. (If minThreshold == 0.05 the door needs to open 5% to be considered opened.)")]
        public float minThreshold = 0.05f;
        [Tooltip("The joint needs to be this percent away from the middle to be considered at the mid-point.")]
        public float midThreshold = 0.05f;
        [Tooltip("The joint needs to be opened this much before it is reset.")]
        public float maxThreshold = 0.05f;

        [Header("Events")]
        [Tooltip("An event that is invoked when the hinge joint reaches max.")]
        public UnityEvent MaxReached;
        [Tooltip("An event that is invoked when the hinge joint reaches mid.")]
        public UnityEvent MidReached;
        [Tooltip("An event that is invoked when the hinge joint reaches min.")]
        public UnityEvent MinReached;

        /// <summary>Returns true if the Door is at the minimum position of it's hinge, otherwise false.</summary>
        public bool AtMin { get; private set; } = false;
        /// <summary>Returns true if the Door is at the maximum position of it's hinge, otherwise false.</summary>
        public bool AtMax { get; private set; } = false;
        /// <summary>Returns true if the Door is at the middle position of it's hinge, otherwise false.</summary>
        public bool AtMid { get; private set; } = true;
		
		Vector3 m_AwakePosition;
        Quaternion m_AwakeRotation;

        // Unity callback(s).
        protected override void Awake()
        {
            // Invoke the base 'Awake()' method.
            base.Awake();

            // Store awake position and rotation.
            m_AwakePosition = transform.position;
            m_AwakeRotation = transform.rotation;
        }

        void FixedUpdate()
        {
            // Update hinged joint door angle reading.
            float hingeValue = GetValue();

            // Calculate adjusted thresholds.
            float adjustedMaxThreshold = maxThreshold * 2f;
            float adjustedMinThreshold = minThreshold * 2f;
            float adjustedMidThreshold = midThreshold * 2f;

            // Check if joint reached max.
            if (!AtMax && AtMid && hingeValue + adjustedMaxThreshold >= 1) 
            {
                Max();
            }
            
            // Check if joint reached min.
            if(!AtMin && AtMid && hingeValue - adjustedMinThreshold <= -1)
            {
                Min();
            }
        
            // Check if joint reached mid.
            if (hingeValue <= adjustedMidThreshold && AtMax && !AtMid) 
            {
                Mid();
            }

            if (hingeValue >= -adjustedMidThreshold && AtMin && !AtMid) 
            {
                Mid();
            }
        }


        // Private method(s).
        void Max()
        {
            // Update 'at' variables.
            AtMid = false;
            AtMax = true;

            // Invoke the 'MaxReached' unity event.
            MaxReached?.Invoke();
        }

        void Mid()
        {
            // Update 'at' variables.
            AtMin = false;
            AtMax = false;
            AtMid = true;

            // Invoke the 'MidReached' unity event.
            MidReached?.Invoke();
        }

        void Min() 
        {
            // Update 'at' variables.
            AtMin = true;
            AtMid = false;

            // Invoke the 'MinReached' unity event.
            MinReached?.Invoke();
        }

        // Public method(s).
        /// <summary>A public method that resets a hinge joint's transform to what it was at Awake().</summary>
        public void ResetToAwake()
        {
            transform.SetPositionAndRotation(m_AwakePosition, m_AwakeRotation);
        }
    }
}
