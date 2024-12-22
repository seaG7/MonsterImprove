using UnityEngine;
using System.Collections.Generic;

namespace PhysicsHand.Demo.Triggers
{
    /// <summary>
    /// A component that tracks all Collider and Rigidbody components inside a trigger.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class TriggerTracker : MonoBehaviour
    {
        /// <summary>A list of all Rigidbody components currently inside this gameObject's trigger.</summary>
        public List<Rigidbody> BodiesInTrigger { get; private set; } = new List<Rigidbody>();
        /// <summary>A list of all Collider components currently inside this gameObject's trigger. </summary>
        public List<Collider> CollidersInTrigger { get; private set; } = new List<Collider>();

        // Unity callback(s).
        void OnTriggerEnter(Collider pOther)
        {
            // Add pOther's Rigidbody reference if one exists.
            if (pOther.attachedRigidbody != null && !BodiesInTrigger.Contains(pOther.attachedRigidbody))
                BodiesInTrigger.Add(pOther.attachedRigidbody);

            // Add the collider to the list of colliders in the trigger if not already in it.
            if (!CollidersInTrigger.Contains(pOther))
                CollidersInTrigger.Add(pOther);
        }

        void OnTriggerExit(Collider pOther)
        {
            // Remove pOther's Rigidbody reference if one exists.
            if (pOther.attachedRigidbody != null)
                BodiesInTrigger.Remove(pOther.attachedRigidbody);

            // Remove the collider from the list of colliders in the trigger.
            CollidersInTrigger.Remove(pOther);
        }
    }
}
