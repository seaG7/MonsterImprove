using UnityEngine;
using UnityEngine.Events;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject that allows animations to be played on RigidbodyHands that are grabbing the GrabbableObject.
    /// The public methods Play(...) and Trigger(...) are provided to automatically play/trigger animations on any grabbing RigidbodyHands with valid 'animator' references.
    /// 
    /// By default this component will start with 'Animate' as true meaning it will override KinematicHand behaviour, you can change this behaviour by dragging it into a 'Started' event and using SetAnimated(false).
    /// </summary>
    [RequireComponent(typeof(GrabbableObject))]
    public class AnimateGrabbingHand : MonoBehaviour
    {
        #region Editor Serialized Fields
        [Header("Events")]
        [Tooltip("An event that is invoked when Start() is invoked. Useful for setting initial 'Animated' state.")]
        public UnityEvent Started;
        #endregion
        #region Public Properties
        /// <summary>Is this component currently overriding the animations for the RigidbodyHands (with 'animator' references) that are grabbing it?</summary>
        public bool Animate { get; private set; }
        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Awake()
        {
            // Find relevant GrabbableObject.
            Grabbable = GetComponent<GrabbableObject>();

            // Start with 'Animate' true.
            Animate = true;
        }

        void Start()
        {
            // Invoke the 'Started' Unity event.
            Started?.Invoke();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            if (Grabbable != null)
            {
                Grabbable.Grabbed.AddListener(OnGrabbed);
                Grabbable.Released.AddListener(OnReleased);
            }

            // If 'Animate' is enabled then enable animator for all RigidbodyHands currently grabbing the GrabbableObject.
            if (Animate)
            {
                // Disable animator for all RigidbodyHands currently grabbing the GrabbableObject.
                for (int i = 0; i < Grabbable.HeldByCount; ++i)
                {
                    Grabber grabber = Grabbable.GetHeldBy(i);
                    if (grabber != null)
                    {
                        RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                        SetRigidbodyHandAnimated(rigidbodyHand, true);
                    }
                }
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

        // Public method(s).
        #region Toggle Method(s)
        /// <summary>Enables or disables the animation override for any RigidbodyHands currently grabbing the relevant GrabbableObject.</summary>
        /// <param name="pAnimated"></param>
        public void SetAnimated(bool pAnimated)
        {
            Animate = pAnimated;

            // Update animated state for all rigidbody hands currently grabbing the GrabbableObject.
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    SetRigidbodyHandAnimated(rigidbodyHand, pAnimated);
                }
            }
        }

        /// <summary>Plays an animation clip with the given name on all RigidbodyHands (that have 'animator' references) that are currently grabbing the relevant GrabbableObject.</summary>
        /// <param name="pClip"></param>
        public void Play(string pClip)
        {
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null && rigidbodyHand.animator != null)
                    {
                        rigidbodyHand.animator.Play(pClip);
                    }
                }
            }
        }

        /// <summary>Triggers an Animator trigger with the given name on all RigidbodyHands (that have 'animator' references) that are currently grabbing the relevant GrabbableObject.</summary>
        /// <param name="pTrigger"></param>
        public void Trigger(string pTrigger)
        {
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null && rigidbodyHand.animator != null)
                    {
                        rigidbodyHand.animator.SetTrigger(pTrigger);
                    }
                }
            }
        }

        /// <summary>Sets the bool value of a parameter named pName in the Animator of all relevant RigidbodyHands (that have valid 'animator' references).</summary>
        /// <param name="pName">The name of the parameter in the Animator to set.</param>
        /// <param name="pValue">The value to set.</param>
        public void SetBool(string pName, bool pValue)
        {
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null && rigidbodyHand.animator != null)
                    {
                        rigidbodyHand.animator.SetBool(pName, pValue);
                    }
                }
            }
        }

        /// <summary>Sets the integer value of a parameter named pName in the Animator of all relevant RigidbodyHands (that have valid 'animator' references).</summary>
        /// <param name="pName">The name of the parameter in the Animator to set.</param>
        /// <param name="pValue">The value to set.</param>
        public void SetInteger(string pName, int pValue)
        {
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null && rigidbodyHand.animator != null)
                    {
                        rigidbodyHand.animator.SetInteger(pName, pValue);
                    }
                }
            }
        }

        /// <summary>Sets the float value of a parameter named pName in the Animator of all relevant RigidbodyHands (that have valid 'animator' references).</summary>
        /// <param name="pName">The name of the parameter in the Animator to set.</param>
        /// <param name="pValue">The value to set.</param>
        public void SetFloat(string pName, float pValue)
        {
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand rigidbodyHand = grabber.GetComponent<RigidbodyHand>();
                    if (rigidbodyHand != null && rigidbodyHand.animator != null)
                    {
                        rigidbodyHand.animator.SetFloat(pName, pValue);
                    }
                }
            }
        }

        /// <summary>Sets the bool value of a parameter named pName in the Animator of all relevant RigidbodyHands (that have valid 'animator' references) to true.</summary>
        /// <param name="pName">The name of the parameter in the Animator to set.</param>
        public void EnableBool(string pName) { SetBool(pName, true); }

        /// <summary>Sets the bool value of a parameter named pName in the Animator of all relevant RigidbodyHands (that have valid 'animator' references) to false.</summary>
        /// <param name="pName">The name of the parameter in the Animator to set.</param>
        public void DisableBool(string pName) { SetBool(pName, true); }
        #endregion

        // Private method(s).
        #region Rigidbody Hand Animated State Method(s)
        /// <summary>Sets whether or not a RigidbodyHand is being animated by this component.</summary>
        /// <param name="pRigidbodyHand"></param>
        /// <param name="pAnimated"></param>
        void SetRigidbodyHandAnimated(RigidbodyHand pRigidbodyHand, bool pAnimated)
        {
            if (pRigidbodyHand != null && pRigidbodyHand.KinematicHand != null && pRigidbodyHand.animator != null)
            {
                pRigidbodyHand.KinematicHand.enabled = !pAnimated;
                pRigidbodyHand.animator.enabled = pAnimated;
            }
        }
        #endregion

        // Private callback(s).
        #region Private Grabbable Callbacks
        void OnGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // If set to animate look for 'RigidbodyHand' on pGrabber, if found disable KinematicHand.
            if (Animate)
            {
                RigidbodyHand rigidbodyHand = pGrabber.GetComponent<RigidbodyHand>();
                SetRigidbodyHandAnimated(rigidbodyHand, true);
            }
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
