using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject that automatically restores the 'Force Follow' components enabled state after the object is grabbed.
    /// USAGE: When you have a grabbable set to 'Maintain Offset' mode by default the physics hand will stop 'force following' the controllers to maintain its relative position, this reenables that force following for situations where you want it.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class EnableForceFollowOnGrab : MonoBehaviour
    {
        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find 'Grabbable' reference.
            Grabbable = GetComponent<GrabbableObject>();     
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            Grabbable.Grabbed.AddListener(OnGrabbed);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            Grabbable.Grabbed.RemoveListener(OnGrabbed);
        }

        // Private callback(s).
        void OnGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Look for RigidbodyHand on grabber.
            RigidbodyHand rigidbodyHand = pGrabber.GetComponent<RigidbodyHand>();
            if (rigidbodyHand != null)
            {
                // Enable force following for the physics hand.
                rigidbodyHand.ForceFollower.enabled = true;
            }
        }
    }
}
