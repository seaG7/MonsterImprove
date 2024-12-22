using System;
using UnityEngine;
using UnityEngine.Events;
using PhysicsHand.Helpers;

namespace PhysicsHand.Demo.Joints
{
    /// <summary>
    /// A component that uses HingeJointAngleReader to make a door with a functioning handle.
    /// Doors must start closed.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HingeJointDoorHandle : HingeJointAngleReader
    {
        // HandleStates.
        [Flags]
        [Serializable]
        public enum HandleStates : sbyte
        {
            /// <summary>No handle states open the door.</summary>
            None = 0,
            /// <summary>The handle 'Min' state opens the door.</summary>
            Min = 1,
            /// <summary>The handle 'Mid' state opens the door.</summary>
            Mid = 2,
            /// <summary>The handle 'Max' state opens the door.</summary>
            Max = 4
        }

        // HingeJointDoorHandle.
        #region Edtior Serialized Settings & Events
        [Header("Settings")]
        [Tooltip("A reference to the Rigidbody associated with this door. (If null GetComponent<Rigidbody>() will be used to find one.)")]
        public Rigidbody doorBody;
        [Tooltip("A mask of handle states the door opens at.")]
        public HandleStates opensAt;
        [Tooltip("The door needs to be opened by this percent to open. (If minThreshold == 0.05 the door needs to open 5% to be considered opened.)")]
        public float minThreshold = 0.05f;
        [Tooltip("The door needs to be this percent away from the middle to be considered at the mid-point.")]
        public float midThreshold = 0.05f;
        [Tooltip("The door needs to be opened this much before it is reset.")]
        public float maxThreshold = 0.05f;

        [Header("Settings - Locking")]
        [Tooltip("Should the door handle start locked?")]
        public bool startLocked;
        [Tooltip("The minimum hinge limit for a locked handle. (Use 'Infinity' or '-Infinity' to not modify when locked.)")]
        public float lockedHingeMin = float.PositiveInfinity;
        [Tooltip("The maximumhinge limit for a locked handle. (Use 'Infinity' or '-Infinity' to not modify when locked.)")]
        public float lockedHingeMax = float.PositiveInfinity;

        [Header("Events - State")]
        [Tooltip("An event that is invoked when the door's hinge joint reaches max.")]
        public UnityEvent MaxReached;
        [Tooltip("An event that is invoked when the door's hinge joint reaches mid.")]
        public UnityEvent MidReached;
        [Tooltip("An event that is invoked when the door's hinge joint reaches min.")]
        public UnityEvent MinReached;

        [Header("Events - Door")]
        [Tooltip("An event that is invoked whenever the door is opened.")]
        public UnityEvent DoorOpened;
        [Tooltip("An event that is invoked whenever the door is closed.")]
        public UnityEvent DoorClosed;
        [Tooltip("An event that is invoked whenever the component tries to open a locked door.")]
        public UnityEvent TryOpenLocked;

        [Header("Events - Handle")]
        [Tooltip("An event that is invoked when this door handle is locked. (Not invoked by 'startLocked' state.)")]
        public UnityEvent Locked;
        [Tooltip("An event that is invoked when this door handle is unlocked. (Not invoked by 'startLocked' state.)")]
        public UnityEvent Unlocked;
        #endregion
        #region Public Properties
        /// <summary>Returns true if the door handle is locked, otherwise false. Use the 'SetLocked(bool)' method to change locked state.</summary>
        public bool IsLocked { get; private set; }
        /// <summary>Returns true if the door is opened, otherwise false.</summary>
        public bool IsDoorOpened { get { return !doorBody.isKinematic; } }
        /// <summary>Returns true if the HingeJointDoorHandle is at the minimum position of it's hinge, otherwise false.</summary>
        public bool AtMin { get; private set; } = false;
        /// <summary>Returns true if the HingeJointDoorHandle is at the maximum position of it's hinge, otherwise false.</summary>
        public bool AtMax { get; private set; } = false;
        /// <summary>Returns true if the HingeJointDoorHandle is at the middle position of it's hinge, otherwise false.</summary>
        public bool AtMid { get; private set; } = true;
        #endregion
        #region Private Field(s)
        /// <summary>The cached closed position of the door.</summary>
        Vector3 m_ClosedDoorPosition;
        /// <summary>The cached closed rotation of the door.</summary>
        Quaternion m_ClosedDoorRotation;
        /// <summary>The cached starting Joint.limits.min for the handle.</summary>
        float m_HandleMinLimit;
        /// <summary>The cached starting Joint.limits.max for the handle.</summary>
        float m_HandleMaxLimit;
        #endregion

        // Unity callback(s).
        #region Unity Callback(s)
        protected override void Awake()
        {
            // Invoke the base 'Awake()' method.
            base.Awake();

            // Store closed door position and rotation.
            if (doorBody != null)
            {
                m_ClosedDoorPosition = doorBody.transform.position;
                m_ClosedDoorRotation = doorBody.transform.rotation;
            }
            // Log warning if no 'doorBody' is specified.
            else { Debug.LogWarning("No Rigidbody reference set or found for 'doorBody' for HingeJointDoorHandle component on gameObject '" + gameObject.name + "'!", gameObject); }

            // Cache starting joint limits.
            CacheHingeLimits();

            // Handle the 'start locked' state of the door handle.
            if (startLocked)
            {
                IsLocked = true;

                // Use locked joint limits.
                UseLockedJointLimits();
            }
        }

        protected virtual void FixedUpdate()
        {
            // Update door hinge read value.
            float hingeValue = GetValue();

            // Calculate adjusted thresholds.
            float adjustedMaxThreshold = maxThreshold * 2f;
            float adjustedMinThreshold = minThreshold * 2f;
            float adjustedMidThreshold = midThreshold * 2f;

            // Check if door reached max.
            if (!AtMax && AtMid && hingeValue + adjustedMaxThreshold >= 1) 
            {
                Max();
            }
            
            // Check if door reached min.
            if(!AtMin && AtMid && hingeValue - adjustedMinThreshold <= -1)
            {
                Min();
            }
        
            // Check if door reached mid.
            if (hingeValue <= adjustedMidThreshold && AtMax && !AtMid) 
            {
                Mid();
            }

            if (hingeValue >= -adjustedMidThreshold && AtMin && !AtMid) 
            {
                Mid();
            }
        }
        #endregion

        // Public method(s).
        #region Open & Close Door Method(s)
        /// <summary>A public method that opens the door.</summary>
        public void OpenDoor()
        {
            doorBody.isKinematic = false;

            // Invoke the 'DoorOpened' Unity event.
            DoorOpened?.Invoke();
        }

        /// <summary>A public method that closes the door.</summary>
        public void CloseDoor()
        {
            doorBody.isKinematic = true;
            doorBody.transform.SetPositionAndRotation(m_ClosedDoorPosition, m_ClosedDoorRotation);
            doorBody.position = m_ClosedDoorPosition;
            doorBody.rotation = m_ClosedDoorRotation;

            // Invoke the 'DoorClosed' Unity event.
            DoorClosed?.Invoke();
        }
        #endregion
        #region Door Lock Method(s)
        /// <summary>Changes the locked state of a door.</summary>
        /// <param name="pLocked"></param>
        public void SetLocked(bool pLocked)
        { 
            IsLocked = pLocked; 

            // Handle a locked door if locked.
            if (IsLocked)
            {
                // Use locked joint limits.
                UseLockedJointLimits();

                // Invoke the 'Locked' Unity event.
                Locked?.Invoke();
            }
            // Otherwise handle unlocked door.
            else
            {
                // Use unlocked joint limits.
                UseUnlockedJointLimits();

                // Invoke the 'Unlocked' Unity event.
                Unlocked?.Invoke();
            }
        }
        #endregion
        #region Caching & Joint Limits
        /// <summary>Caches the currently hinge limit values as the 'default'/unlocked hinge limits.</summary>
        public void CacheHingeLimits()
        {
            m_HandleMinLimit = Joint.limits.min;
            m_HandleMaxLimit = Joint.limits.max;
        }

        /// <summary>Makes the door hnadle use the locked joint settings.</summary>
        public void UseLockedJointLimits()
        {
            if (!float.IsInfinity(lockedHingeMin) || !float.IsInfinity(lockedHingeMax))
            {
                Joint.limits = new JointLimits()
                {
                    bounceMinVelocity = Joint.limits.bounceMinVelocity,
                    bounciness = Joint.limits.bounciness,
                    contactDistance = Joint.limits.contactDistance,
                    min = float.IsInfinity(lockedHingeMin) ? Joint.limits.min : lockedHingeMin,
                    max = float.IsInfinity(lockedHingeMax) ? Joint.limits.max : lockedHingeMax,
                };
            }
        }

        /// <summary>Makes the door handle use the unlocked joint settings.</summary>
        public void UseUnlockedJointLimits()
        {
            Joint.limits = new JointLimits()
            {
                bounceMinVelocity = Joint.limits.bounceMinVelocity,
                bounciness = Joint.limits.bounciness,
                contactDistance = Joint.limits.contactDistance,
                min = m_HandleMinLimit,
                max = m_HandleMaxLimit,
            };
        }
        #endregion

        // Private method(s).
        #region Door State Methods
        /// <summary>Intended to be invoked when the door reaches its 'Max' state.</summary>
        void Max()
        {
            // Update 'at' variables.
            AtMid = false;
            AtMax = true;

            // Invoke the 'MaxReached' unity event.
            MaxReached?.Invoke();

            // Try to open the door at this handle state.
            TryOpenDoorAtState(HandleStates.Max);
        }

        /// <summary>Intended to be invoked when the door reaches its 'Mid' state.</summary>
        void Mid()
        {
            // Update 'at' variables.
            AtMin = false;
            AtMax = false;
            AtMid = true;

            // Invoke the 'MidReached' unity event.
            MidReached?.Invoke();

            // Try to open the door at this handle state.
            TryOpenDoorAtState(HandleStates.Mid);
        }

        /// <summary>Intended to be invoked when the door reaches its 'Min' state.</summary>
        void Min()
        {
            // Update 'at' variables.
            AtMin = true;
            AtMid = false;

            // Invoke the 'MinReached' unity event.
            MinReached?.Invoke();

            // Try to open the door at this handle state.
            TryOpenDoorAtState(HandleStates.Min);
        }

        /// <summary>Attempts to open the door at the given handle state. (Restricted based on settings and door state.)</summary>
        /// <param name="pState"></param>
        void TryOpenDoorAtState(HandleStates pState)
        {
            if (!IsDoorOpened && FlagsHelper.IsSet(opensAt, pState))
            {
                // Open the door if it is unlocked.
                if (!IsLocked)
                {
                    OpenDoor();
                }
                // Otherwise invoke the 'TryOpenLocked' Unity event.
                else { TryOpenLocked?.Invoke(); }
            }
        }
        #endregion
    }
}
