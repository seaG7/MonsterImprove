using UnityEngine;

namespace PhysicsHand.Demo
{
    /// <summary>
    /// A simple component used to identify a bowling ball.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Rigidbody))]
    public class BowlingBall : MonoBehaviour
    {
        /// <summary>Returns a reference to the Rigidbody associated with this bowling ball.</summary>
        public Rigidbody Rigidbody { get; private set; }

        // Untiy callback(s).
        void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }
    }
}
