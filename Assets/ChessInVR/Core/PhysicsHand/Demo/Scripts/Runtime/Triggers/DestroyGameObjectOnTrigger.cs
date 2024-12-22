using UnityEngine;

namespace PhysicsHand.Demo.Triggers
{
	/// <summary>
	/// Destroys any GameObject that enters the trigger.
	/// </summary>
	/// Author: Intuitive Gaming Solutions
	public class DestroyGameObjectOnTrigger : MonoBehaviour
	{
		[Header("Settings")]
		[Tooltip("An array of Transforms that are ignored. (Child objects are ignored too.)")]
		public Transform[] blacklist;
		
		// Unity callback(s).
		void OnTriggerEnter(Collider pCollider)
		{
			// Check if the collider is blacklisted.
			bool blacklisted = false;
			foreach (Transform blacklistedTransform in blacklist)
			{
				// If pCollider.transform is equal to a blacklisted Transform or is a child of a blacklisted Transform it is blacklisted.
				if (pCollider.transform == blacklistedTransform || pCollider.transform.IsChildOf(blacklistedTransform))
				{
					blacklisted = true;
					break;
				}
			}

			// Check if a Rigidbody fell.
			if (pCollider.attachedRigidbody != null && !blacklisted)
            {
				foreach (Transform blacklistedTransform in blacklist)
                {
					// If pCollider.attachedRigidbody.transform is equal to a blacklisted Transform or is a child of a blacklisted Transform it is blacklisted.
					if (pCollider.attachedRigidbody.transform == blacklistedTransform || pCollider.attachedRigidbody.transform.IsChildOf(blacklistedTransform))
                    {
						blacklisted = true;
						break;
                    }
				}

				// Destroy the Rigidbody instead of the collider if not blacklisted.
				if (!blacklisted)
                {
					Destroy(pCollider.attachedRigidbody.gameObject);
					return;
                }
            }

			// If not blacklisted destroy the object.
			if (!blacklisted)
				Destroy(pCollider.gameObject);
		}
	}
}