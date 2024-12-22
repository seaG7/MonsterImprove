using UnityEngine;
using GrabSystem;

namespace PhysicsHand.GrabSystem
{
    /// <summary>
    /// A component that is attached to the same GameObject as a GrabbableObject (with a Rigidbody, otherwise useless) to adjust its mass while it is grabbed.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(GrabbableObject))]
    public class GrabbableMassAdjuster : MonoBehaviour
    {
        [Header("Settings")]
        [Min(0f)]
        [Tooltip("The mass of the relevant grabbable object while being held.")]
        public float heldMass = 0.5f;
        [Tooltip("Restore original mass on release?")]
        public bool restoreOnRelease = true;

        /// <summary>A reference to the GrabbableObject associated with this component.</summary>
        public GrabbableObject Grabbable { get; private set; }
        /// <summary>The cached mass for the GrabbableObject.</summary>
        public float CachedMass { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find 'Grabbable' reference.
            Grabbable = GetComponent<GrabbableObject>();

            // If the Grabbable has a Rigidbody subscribe to events.
            if (Grabbable.Rigidbody != null)
            {
                // Update cached mass.
                UpdateCachedMass();
            }
            // Otherwise log warning if there is no 'Rigidbody' on the grabbable.
            else { Debug.LogWarning("GrabbableObject associated with GrabbableMassAdjuster component has no Rigidbody! Component has no use.", gameObject); }
        }

        void OnEnable()
        {
            // Subscribe to relevant event(s).
            Grabbable.Grabbed.AddListener(OnGrabbed);
            Grabbable.Released.AddListener(OnReleased);
        }

        void OnDisable()
        {
            // Unsubscribe from relevant event(s).
            Grabbable.Grabbed.RemoveListener(OnGrabbed);
            Grabbable.Released.RemoveListener(OnReleased);
        }

        // Public method(s).
        /// <summary>Updates the cached mass value for the Grabbable's Rigidbody.</summary>
        public void UpdateCachedMass()
        {
            if (Grabbable.Rigidbody != null)
                CachedMass = Grabbable.Rigidbody.mass;
        }

        // Private callback(s).
        void OnGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Set to held mass.
            if (Grabbable.Rigidbody != null)
                Grabbable.Rigidbody.mass = heldMass;
        }

        void OnReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Ensure there is a valid Rigidbody.
            if (Grabbable.Rigidbody != null)
            {
                // Restore the mass of the Rigidbody only if the last grabbing hand released it.
                if (Grabbable.HeldByCount <= 0)
                {
                    // Restore to cached mass if enabled.
                    if (restoreOnRelease)
                        Grabbable.Rigidbody.mass = CachedMass;
                }
            }
        }
    }
}
