using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that forces a Grabber to grab a specific Grabbable while it's within a trigger and release it when the Grabber gets a certain distance from said trigger.
    /// NOTE: This component will only force a grab if the Grabber entering the trigger is not already holding a GrabbableObject.
    /// </summary>
    /// Author: Mathew Aloisio
    public class ForceGrabWithinTrigger : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The GrabbableObject that should be attempt to be grabbed when an empty Hand enters the trigger.")]
        public GrabbableObject grabbable;
        [Tooltip("Should this component check an 'attachedRigidbody' when a Collider with one interacts with the trigger?")]
        public bool checkRigidbody;
        [Min(0f)]
        [Tooltip("The distance between the hand's follow transform position and the referenced grabbable's position at or over which the hand will release the Grabbable.")]
        public float releaseDistance = 0.25f;
        [Min(0f)]
        [Tooltip("The number of seconds the grabbable must be out of range for before a release will occur.")]
        public float releaseDelay = 0.1f;
        [Min(0f)]
        [Tooltip("The minimum number of seconds after releasing the force grab before the component will force a new grab.")]
        public float regrabDelay = 0.3f;
        [Min(0f)]
        [Tooltip("The minimum number of seconds before try grab attempts.")]
        public float tryGrabDelay = 0.1f;

        [Header("Maintain Grab Offset")]
        [Tooltip("The position in 'transform' local space the grabber is forced to if 'grabbable' is using 'Maintain Offset' mode.")]
        public Vector3 grabberOffset;
        [Tooltip("The euler angles the grabber is forced to if 'grabbable' is using 'Maintain Offset' mode.")]
        public Vector3 grabberEulerAngles;

        /// <summary>A reference to the Grabber that is currently grabbing the referenced grabbable object.</summary>
        Grabber m_Grabber;
        /// <summary>The Time.time the next force grab is allowed. (This is used to prevent unintentional re-grabs when releasing.)</summary>
        float m_NextAllowedGrabTime;
        /// <summary>The last Time.time that the hand in trigger's Grabbable has been out of range for.</summary>
        float m_LastInRangeTime;

        // Unity callback(s).
        void Start()
        {
            // Ensure a grabbable reference is set, otherwise spew a warning.
            if (grabbable == null)
                Debug.LogWarning("No 'grabbable' reference set in ForceGrabWithinTrigger component on gameObject '" + gameObject.name + "'!", gameObject);
        }

        void OnDisable()
        {
            // Force release on disable.
            Private_ForceRelease();
        }

        void FixedUpdate()
        {
            // Only check anything if there is a Grabber grabbing the grabbable object via this component.
            if (m_Grabber != null)
            {
                // Ensure the Grabber is holding is holding the referenced grabbable.
                if (m_Grabber.Grabbing == grabbable)
                {
                    // Check if the grabber is out of range.
                    Transform followTransform = GrabUtilities.GetFollowTransform(m_Grabber);
                    if (followTransform == null || !IsGrabbableInRange(followTransform.position))
                    {
                        // If the grabbable has been out of range long enough force release it.
                        if (Time.time - m_LastInRangeTime >= releaseDelay)
                            Private_ForceRelease();
                    }
                    else { m_LastInRangeTime = Time.time; } // Set last in range time to n.w
                }
                else { m_Grabber = null; }
            }
        }

        void OnTriggerEnter(Collider pOther)
        {
            // Only bother attempting a grab if another grabber is not already in the trigger grabbing.
            if (m_Grabber == null)
            {
                // Only allow a grab attempt if the current time is greater than or equal to the next allowed grab time.
                if (Time.time >= m_NextAllowedGrabTime)
                {
                    RayGrabber grabber = pOther.GetComponent<RayGrabber>();
                    if (grabber == null && checkRigidbody && pOther.attachedRigidbody != null)
                        grabber = pOther.attachedRigidbody.GetComponent<RayGrabber>();
                    if (grabber != null)
                    {
                        // Try to grab the referenced GrabbableObject with the 'grabber'.
                        TryGrab(grabber);
                    }
                }
            }
        }

        void OnTriggerStay(Collider pOther)
        {
            // Only allow a grab attempt if the current time is greater than or equal to the next allowed grab time.
            if (Time.time >= m_NextAllowedGrabTime)
            {
                // Try to grab the referenced GrabbableObject with a 'grabber' in the referenced Collider.
                RayGrabber grabber = pOther.GetComponent<RayGrabber>();
                if (grabber == null && checkRigidbody && pOther.attachedRigidbody != null)
                    grabber = pOther.attachedRigidbody.GetComponent<RayGrabber>();
                if (grabber != null)
                {
                    // Try to grab the referenced GrabbableObject with the 'grabber'.
                    TryGrab(grabber);
                }
            }
        }

        // Public method(s).
        /// <summary>A public method that allows the next allowed grab time to be set to Time.time + pDelayInSeconds.</summary>
        /// <param name="pDelayInSeconds"></param>
        public void SetNextAllowedGrabDelay(float pDelayInSeconds)
        {
            m_NextAllowedGrabTime = Time.time + pDelayInSeconds;
        }

        /// <summary>A public method that forcefully makes the hand release if it's holding the referenced GrabbableObject.</summary>
        public void ForceRelease()
        {
            // Only force a release if there is a grabber in the trigger.
            if (m_Grabber != null)
            {
                // Ensure the Grabber is holding is holding the referenced grabbable object.
                if (m_Grabber.Grabbing == grabbable)
                {
                    // Release the object since the hand's follow transform is too far away from the grabbable.
                    m_Grabber.ForceRelease();
                }

                // Update next allowed grab time.
                m_NextAllowedGrabTime = Time.time + regrabDelay;

                // Nullify 'm_Grabber' reference.
                m_Grabber = null;
            }
        }

        // Private method(s).
        /// <summary>Tries to grab using the Grabber, pGrabber.</summary>
        /// <param name="pHand"></param>
        void TryGrab(Grabber pGrabber)
        {
            // If a grabber that is not holding anything entered the trigger force it to attempt a grab.
            if (pGrabber.Grabbing == null)
            {
                // Only grab if the grabbable is within range.
                Transform followTransform = GrabUtilities.GetFollowTransform(pGrabber);
                if (followTransform == null || IsGrabbableInRange(followTransform.position))
                {
                    // Force the grabber to the appropriate position and rotation if 'grabbable' is in 'Maintain Offset' mode.
                    if (grabbable.grabMode == GrabbableObject.GrabMode.MaintainOffset)
                    {
                        pGrabber.transform.position = transform.TransformPoint(grabberOffset);
                        pGrabber.transform.eulerAngles = transform.TransformDirection(grabberEulerAngles);
                    }

                    // Attempt the grab, set hand in trigger reference.
                    pGrabber.Grab(grabbable);
                    m_Grabber = pGrabber;

                    // Lock the grabber.
                    if (pGrabber.Grabbing == grabbable)
                        pGrabber.SetGrabLocked(true);

                    // Update next allowed grab time.
                    m_NextAllowedGrabTime = Time.time + tryGrabDelay;
                }
            }
        }

        /// <summary>
        /// Forces the 'hand in trigger' to release.
        /// </summary>
        void Private_ForceRelease()
        {
            // Forcibly release the object since the hand's follow transform is too far away from the grabbable.
            if (m_Grabber != null)
            {
                m_Grabber.SetGrabLocked(false);
                m_Grabber.ForceRelease();
            }

            // Update next allowed grab time.
            m_NextAllowedGrabTime = Time.time + regrabDelay;

            // Nullify 'm_Grabber' reference.
            m_Grabber = null;
        }

        /// <summary>
        /// Returns true if the referenced Grabbable is in range of the follower of the hand performing the force grab, otherwise false.
        /// </summary>
        /// <param name="pHandFollowPosition">The position of the Hand's follow transform.</param>
        /// <returns>true if the referenced Grabbable is in range of the follower of the hand performing the force grab, otherwise false.</returns>
        bool IsGrabbableInRange(Vector3 pHandFollowPosition)
        {
            return Vector3.Distance(pHandFollowPosition, grabbable.transform.position) < releaseDistance;
        }
    }
}
