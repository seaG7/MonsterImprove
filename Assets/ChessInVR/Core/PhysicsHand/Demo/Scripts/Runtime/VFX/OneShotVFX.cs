using UnityEngine;
using UnityEngine.VFX;

namespace PhysicsHand.Demo.FX
{
    /// <summary>
    /// A component that allows for one shot vfx to be defined and then played via a public method, OneShotVFX.Play().
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class OneShotVFX : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The prefab used for the one shot vfx.")]
        public GameObject vfxPrefab;
        [Tooltip("(Optional) The Transform who's position and rotation will be copied by the instantiated vfx prefab.")]
        public Transform vfxTransform;
        [Tooltip("(Optional) The parent Transform for the spawned one shot VFX.")]
        public Transform vfxParent;
        [Tooltip("The number of seconds the vfx will remain active for.")]
        public float vfxLifetime = 1.25f;
        [Tooltip("The local offset for the vfx object from the vfx transform.")]
        public Vector3 vfxOffset;
        [Tooltip("The local scale for the instantiated vfx.")]
        public Vector3 vfxLocalScale = new Vector3(1f, 1f, 1f);
        [Tooltip("The local euler angle offset for the vfx object.")]
        public Vector3 vfxEulerAngleOffset;

        /// <summary>A reference to an instance of the vfx prefab, otherwise null if none is instantiated.</summary>
        public GameObject VFXInstance { get; protected set; }

        /// <summary>
        /// The Time.time of the last VFX play.
        /// </summary>
        public float LastVFXPlay { get; protected set; }

        /// <summary>
        /// Returns true when the VFX effect is being played, otherwise false.
        /// </summary>
        public bool IsVFXPlaying { get; protected set; }

        // Unity callback(s).
        void Update()
        {
            // If the VFX effect is playing see if it is time to stop it.
            if (IsVFXPlaying)
            {
                // Check if it is time to stop the vfx.
                if (Time.time - LastVFXPlay >= vfxLifetime)
                {
                    // Stop all particle system components in the instance.
                    foreach (ParticleSystem particleSystem in VFXInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        particleSystem.Stop();
                    }

                    // Stop all visual effects in the instance.
                    foreach (VisualEffect visualEffect in VFXInstance.GetComponentsInChildren<VisualEffect>())
                    {
                        visualEffect.Stop();
                    }

                    // Deactivate the vfx.
                    VFXInstance.SetActive(false);

                    // Update the vfx playing boolean.
                    IsVFXPlaying = false;
                }
            }
        }

        // Public method(s).
        /// <summary>
        /// A public method to play the one shot vfx.
        /// </summary>
        public void Play()
        {
            // Instantiate a vfx prefab if one doesn't exist.
            if (VFXInstance == null)
            {
                VFXInstance = Instantiate(vfxPrefab, vfxParent);
                if (VFXInstance != null)
                {
                    // Set initial position if vfxTransform is not null.
                    if (vfxTransform != null)
                    {
                        VFXInstance.transform.SetPositionAndRotation(vfxTransform.position, vfxTransform.rotation);
                    }
                    else if (vfxParent != null) { VFXInstance.transform.SetPositionAndRotation(vfxParent.position, vfxParent.rotation); }

                    // Apply offset.
                    VFXInstance.transform.localPosition += vfxOffset;
                    VFXInstance.transform.localEulerAngles += vfxEulerAngleOffset;

                    // Apply local scale.
                    VFXInstance.transform.localScale = vfxLocalScale;
                }
                else { Debug.LogWarning("Failed to instantiate vfx instance of prefab '" + vfxPrefab.name + "'!", gameObject); }
            }
            else
            {
                // Activate the VFX and set it's position and orientation.
                VFXInstance.SetActive(true);

                // Set position if vfxTransform is not null.
                if (vfxTransform != null)
                    VFXInstance.transform.SetPositionAndRotation(vfxTransform.position, vfxTransform.rotation);

                // Apply offset.
                VFXInstance.transform.localPosition += vfxOffset;
                VFXInstance.transform.localEulerAngles += vfxEulerAngleOffset;

                // Apply local scale.
                VFXInstance.transform.localScale = vfxLocalScale;
            }

            // Play all particle system components in the instance.
            foreach (ParticleSystem particleSystem in VFXInstance.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Play();
            }

            // Play all visual effects in the instance.
            foreach (VisualEffect visualEffect in VFXInstance.GetComponentsInChildren<VisualEffect>())
            {
                visualEffect.Play();
            }

            // Update last vfx play and the playing status.
            IsVFXPlaying = true;
            LastVFXPlay = Time.time;
        }
    }
}
