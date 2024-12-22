using UnityEngine;
using System.Collections.Generic;

namespace PhysicsHand.Demo
{
    /// <summary>
    /// A simple component used in conjunction with a trigger to detect bowling balls 'rolling' on the lane.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class BowlingLaneAudio : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Balls must have a velocity square magnitude >= this value to be considered 'moving' in the lane.")]
        public float minBallSpeed = 0.3f;
        [Tooltip("The audio source to play the bowling lane audio on.")]
        public AudioSource audioSource;

        /// <summary>A list of BowlingBalls that are currently touching the lane.</summary>
        List<BowlingBall> m_BowlingBalls = new List<BowlingBall>();

        // Untiy callback(s).
        void Update()
        {
            // Determine whether or not a bowling ball is moving down the lane.
            BowlingBall movingBall = null;
            foreach (BowlingBall ball in m_BowlingBalls)
            {
                if (ball != null)
                {
                    if (ball.Rigidbody != null && ball.Rigidbody.velocity.sqrMagnitude >= minBallSpeed)
                    {
                        movingBall = ball;
                        break;
                    }
                }
            }

            // Update audio source.
            if (audioSource != null)
            {
                // Move the audio source with the first ball 'moving in lane'.
                if (movingBall != null)
                {
                    // Position the bowling lane audio source.
                    audioSource.transform.position = movingBall.transform.position;

                    // Play the bowling lane audio source.
                    if (!audioSource.isPlaying)
                        audioSource.Play();
                }
                else if (audioSource.isPlaying) { audioSource.Stop(); }
            }
        }

        void OnTriggerEnter(Collider pOther)
        {
            BowlingBall ball = pOther.GetComponent<BowlingBall>();
            if (ball != null && !m_BowlingBalls.Contains(ball))
                m_BowlingBalls.Add(ball);
        }

        void OnTriggerExit(Collider pOther)
        {
            BowlingBall ball = pOther.GetComponent<BowlingBall>();
            if (ball != null && m_BowlingBalls.Contains(ball))
                m_BowlingBalls.Remove(ball);
        }
    }
}
