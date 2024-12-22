using UnityEngine;
using GrabSystem;

namespace PhysicsHand.PlaceSystem
{
    /// <summary>
    /// A component that makes a grabbable return to a place point when it is released for a given amount of time.
    /// The grabbable to return to the placePoint is determined by the 'pGrabbable' argument passed to the OnReleased method
    /// WARNING: This component is only designed to be-able to return a single grabbable at a time.
    /// NOTE:    This component also provides a helper function to make it work with place points 'OnRemovedFromPlacePoint'.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ReturnToPlacePointOnRelease : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The placepoint to return grabbable(s) to.")]
        public PlacePoint placePoint;
        [Min(0)]
        [Tooltip("The number of seconds after release the grabbable will be returned to the place point.")]
        public float returnAfterSeconds;

        /// <summary>The GrabbableObject to return to the placepoint after the given timespan (or null if none).</summary>
        GrabbableObject m_GrabbableToReturn;
        /// <summary>The Time.time the grabbable should be returned to it's placePoint at assuming it is still unheld.</summary>
        float m_ReturnToPlacePointTime;

        // Unity callback(s).
        void Start()
        {
            // Ensure a place point reference is set.
            if (placePoint == null)
                Debug.LogWarning("No 'placePoint' field was set for ReturnToPlacePointOnRelease component attaced to gameObject '" + gameObject.name + "'.", gameObject);
        }

        void Update()
        {
            // Check if it's time to return a grabbable to it's placepoint.
            if (m_GrabbableToReturn != null)
            {
                if (Time.time >= m_ReturnToPlacePointTime)
                {
                    // Ensure the grabbable is not being held anymore.
                    if (m_GrabbableToReturn.HeldByCount == 0)
                    {
                        // Return the grabbable to it's placepoint.
                        placePoint.Place(m_GrabbableToReturn);

                        // No longer need to return a grabbable to it's placepoint because it was just returned.
                        m_GrabbableToReturn = null;
                    }
                    else
                    {
                        // No longer need to return a grabbable to it's placepoint because it is being held again.
                        m_GrabbableToReturn = null;
                    }
                }
            }
        }

        // Public method(s).
        /// <summary>A public method that allows the 'placePoint' reference of this component to be set. Intended for use with editor events.</summary>
        /// <param name="pPoint"></param>
        public void SetPlacePoint(PlacePoint pPoint)
        {
            placePoint = pPoint;
        }

        // Public callback(s).
        /// <summary>A callback that is intended to be invoked by a Grabbable component's 'Released' unity event.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        public void OnReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            m_GrabbableToReturn = pGrabbable;
            m_ReturnToPlacePointTime = Time.time + returnAfterSeconds;
        }

        /// <summary>A callback that is intended to be invoked by a PlacePoint component's 'Released' unity event.</summary>
        /// <param name="pPlacePoint"></param>
        /// <param name="pGrabbable"></param>
        public void OnReleasedFromPlacePoint(PlacePoint pPlacePoint, GrabbableObject pGrabbable)
        {
            m_GrabbableToReturn = pGrabbable;
            m_ReturnToPlacePointTime = Time.time + returnAfterSeconds;
        }
    }
}
