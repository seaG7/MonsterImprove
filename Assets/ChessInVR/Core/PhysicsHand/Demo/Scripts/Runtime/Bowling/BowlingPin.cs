using UnityEngine;

namespace PhysicsHand.Demo
{
    /// <summary>
    /// A simple component used to identify a bowling pin
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Rigidbody))]
    public class BowlingPin : MonoBehaviour
    {
        /// <summary>Returns a reference to the Rigidbody associated with this bowling pin.</summary>
        public Rigidbody Rigidbody { get; private set; }

        // Untiy callback(s).
        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
