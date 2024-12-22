using UnityEngine;
using GrabSystem;
using AdaptiveHands.Poser;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// Poses a RigidbodyHand after it grabs a GrabbableObject, removes pose after release.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class PoseHandOnGrab : MonoBehaviour
    {
        #region Editor Serialized Fields
        [Header("Settings")]
        [Tooltip("Should the pose be cleared when the hand releases the grabbable?")]
        public bool clearOnRelease = true;
        [Tooltip("The name of the left hand pose to play for left hand grabbers.")]
        public string leftHandPose;
        [Tooltip("The name of the right hand pose to play for right hand grabbers.")]
        public string rightHandPose;
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

            // Disable animator for all KinematicHands currently grabbing the GrabbableObject.
            for (int i = 0; i < Grabbable.HeldByCount; ++i)
            {
                Grabber grabber = Grabbable.GetHeldBy(i);
                if (grabber != null)
                {
                    RigidbodyHand hand = grabber.GetComponent<RigidbodyHand>();
                    SetHandPosed(hand, false);
                }
            }
        }
        #endregion

        // Private method(s).
        #region Hand Posed State Method(s)
        /// <summary>Sets whether or not an RigidbodyHand Hand is being posed by this component.</summary>
        /// <param name="pHand"></param>
        /// <param name="pPosed"></param>
        void SetHandPosed(RigidbodyHand pHand, bool pPosed)
        {
            // Enable posing.
            if (pPosed)
            {
                // Pose relevant hand.
                if (pHand.leftHand)
                {
                    HandPoser leftHandPoser = pHand.KinematicHand.GetComponent<HandPoser>();
                    if (leftHandPoser != null)
                    {
                        int poseIndex = leftHandPoser.GetPoseIndexByName(leftHandPose);
                        if (poseIndex != HandPoser.POSE_NONE)
                            leftHandPoser.SetPoseByIndex(poseIndex);
                    }
                }
                else
                {
                    HandPoser rightHandPoser = pHand.KinematicHand.GetComponent<HandPoser>();
                    if (rightHandPoser != null)
                    {
                        int poseIndex = rightHandPoser.GetPoseIndexByName(rightHandPose);
                        if (poseIndex != HandPoser.POSE_NONE)
                            rightHandPoser.SetPoseByIndex(poseIndex);
                    }
                }
            }
            // Disable posing.
            else
            {
                // Unpose relevant hand.
                if (pHand.leftHand)
                {
                    HandPoser leftHandPoser = pHand.KinematicHand.GetComponent<HandPoser>();
                    if (leftHandPoser != null && clearOnRelease)
                        leftHandPoser.ClearPose();
                }
                else
                {
                    HandPoser rightHandPoser = pHand.KinematicHand.GetComponent<HandPoser>();
                    if (rightHandPoser != null && clearOnRelease)
                        rightHandPoser.ClearPose();
                }
            }
        }
        #endregion

        // Private callback(s).
        #region Private Grabbable Callbacks
        void OnGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Pose the hand.
            RigidbodyHand hand = pGrabber.GetComponent<RigidbodyHand>();
            if (hand != null)
                SetHandPosed(hand, true);
        }

        void OnReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Unpose the hand.
            RigidbodyHand hand = pGrabber.GetComponent<RigidbodyHand>();
            if (hand != null)
                SetHandPosed(hand, false);
        }
        #endregion
    }
}
