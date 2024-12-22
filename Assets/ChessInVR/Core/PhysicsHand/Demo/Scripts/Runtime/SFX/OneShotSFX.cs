using System;
using UnityEngine;

namespace PhysicsHand.Demo.SFX
{
    /// <summary>
    /// A component intended to be used in conjunction with unity events to play one shot SFX audio.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class OneShotSFX : MonoBehaviour
    {
        // AudioClipVolumePair.
        [Serializable]
        public struct AudioClipVolumePair
        {
            [Range(0f, 1f)]
            [Tooltip("The volume the audio clip will be played at.")]
            public float volume;
            [Tooltip("The audio clip to play.")]
            public AudioClip clip;
        }

        // OneShotSFX.
        [Header("Settings")]
        [Tooltip("An array of possible sounds to be played by this component.")]
        public AudioClipVolumePair[] sounds;
        [Tooltip("(Optional) The audio source that will be used to play the sound(s).")]
        public AudioSource audioSource;

        // Public method(s).
        /// <summary>Plays a random one shot audio clip.</summary>
        public void Play()
        {
            if (sounds.Length > 0)
            {
                Play(UnityEngine.Random.Range(0, sounds.Length));
            }
            else { Debug.LogWarning("Failed to Play() using OneShotSFX component on gameObject '" + gameObject.name + "' because no sounds are set in the component!", gameObject); }
        }

        /// <summary>Plays a sound at a given index once.</summary>
        /// <param name="pSoundIndex"></param>
        public void Play(int pSoundIndex)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(sounds[pSoundIndex].clip, sounds[pSoundIndex].volume);
            }
            else { AudioSource.PlayClipAtPoint(sounds[pSoundIndex].clip, transform.position, sounds[pSoundIndex].volume); }
        }
    }
}
