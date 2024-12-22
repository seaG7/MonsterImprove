using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.Demo.Joints
{
    /// <summary>
    /// A component that allows the angle of a hinge joint to be easily read.
    /// 
    /// NOTE: The 'joint limits' will be cached once at Awake() and these limits will be used to calculate angle. Use 'CacheJointLimits()' to use new joint limits after changing. This is designed like this because things like door handles may want to change joint limits without changing the 'angle' reading.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(HingeJoint))]
    public class HingeJointAngleReader : MonoBehaviour
	{
		[Header("Settings")]
		[Tooltip("Should the value of the hinge joint angle reader be inverted?")]
        public bool invertValue = false;
        [Tooltip("The absolute 'allowable play' in the hinge joint before it will return a non-zero value. If play range is 0.05f for example then the hinge joint needs to move atleast 5% to attain a non-zero value.")]
        public float playRange = 0.05f; 
		
        /// <summary>A reference to the HingeJoint this component is reading the angles for.</summary>
        public HingeJoint Joint { get; private set; }
		///<summary>The value returned by the last call to HingeJointAngleReader.GetValue().</summary>
		public float LastValue { get; protected set; }
        /// <summary>The cached joint limits this joint angle reader uses to determine angle. (NOTE: After changing Joint.limits you will need to use the 'CacheJointLimits()' method to consider the new limits when determing angle.)</summary>
        public JointLimits CachedLimits { get; protected set; }
		
        protected Quaternion m_StartRotation;

		// Unity callback(s).
        protected virtual void Awake()
		{
            // Find HingeJoint component.
            Joint = GetComponent<HingeJoint>(); 
            
            // Store starting rotation.
		    m_StartRotation = transform.localRotation;

            // Cache joint limits.
            CacheJointLimits();
        }

        // Public method(s).
        /// <summary>Caches the current joint limits to use them for angle reading.</summary>
        public void CacheJointLimits()
        {
            CachedLimits = Joint.limits;
        }

        /// <summary>Returns a -1f to 1f value representing the hinges angle from min-max.</summary>
		/// <returns>a -1f to 1f value representing the hinges angle from min-max.</returns>
        public float GetValue()
		{
            LastValue = Joint.angle / (CachedLimits.max - CachedLimits.min) * 2;
			if (invertValue)
				LastValue = -LastValue;

            if (Mathf.Abs(LastValue) < playRange)
                LastValue = 0f;
			
			LastValue = Mathf.Clamp(LastValue, -1f, 1f);
            return LastValue;
        }
    }
}
