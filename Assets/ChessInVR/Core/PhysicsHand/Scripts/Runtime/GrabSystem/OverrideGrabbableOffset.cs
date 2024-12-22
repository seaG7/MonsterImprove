using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that overrides the 'move object' offset settings based on hand side.
    /// Useful for use with non-symmetrical grabbables.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class OverrideGrabbableOffset : MonoBehaviour
    {
        #region Editor Serialized Fields
        [Header("Settings")]
        [Tooltip("Should the left hand grab offset being overridden?")]
        public bool overrideLeftOffset = true;
        [Tooltip("The left hand offset in local space of the left hand that is grabbing.")]
        public Vector3 leftOffset = Vector3.zero;
        [Tooltip("The euler angle offset from the left hand that is grabbing.")]
        public Vector3 leftEulerOffset = Vector3.zero;
        [Tooltip("Should the right hand grab offset being overridden?")]
        public bool overrideRightOffset = true;
        [Tooltip("The right hand offset in local space of the right hand that is grabbing.")]
        public Vector3 rightOffset = Vector3.zero;
        [Tooltip("The euler angle offset from the right hand that is grabbing.")]
        public Vector3 rightEulerOffset = Vector3.zero;
        #endregion
        #region Public Properties
        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Awake()
        {
            // Find relevant GrabbableObject.
            Grabbable = GetComponent<GrabbableObject>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            if (Grabbable != null)
            {
                Grabbable.OverrideGrabOffsetDelegate += OnOverrideGrabOffset;
                Grabbable.OverrideGrabEulerOffsetDelegate += OnOverrideGrabEulerOffset;
            }
        }

        void OnDisable()
        {
            // Subscribe to relevant event(s).
            if (Grabbable != null)
            {
                Grabbable.OverrideGrabOffsetDelegate -= OnOverrideGrabOffset;
                Grabbable.OverrideGrabEulerOffsetDelegate -= OnOverrideGrabEulerOffset;
            }
        }
        #endregion

        // Private callback(s).
        #region Private Grabbable Callbacks
        /// <summary>Invoked whenever 'Grabbable.GetGrabOffset(Grabber)' is invoked.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        /// <param name="pOffset">A reference to the Vector3 that represents the local space (relative to the grabber) for the grab offset.</param>
        void OnOverrideGrabOffset(Grabber pGrabber, GrabbableObject pGrabbable, ref Vector3 pOffset)
        {
            // Look for 'RigidbodyHand'.
            RigidbodyHand hand = pGrabber.GetComponent<RigidbodyHand>();
            if (hand != null)
            {
                // Override grab offset for left hand.
                if (hand.leftHand)
                {
                    // Only offset if set to.
                    if (overrideLeftOffset)
                        pOffset = leftOffset;
                }
                // Override grab offset for right hand.
                else
                {
                    // Only offset if set to.
                    if (overrideRightOffset)
                        pOffset = rightOffset;
                }
            }
        }

        /// <summary>Invoked whenever 'Grabbable.GetGrabEulerOffset(Grabber)' is invoked.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        /// <param name="pEulerOffset">A reference to the Vector3 that represents the local space (relative to the grabber) for the grab euler offset.</param>
        void OnOverrideGrabEulerOffset(Grabber pGrabber, GrabbableObject pGrabbable, ref Vector3 pEulerOffset)
        {
            // Look for 'RigidbodyHand'.
            RigidbodyHand hand = pGrabber.GetComponent<RigidbodyHand>();
            if (hand != null)
            {
                // Override grab euler offset for left hand.
                if (hand.leftHand)
                {
                    // Only offset if set to.
                    if (overrideLeftOffset)
                        pEulerOffset = leftEulerOffset;
                }
                // Override grab euler offset for right hand.
                else
                {
                    // Only offset if set to.
                    if (overrideRightOffset)
                        pEulerOffset = rightEulerOffset;
                }
            }
        }
        #endregion
    }
}
