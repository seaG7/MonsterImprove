using UnityEngine;
using GrabSystem;
using AdaptiveHands.Poser;
using AdaptiveHands.Triggers;

namespace PhysicsHand.KinematicHands
{
    /// <summary>
    /// A component that is attached to the same GameObject as a HandPoseArea to make the areas be 'blocked' when the HandPoser has a Grabber component that is grabbing something.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(HandPoseArea))]
    public class IgnoreHandPoseAreaInGrab : MonoBehaviour
    {
        /// <summary>A reference to the HandPoseArea component associated with this component.</summary>
        public HandPoseArea PoseArea { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find HandPoseArea reference.
            PoseArea = GetComponent<HandPoseArea>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            PoseArea.BlockHandPoseDelegate += OnBlockHandPoseDelegate;
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            if (PoseArea != null)
                PoseArea.BlockHandPoseDelegate -= OnBlockHandPoseDelegate;
        }

        // Private callback(s).
        void OnBlockHandPoseDelegate(HandPoseArea pPoseArea, HandPoser pPoser, ref bool pBlockPose)
        {
            Grabber grabber = pPoser.GetComponent<Grabber>();
            if (grabber != null && grabber.Grabbing != null)
                pBlockPose = true; 
        }
    }
}
