using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject component that automatically finds any 'RigidbodyHands' holding the grabbable object and toggles, or sets on or off their 'ForceFollow' components enabled state.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class GrabForceFollowToggler : MonoBehaviour
    {
        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find 'Grabbable' reference.
            Grabbable = GetComponent<GrabbableObject>();     
        }

        // Public method(s).
        /// <summary>A public method that toggles the enabled state of all 'ForceFollower' components of any RigidbodyHands that are holding the relevant grabbable object.</summary>
        public void ToggleForceFollowers()
        {
            // Look for RigidbodyHand on grabber.
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null)
                    {
                        // Enable force following for the physics hand.
                        rigidbodyHand.ForceFollower.enabled = !rigidbodyHand.ForceFollower.enabled;
                    }
                }
            }
        }

        /// <summary>A public method that allows you to enable or disable the 'ForceFollower' component for any RigidbodyHands that are holding the relevant grabbable object.</summary>
        /// <param name="pEnabled"></param>
        public void SetForceFollowersEnabled(bool pEnabled)
        {
            // Look for RigidbodyHand on grabber.
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null)
                    {
                        // Enable force following for the physics hand.
                        rigidbodyHand.ForceFollower.enabled = pEnabled;
                    }
                }
            }
        }
    }
}
