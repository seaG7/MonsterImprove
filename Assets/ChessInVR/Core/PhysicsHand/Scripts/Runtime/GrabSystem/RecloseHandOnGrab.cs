using UnityEngine;
using GrabSystem;
using AdaptiveHands;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject that automatically forces any Grabber who grabs it that has a KinematicHand component to zero the 'current' finger bend for the kinematic hand (not the target finger bend).
    /// USAGE: Simply attach to the same GameObject as the GrabbableObject you want KinematicHand Grabbers to regrab.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class RecloseHandOnGrab : MonoBehaviour
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
            // Look for KinematicHand on grabber.
            KinematicHand kinematicHand = pGrabber.GetComponent<KinematicHand>();
            if (kinematicHand != null)
            {
                // Zero the current finger bend on all fingers in the hand.
                kinematicHand.ZeroAllFingerCurrentBend();
            }
        }
    }
}
