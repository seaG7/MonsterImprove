using UnityEngine;
using UnityEngine.Events;
using GrabSystem;
using PhysicsHand.Animation;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject that makes it so
	/// any found HandAnimators play the relevant animation clip.
    /// </summary>
	/// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class AnimateHandOnGrab : MonoBehaviour
    {
        #region Editor Serialized Fields
		[Header("Settings")]
		[Tooltip("The name of the left hand animation clip to play for left hand grabbers.")]
		public string leftHandState;
		[Tooltip("The name of the right hand animation clip to play for right hand grabbers.")]
		public string rightHandState;
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
                Grabbable.Grabbed.AddListener(OnGrabbed);
                Grabbable.Released.AddListener(OnReleased);
            }
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            if (Grabbable != null)
            {
                Grabbable.Grabbed.RemoveListener(OnGrabbed);
                Grabbable.Released.RemoveListener(OnReleased);
            }

            // Disable animator for all RigidbodyHands currently grabbing the GrabbableObject.
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    SetRigidbodyHandAnimated(rigidbodyHand, false);
                }
            }
        }
        #endregion
		
        // Private method(s).
        #region Rigidbody Hand Animated State Method(s)
        /// <summary>Sets whether or not a RigidbodyHand is being animated by this component.</summary>
        /// <param name="pRigidbodyHand"></param>
        /// <param name="pAnimated"></param>
        void SetRigidbodyHandAnimated(RigidbodyHand pRigidbodyHand, bool pAnimated)
        {
			if (pRigidbodyHand != null)
			{
				// Set 'KinematicHand' enabled state based on if animation is overriding or not.
				if (pRigidbodyHand.KinematicHand != null)
					pRigidbodyHand.KinematicHand.enabled = !pAnimated;
				
				// Look for 'HandAnimator'.
				HandsAnimator animator = pRigidbodyHand.GetComponentInParent<HandsAnimator>();
				if (animator == null)
					animator = pRigidbodyHand.GetComponentInChildren<HandsAnimator>(true);
				if (animator != null)
				{
                    // If 'pAnimated' is true look for 'HandAnimator' component to animate.
                    if (pAnimated)
                    {
                        // HandAnimator found! Animate it.
                        if (pRigidbodyHand.leftHand)
                        {
                            animator.EnableLeftAnimators();
                            animator.PlayLeftHand(leftHandState);
                        }
                        else
                        {
                            animator.EnableRightAnimators();
                            animator.PlayRightHand(rightHandState);
                        }
                    }
                    // Otherwise simply disable animators.
                    else
                    {
                        // Choose which hand side to disable.
                        if (pRigidbodyHand.leftHand)
                        {
                            animator.DisableLeftAnimators();
                        }
                        else
                        {
                            animator.DisableRightAnimators();
                        }
                    }
				}
			}
        }
        #endregion

        // Private callback(s).
        #region Private Grabbable Callbacks
        void OnGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
			// Animate the hand.
			RigidbodyHand rigidbodyHand = pGrabber.GetComponent<RigidbodyHand>();
            SetRigidbodyHandAnimated(rigidbodyHand, true);
        }

        void OnReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Restore kinematic hand enabled status on rigidbodyHand's KinematicHand reference if found.
            RigidbodyHand rigidbodyHand = pGrabber.GetComponent<RigidbodyHand>();
            SetRigidbodyHandAnimated(rigidbodyHand, false);
        }
        #endregion
    }
}
