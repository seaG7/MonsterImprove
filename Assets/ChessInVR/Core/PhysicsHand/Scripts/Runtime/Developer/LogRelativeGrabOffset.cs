using UnityEngine;
using GrabSystem;

namespace PhysicsHand.Developer
{
    /// <summary>
    /// A simple component to aid developers by logging the relative position and rotation of a grabbed object from the hand once grabbed.
    /// This component should not be shipped with your built game.
    /// </summary>
    [RequireComponent(typeof(GrabbableObject))]
    public class LogRelativeGrabOffset : MonoBehaviour
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
            Grabbable.PreGrabbed.AddListener(OnPreGrabbed);
            Grabbable.Released.AddListener(OnReleased);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            Grabbable.PreGrabbed.RemoveListener(OnPreGrabbed);
            Grabbable.Released.RemoveListener(OnReleased);
        }

        // Private callback(s).
        void OnPreGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            Vector3 localPosition = pGrabber.transform.InverseTransformPoint(pGrabbable.transform.position);
            Vector3 localEulerAngles = pGrabber.transform.InverseTransformDirection(pGrabbable.transform.eulerAngles - pGrabber.transform.eulerAngles);
            Debug.Log("Grabbed " + pGrabbable.name + " at local position " + localPosition + " and local euler angles " + localEulerAngles + " relative to grabber " + pGrabber.name + ".", pGrabbable.gameObject);
        }

        void OnReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            Vector3 localPosition = pGrabber.transform.InverseTransformPoint(pGrabbable.transform.position);
            Vector3 localEulerAngles = pGrabber.transform.InverseTransformDirection(pGrabbable.transform.eulerAngles - pGrabber.transform.eulerAngles);
            Debug.Log("Released " + pGrabbable.name + " at local position " + localPosition + " and local euler angles " + localEulerAngles + " relative to grabber " + pGrabber.name + ".", pGrabbable.gameObject);
        }
    }
}
