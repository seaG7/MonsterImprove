using UnityEngine;
using GrabSystem;
using AdaptiveHands.Triggers;
using AdaptiveHands.BendStates;

namespace PhysicsHand.KinematicHands
{
    /// <summary>
    /// A component that is attached to the same GameObject as a BendStateArea to make the areas be 'blocked' when the BendStateSwapper has a Grabber component that is grabbing something.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(BendStateArea))]
    public class IgnoreBendStateAreaInGrab : MonoBehaviour
    {
        /// <summary>A reference to the BendStateArea component associated with this component.</summary>
        public BendStateArea BendStateArea { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find BendStateArea reference.
            BendStateArea = GetComponent<BendStateArea>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            BendStateArea.BlockBendStateSwapDelegate += OnBlockBendStateSwapDelegate;
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            if (BendStateArea != null)
                BendStateArea.BlockBendStateSwapDelegate -= OnBlockBendStateSwapDelegate;
        }

        // Private callback(s).
        void OnBlockBendStateSwapDelegate(BendStateArea pPoseArea, BendStateSwapper pPoser, ref bool pBlockStateSwap)
        {
            Grabber grabber = pPoser.GetComponent<Grabber>();
            if (grabber != null && grabber.Grabbing != null)
                pBlockStateSwap = true; 
        }
    }
}
