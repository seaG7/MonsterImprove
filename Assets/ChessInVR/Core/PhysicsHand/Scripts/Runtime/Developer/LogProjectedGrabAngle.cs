using UnityEngine;
using GrabSystem;

namespace PhysicsHand.Developer
{
    /// <summary>
    /// A simple component to aid developers by logging the projected grab angle relative to some local axis of the grabbed GrabbableObject at the time of grab.
    /// Useful for configuring 'MaintainOffsetByProjectedAngle' settings.
    /// This component should not be shipped with your built game.
    /// </summary>
    [RequireComponent(typeof(GrabbableObject))]
    public class LogProjectedGrabAngle : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The upwards direction vector to test against. (In GrabbableObjects local space.)")]
        public Vector3 upAxis = Vector3.up;
        [Tooltip("The axis to project onto and get the angle around. (In GrabbableObjects local space.)")]
        public Vector3 angleAxis = Vector3.right;
        [Tooltip("The center point Transform to get the angle around. [If null 'transform' will be used.]")]
        [SerializeField] Transform m_CenterPoint;

        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }
        /// <summary>The center point Transform to get the angle around.</summary>
        public Transform CenterPoint { get { return m_CenterPoint != null ? m_CenterPoint : transform; } }

        // Unity callback(s).
        void Awake()
        {
            // Find 'Grabbable' reference.
            Grabbable = GetComponent<GrabbableObject>();
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            Grabbable.PreGrabbed.AddListener(OnPreGrabbed);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            Grabbable.PreGrabbed.RemoveListener(OnPreGrabbed);
        }

        // Private callback(s).
        void OnPreGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            Vector3 worldAxisPlane = pGrabbable.transform.TransformDirection(angleAxis);
            Vector3 projectedCenterPos = Vector3.ProjectOnPlane(CenterPoint.position, worldAxisPlane);
            Vector3 projectedGrabberPos = Vector3.ProjectOnPlane(pGrabber.transform.position, worldAxisPlane);
            Vector3 offsetDirection = projectedCenterPos - projectedGrabberPos;
            float angle = Vector3.SignedAngle(Grabbable.transform.TransformDirection(upAxis), offsetDirection, worldAxisPlane);
            if (angle < 0)
                angle += 360;

            // Log the projected angle.
            Debug.Log("Projected angle: " + angle);
        }
    }
}
