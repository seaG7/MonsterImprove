using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using PhysicsHand.Demo.Triggers;

namespace PhysicsHand.Demo.Grenades
{
    /// <summary>
    /// A grenade that has a detonation timer and applies force to anything that is in its 'trigger'.
    /// 
    /// Can be used as a base class to derive further grenade types from.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ForceGrenade : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The number of seconds before the grenade explodes.")]
        public float secondsTilDetonate = 5f;
        [Range(0f, 1f)]
        [Tooltip("A multiplier for detonation sound volume for this grenade. (0->1)")]
        public float detonateSoundVolume = 1f;
        [Tooltip("A reference to the AudioCLip to be played when this Grenade detonates.")]
        public AudioClip detonateSound;
        [Tooltip("A reference to the component that tracks Rigidbodies in the trigger of the 'area of effect' of the grenade.")]
        public TriggerTracker aoeTracker;

        [Header("Settings - Physics")]
        [Tooltip("The force this grenade detonates with.")]
        public float detonateForce = 500f;

        [Header("Events")]
        [Tooltip("A method that is invoked when the grenade timer is started.")]
        public UnityEvent TimerStarted;
        [Tooltip("An event that is invoked when this Grenade's Detonate() method is invoked.")]
        public UnityEvent Detonated;

        /// <summary>A reference to the Time.time this grenades timer was started at.</summary>
        public float StartTime { get; protected set; }
        /// <summary>Returns the Time.time the grenade will detonate at.</summary>
        public float TimeTilDetonate { get { return secondsTilDetonate - (Time.time - StartTime); } }

        // Unity callback(s).
        protected virtual void Awake()
        {
            // Start time awakes at positive infinity.
            StartTime = float.PositiveInfinity;
        }

        protected virtual void Update()
        {
            // If it's time to detonate...
            if (!float.IsInfinity(StartTime) && TimeTilDetonate <= 0)
            {
                // Detonate the grenade.
                Detonate();
            }
        }

        // Public method(s).
        /// <summary>Starts the grenades timer.</summary>
        public void StartTimer()
        {
            // Record start time.
            StartTime = Time.time;

            // Invoke the 'TimerStarted' Unity event.
            TimerStarted?.Invoke();

            // Invoke the 'OnTimerStarted()' callback.
            OnTimerStarted();
        }

        /// <summary>Starts the grenades timer if the component is enabled.</summary>
        public void StartTimerIfEnabled()
        {
            if (enabled)
                StartTimer();
        }     

        /// <summary>Detonates the grenade.</summary>
        public void Detonate()
        {
            // Invoke the 'Detonated' unity event.
            Detonated?.Invoke();

            // Play the detonate sound if set.
            if (detonateSound != null)
                AudioSource.PlayClipAtPoint(detonateSound, transform.position, detonateSoundVolume);

            // Loop over every body in AOE tracker.
            if (aoeTracker != null)
            {
                foreach (Rigidbody body in aoeTracker.BodiesInTrigger)
                {
                    // Ensure body is still valid.
                    if (body != null)
                    {
                        // Calculate distance & direction from detonation.
                        float distanceFromDetonation = Vector3.Distance(body.position, transform.position);
                        Vector3 directionFromDetonation = (body.position - transform.position).normalized;

                        // Apply force to this body at the position of the blast.
                        body.AddForceAtPosition(directionFromDetonation * detonateForce, transform.position, ForceMode.Impulse);
                    }
                }
            }

            // Invoke the 'OnDetonated' callback. (WARNING: No code after this as the GameObject may be destroyed.)
            OnDetonated();            
        }

        // Protected virtualcallback(s).
        /// <summary>Invoked when the grenade timer is started.</summary>
        protected virtual void OnTimerStarted() { }
        /// <summary>Invoked when the grenade is detonated. It is safe to destroy the gameObject in this callback, by default 'OnDetonated' (unoverloaded) does.</summary>
        protected virtual void OnDetonated()
        {
            // Lastly destroy the grenade's gameObject.
            Destroy(gameObject);
        }
    }
}
