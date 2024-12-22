using UnityEngine;

namespace PhysicsHand.Events
{
    /// <summary>
    /// A component that can be attached to any Rigidbody or Collider that will invoke various events when a RigidbodyHand starts, continues, or stops colliding with the relevant collider(s).
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HandCollisionEvent : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked whenever a RigidbodyHand enters a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that entered the collision.")]
        public RigidbodyHandUnityEvent CollisionEntered;
        [Tooltip("An event that is invoked whenever a RigidbodyHand stays in a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that stayed in the collision.")]
        public RigidbodyHandUnityEvent CollisionStayed;
        [Tooltip("An event that is invoked whenever a RigidbodyHand exits a collision with this component.\n\nArg0: RigidbodyHand - The RigidbodyHand that exited the collision.")]
        public RigidbodyHandUnityEvent CollisionExited;

        // Unity callback(s).
        void OnCollisionEnter(Collision pCollision)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollision.collider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollision.collider.attachedRigidbody != null)
                rigidbodyHand = pCollision.collider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                CollisionEntered?.Invoke(rigidbodyHand);
        }

        void OnCollisionStay(Collision pCollision)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollision.collider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollision.collider.attachedRigidbody != null)
                rigidbodyHand = pCollision.collider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                CollisionStayed?.Invoke(rigidbodyHand);
        }

        void OnCollisionExit(Collision pCollision)
        {
            // Check for RigidbodyHand component.
            RigidbodyHand rigidbodyHand = pCollision.collider.GetComponent<RigidbodyHand>();
            if (rigidbodyHand == null && pCollision.collider.attachedRigidbody != null)
                rigidbodyHand = pCollision.collider.attachedRigidbody.GetComponent<RigidbodyHand>();

            // If a RigidbodyHand component was found invoke the relevant event.
            if (rigidbodyHand != null)
                CollisionExited?.Invoke(rigidbodyHand);
        }
    }
}
