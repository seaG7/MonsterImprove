using UnityEngine;

namespace PhysicsHand.Events
{
    /// <summary>
    /// A component that can be attached to any Rigidbody or Collider that will invoke various events when a RigidbodyHand starts, continues, or stops triggering the relevant collider(s).
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HandTriggerEvent : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked whenever a RigidbodyHand enters a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that entered the trigger.")]
        public RigidbodyHandUnityEvent TriggerEntered;
        [Tooltip("An event that is invoked whenever a RigidbodyHand stays in a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that stayed in the  trigger.")]
        public RigidbodyHandUnityEvent TriggerStayed;
        [Tooltip("An event that is invoked whenever a RigidbodyHand exits a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that exited the trigger.")]
        public RigidbodyHandUnityEvent TriggerExited;

        // Unity callback(s).
        void OnTriggerEnter(Collider pCollider)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollider.attachedRigidbody != null)
                rigidbodyHand = pCollider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                TriggerEntered?.Invoke(rigidbodyHand);
        }

        void OnTriggerStay(Collider pCollider)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollider.attachedRigidbody != null)
                rigidbodyHand = pCollider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                TriggerStayed?.Invoke(rigidbodyHand);
        }

        void OnTriggerExit(Collider pCollider)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollider.attachedRigidbody != null)
                rigidbodyHand = pCollider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                TriggerExited?.Invoke(rigidbodyHand);
        }
    }
}
