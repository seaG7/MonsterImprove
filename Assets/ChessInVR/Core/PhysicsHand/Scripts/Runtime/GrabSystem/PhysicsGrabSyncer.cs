using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// An optional component that syncs all Transforms with the physics system before key grab events to ensure completely accurate grabbing at a slight performance cost.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(RigidbodyHand))]
    public class PhysicsGrabSyncer : MonoBehaviour
    {
        #region Public Properties
        /// <summary>A reference to the RigidbodyHand associated with this component.</summary>
        public RigidbodyHand Hand { get; private set; }
        #endregion

        // Unity callback(s).
        #region Unity Callback(s)
        void Awake()
        {
            // Find the 'RigidbodyHand' component reference.
            Hand = GetComponent<RigidbodyHand>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            if (Hand.KinematicHand != null)
            {
                Hand.KinematicHand.PreHandUpdated.AddListener(OnPreKinematicHandUpdated);
            }
            if (Hand.Grabber != null)
            {
                Hand.Grabber.PreGrabAttempted.AddListener(OnPreGrabAttempted);
            }
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            if (Hand != null)
            {
                if (Hand.KinematicHand != null)
                {
                    Hand.KinematicHand.PreHandUpdated.RemoveListener(OnPreKinematicHandUpdated);
                }
                if (Hand.Grabber != null)
                {
                    Hand.Grabber.PreGrabAttempted.RemoveListener(OnPreGrabAttempted);
                }
            }
        }
        #endregion

        // Public method(s).
        #region Physics Sync Method(s)
        /// <summary>Syncs all Colliders and Rigidbodys in the component with their Transforms.</summary>
        public void SyncTransforms()
        {
            // Sync physics transforms.
            Physics.SyncTransforms();
        }
        #endregion

        // Private callback(s).
        #region Grabber Callback(s).
        /// <summary>Invoked just before the Grabber attempts a conditional grab.</summary>
        /// <param name="pGrabber"></param>
        void OnPreGrabAttempted(Grabber pGrabber)
        {
            // Sync physics transforms.
            SyncTransforms();
        }

        /// <summary>Invoked just before the relevant KinematicHand is updated..</summary>
        void OnPreKinematicHandUpdated()
        {
            // Sync physics transforms.
            SyncTransforms();
        }
        #endregion
    }
}
