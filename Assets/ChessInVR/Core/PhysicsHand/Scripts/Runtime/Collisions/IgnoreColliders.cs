using UnityEngine;

namespace PhysicsHand.Collisions
{
    /// <summary>
    /// A component that will ignore collisions with colliders in a given list.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class IgnoreColliders : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The collider(s) that you want to ignore collisions for.")]
        [SerializeField] protected Collider[] m_Colliders;
        [Tooltip("An array of Colliders to ignore on awake.")]
        [SerializeField] protected Collider[] m_IgnoreColliders;

        // Unity callback(s).
        void Awake()
        {
            // Ignore collisions.
            foreach (Collider collider in m_Colliders)
            {
                foreach (Collider ignoreCollider in m_IgnoreColliders)
                {
                    Physics.IgnoreCollision(collider, ignoreCollider);
                }
            }
        }

        // Public method(s).
        /// <summary>
        /// A public method that allows the ignore collision setting with colliders referenced in this component's settings.
        /// </summary>
        /// <param name="pIgnore"></param>
        public void SetIgnoreCollision(bool pIgnore)
        {
            foreach (Collider collider in m_Colliders)
            {
                foreach (Collider ignoreCollider in m_IgnoreColliders)
                {
                    Physics.IgnoreCollision(collider, ignoreCollider, pIgnore);
                }
            }
        }
    }
}
