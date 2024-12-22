using UnityEngine;

namespace PhysicsHand.Animation
{
    /// <summary>
    /// A component that can apply animations to the left and or right hand.
	/// Intended to be attached to a parent object of both hands.
    /// </summary>
	/// Author: Intuitive Gaming Solutions
    public class HandsAnimator : MonoBehaviour
    {
        #region Editor Serialized Fields
		[Header("Settings")]
		[Tooltip("Global Animator(s) that play both left and right thand animations.")]
		public Animator[] globalAnimators;
		[Tooltip("Left hand Animator(s) that play only left hand animations.")]
		public Animator[] leftAnimators;
		[Tooltip("Right hand Animator(s) that play only right hand animations.")]
		public Animator[] rightAnimators;

		[Header("Settings - Layers")]
		[Min(-1)]
		[Tooltip("The layer to play left hand animations on for left animators.")]
		public int leftLayerIndex = -1;
		[Min(-1)]
		[Tooltip("The layer to play right hand animations on for right animators.")]
		public int rightLayerIndex = -1;
		[Min(-1)]
		[Tooltip("The layer to play left hand animations on for global animators.")]
		public int globalLeftLayerIndex = -1;
		[Min(-1)]
		[Tooltip("The layer to play right hand animations on for global animators.")]
		public int globalRightLayerIndex = -1;
		#endregion

		#region Public Animation Method(s)
		/// <summary>Plays a 'left hand' animation by state name on all global/left hand animators.</summary>
		/// <param name="pState"></param>
		public void PlayLeftHand(string pState) { PlayLeftHand(pState, 0); }

		/// <summary>Plays a 'left hand' animation by state name on all global/left hand animators.</summary>
		/// <param name="pState"></param>
		/// <param name="pNormalizedTime">The normalized time to play the state at.</param>
		public void PlayLeftHand(string pState, float pNormalizedTime)
		{
			// Play on all global animator(s).
			if (globalAnimators != null && globalAnimators.Length > 0)
			{
				foreach (Animator globalAnimator in globalAnimators)
				{
					globalAnimator.Play(pState, globalLeftLayerIndex);
				}
			}

			// Play on all left hand animator(s).
			if (leftAnimators != null && leftAnimators.Length > 0)
            {
				foreach (Animator leftAnimator in leftAnimators)
                {
					leftAnimator.Play(pState, leftLayerIndex);
                }
            }
		}

		/// <summary>Plays a 'right hand' animation by state name on all global/right hand animators.</summary>
		/// <param name="pState"></param>
		public void PlayRightHand(string pState) { PlayRightHand(pState, 0); }

		/// <summary>Plays a 'right hand' animation by state name on all global/right hand animators.</summary>
		/// <param name="pState"></param>
		/// <param name="pNormalizedTime">The normalized time to play the state at.</param>
		public void PlayRightHand(string pState, float pNormalizedTime)
		{
			// Play on all global animator(s).
			if (globalAnimators != null && globalAnimators.Length > 0)
			{
				foreach (Animator globalAnimator in globalAnimators)
				{
					globalAnimator.Play(pState, globalRightLayerIndex);
				}
			}

			// Play on all right hand animator(s).
			if (rightAnimators != null && rightAnimators.Length > 0)
			{
				foreach (Animator rightAnimator in rightAnimators)
				{
					rightAnimator.Play(pState, rightLayerIndex);
				}
			}
		}

		/// <summary>Plays an animation by state name on both hands.</summary>
		/// <param name="pState"></param>
		public void PlayBothHands(string pState) { PlayBothHands(pState, 0); }

		/// <summary>Plays an animation by state name on both hands.</summary>
		/// <param name="pState"></param>
		/// <param name="pNormalizedTime">The normalized time to play the state at.</param>
		public void PlayBothHands(string pState, float pNormalizedTime)
        {
			PlayLeftHand(pState);
			PlayRightHand(pState);
        }
		#endregion
		#region Public Toggle Method(s)
		/// <summary>Enables all left hand animators.</summary>
		public void EnableLeftAnimators()
        {
			if (leftAnimators != null && leftAnimators.Length > 0)
			{
				foreach (Animator animator in leftAnimators)
				{
					animator.enabled = true;
				}
			}
        }

		/// <summary>Disables all left hand animators.</summary>
		public void DisableLeftAnimators()
        {
			if (leftAnimators != null && leftAnimators.Length > 0)
			{
				foreach (Animator animator in leftAnimators)
				{
					animator.enabled = false;
				}
			}
		}

		/// <summary>Enables all right hand animators.</summary>
		public void EnableRightAnimators()
		{
			if (rightAnimators != null && rightAnimators.Length > 0)
			{
				foreach (Animator animator in rightAnimators)
				{
					animator.enabled = true;
				}
			}
		}

		/// <summary>Disables all right hand animators.</summary>
		public void DisableRightAnimators()
		{
			if (rightAnimators != null && rightAnimators.Length > 0)
			{
				foreach (Animator animator in rightAnimators)
				{
					animator.enabled = false;
				}
			}
		}
		#endregion
	}
}
