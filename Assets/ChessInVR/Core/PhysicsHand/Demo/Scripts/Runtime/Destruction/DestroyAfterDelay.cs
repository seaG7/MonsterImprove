using UnityEngine;

namespace PhysicsHand.Demo.Destruction
{
    /// <summary>
    /// A component that destroys the relevant 'destroyObject' after some time passes.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class DestroyAfterDelay : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the GameObject to destroy when 'DestroyTime' has been reached.")]
        public GameObject destroyObject;

        /// <summary>The Time.time to destroy the relevant destroyObject at.</summary>
        public float DestroyTime { get; set; }

        // Unity callback(s).
        void Awake()
        {
            // Never destroy by default.
            DestroyTime = float.PositiveInfinity;
        }

        void Update()
        {
            // Check if it is time to destroy.
            if (!float.IsPositiveInfinity(DestroyTime) && Time.time >= DestroyTime)
                Destroy(gameObject);
        }

        // Public method(s).
        /// <summary>Tells the component to destroy 'destroyObject' after pSeconds has elapsed.</summary>
        /// <param name="pSeconds"></param>
        public void DestroyAfterSeconds(float pSeconds)
        {
            DestroyTime = Time.time + pSeconds;
        }
    }
}
