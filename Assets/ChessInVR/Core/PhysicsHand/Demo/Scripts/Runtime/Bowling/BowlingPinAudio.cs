using UnityEngine;

namespace PhysicsHand.Demo
{
    /// <summary>
    /// A simple component that should be attached to bowling pins to play sound when hit by a bowling ball, or other bowling pins.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class BowlingPinAudio : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Balls must have a velocity square magnitude >= this value to play pin hit sounds.")]
        public float minBallSpeed = 0.3f;
        [Range(0f, 1f)]
        [Tooltip("The minimum delay between ball-hit-pin sounds.")]
        public float ballHitPinDelay = 0.3f;
        [Tooltip("The volume to play the ball hit pin clip at.")]
        public float ballHitPinVolume = 1f;
        [Tooltip("The AudioClip to play when a moving ball hits a pin.")]
        public AudioClip ballHitPinClip;
        [Tooltip("Pins must have a velocity square magnitude >= this value to play pin hit sounds.")]
        public float minPinSpeed = 0.1f;
        [Tooltip("The minimum delay between pin-hit-pin sounds.")]
        public float pinHitPinDelay = 0.3f;
        [Tooltip("The volume to play the pin hit pin clip at.")]
        public float pinHitPinVolume = 1f;
        [Tooltip("The AudioClip to play when a moving pin hits a pin.")]
        public AudioClip pinHitPinClip;

        /// <summary>The last Time.time a ball sound was played by this component.</summary>
        float m_LastBallSound;
        /// <summary>The last Time.time a pin sound was played by this component.</summary>
        float m_LastPinSound;

        // Untiy callback(s).
        void OnCollisionEnter(Collision pCollision)
        {
            BowlingBall ball = pCollision.collider.GetComponent<BowlingBall>();
            if (ball != null && ball.Rigidbody != null && ball.Rigidbody.velocity.sqrMagnitude >= minBallSpeed)
            {
                if (ballHitPinClip != null && ballHitPinVolume > 0 && Time.time - m_LastBallSound >= ballHitPinDelay)
                {
                    AudioSource.PlayClipAtPoint(ballHitPinClip, transform.position, ballHitPinVolume);
                    m_LastBallSound = Time.time;
                }
            }
            else
            {
                BowlingPin pin = pCollision.collider.GetComponent<BowlingPin>();
                if (pin == null && pCollision.collider.attachedRigidbody != null)
                    pin = pCollision.collider.attachedRigidbody.GetComponent<BowlingPin>();
                if (pin != null && pin.Rigidbody != null && pin.Rigidbody.velocity.sqrMagnitude >= minPinSpeed)
                {
                    if (pinHitPinClip != null && pinHitPinVolume > 0 && Time.time - m_LastPinSound >= pinHitPinDelay)
                    {
                        AudioSource.PlayClipAtPoint(pinHitPinClip, transform.position, pinHitPinVolume);
                        m_LastPinSound = Time.time;
                    }
                }
            }
        }
    }
}
