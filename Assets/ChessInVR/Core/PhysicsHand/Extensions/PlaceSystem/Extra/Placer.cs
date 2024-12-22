using UnityEngine;
using GrabSystem;

namespace PhysicsHand.PlaceSystem
{
    /// <summary>
    /// A component that exposes public method(s) for placing a GrabbableObject into a PlacePoint via a reference to its GameObject (or similar).
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class Placer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the place point that this Placer will place things into. (NOTE: If null the component will attempt to find one via GetComponent<PlacePoint>().)")]
        [SerializeField] PlacePoint m_PlacePoint;

        // Unity callback(s).
        void Awake()
        {
            // Attempt to find a place point if none is set.
            if (m_PlacePoint == null)
            {
                m_PlacePoint = GetComponent<PlacePoint>();

                // Log warning if still no place point reference after attempting to find one.
                if (m_PlacePoint == null)
                    Debug.LogWarning("No 'place point' referenced or found for Placer component on gameObject '" + gameObject.name + "'!", gameObject);
            }
        }

        // Public method(s).
        /// <summary>
        /// Attempts to place a GameObject in the PlacePoint referenced by this Placer.
        /// Will only place an object if it has a Grabbable component.
        /// </summary>
        /// <param name="pObject"></param>
        public void Place(GameObject pObject)
        {
            GrabbableObject grabbable = pObject.GetComponent<GrabbableObject>();
            if (grabbable != null)
                m_PlacePoint.Place(grabbable);
        }
    }
}
