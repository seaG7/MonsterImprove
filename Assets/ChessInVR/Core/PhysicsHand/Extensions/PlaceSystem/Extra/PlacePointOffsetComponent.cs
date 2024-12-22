using System;
using UnityEngine;
using GrabSystem;
using TypeReferences;
    
namespace PhysicsHand.PlaceSystem
{
    /// <summary>
    /// A component that can be added to an existing PlacePoint (or anything so long as it references a PlacePoint) that applies an offset
    /// to Grabbable(s) with atleast one of the given offsetComponents on them.
    /// 
    /// Similiar to PlacePointOffsetGrabbable except uses component types as a filter as opposed to references to the GrabbableObject component.
    /// This is intended to be used for situations where we may not have the ability to reference the Grabbable and isntead want to filter by component type.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PlacePointOffsetComponent : MonoBehaviour
    {

        // OffsetMode.
        [Serializable]
        public enum OffsetMode
        {
            /// <summary>Overrides the placed Grabbable's local euler angles or position with the ones from this component.</summary>
            OverrideLocal,
            /// <summary>Overrides the placed Grabbable's world euler angles or position with the ones from this component.</summary>
            OverrideWorld,
            /// <summary>Offsets the placed Grabbable's local euler angles or position with the ones from this component.</summary>
            OffsetLocal,
            /// <summary>Offsets the placed Grabbable's world euler angles or position with the ones from this component.</summary>
            OffsetWorld
        }

        // PlacePointOffsetComponent.
        [Header("Settings")]
        [Tooltip("The amount to offset the placed Grabbable's world space position by.")]
        public Vector3 positionOffset;
        [Tooltip("The mode to use when applying the position offset to the placed Grabbable..")]
        public OffsetMode positionOffsetMode = OffsetMode.OffsetLocal;
        [Tooltip("The euler angles offset to apply using the specified mode to the placed Grabbable.")]
        public Vector3 eulerAngles;
        [Tooltip("The mode to use when applying the euler angle offset to the placed Grabbable.")]
        public OffsetMode eulerAngleOffsetMode = OffsetMode.OverrideLocal;
        [Tooltip("An array of class type reference(s) that will be offset by this component when placed in the associated PlacePoint. (IF empty it will apply to all Grabbables.)")]
        [ClassExtends(typeof(MonoBehaviour))]
        public ClassTypeReference[] offsetComponents;

        [Header("References")]
        [Tooltip("The PlacePoint associated with this component. (If none is set GetComponent<PlacePoint>() will be used to attempt to find one.)")]
        [SerializeField] private PlacePoint m_PlacePoint;

        // Unity callback(s).
        void Awake()
        {
            // Look for a PlacePoint if none was referenced in the editor.
            if (m_PlacePoint == null)
            {
                m_PlacePoint = GetComponent<PlacePoint>();
                if (m_PlacePoint == null)
                    Debug.LogWarning("No 'PlacePoint' reference set or found for PlacePointOffsetGrabbable component of gameObject '" + gameObject.name + "'!", gameObject);
            }
        }

        void OnEnable()
        {
            // Subscribe to PlacePoint placed event.
            if (m_PlacePoint != null)
                m_PlacePoint.ItemPlaced.AddListener(OnGrabbablePlaced);
        }

        void OnDisable()
        {
            // Unsubscribe from PlacePoint placed event.
            if (m_PlacePoint != null)
                m_PlacePoint.ItemPlaced.RemoveListener(OnGrabbablePlaced);
        }

        // Private method(s).
        void OffsetGrabbable(GrabbableObject pGrabbable)
        {
            // Apply offset based on mode.
            switch (positionOffsetMode)
            {
                case OffsetMode.OverrideLocal:
                    pGrabbable.transform.localPosition = positionOffset;
                    break;
                case OffsetMode.OverrideWorld:
                    pGrabbable.transform.position = positionOffset;
                    break;
                case OffsetMode.OffsetLocal:
                    pGrabbable.transform.localPosition += positionOffset;
                    break;
                case OffsetMode.OffsetWorld:
                    pGrabbable.transform.position += positionOffset;
                    break;
                default:
                    Debug.LogWarning("Unhandled position offset mode found ('" + positionOffsetMode.ToString() + "') when offsetting Grabbable! [Component: PlacePointOffsetGrabbable | gameObject name: '" + gameObject.name + "']", gameObject);
                    break;
            }

            // Apply euler angle offset based on mode.
            switch (eulerAngleOffsetMode)
            {
                case OffsetMode.OverrideLocal:
                    pGrabbable.transform.localEulerAngles = eulerAngles;
                    break;
                case OffsetMode.OverrideWorld:
                    pGrabbable.transform.eulerAngles = eulerAngles;
                    break;
                case OffsetMode.OffsetLocal:
                    pGrabbable.transform.localEulerAngles += eulerAngles;
                    break;
                case OffsetMode.OffsetWorld:
                    pGrabbable.transform.eulerAngles += eulerAngles;
                    break;
                default:
                    Debug.LogWarning("Unhandled euler angle offset mode found ('" + positionOffsetMode.ToString() + "') when offsetting Grabbable! [Component: PlacePointOffsetGrabbable | gameObject name: '" + gameObject.name + "']", gameObject);
                    break;
            }
        }

        // Private callback(s).
        /// <summary>Invoked when a GrabbableObject, any GrabbableObject, is placed in the place point associated with this component.</summary>
        /// <param name="pPlacePoint"></param>
        /// <param name="pGrabbable"></param>
        void OnGrabbablePlaced(PlacePoint pPlacePoint, GrabbableObject pGrabbable)
        {
            // Check if no offset components are set, if so offset the grabbable.
            if (offsetComponents == null || offsetComponents.Length == 0)
            {
                OffsetGrabbable(pGrabbable);
            }
            else
            {
                // Look for a component that one of the ones from offsetComponents in the pGrabbable's gameObject.
                foreach (ClassTypeReference typeReference in offsetComponents)
                {
                    var behaviour = pGrabbable.GetComponent(typeReference.Type);
                    if (behaviour != null)
                    {
                        // Offset the grabbable and break out of the loop.
                        OffsetGrabbable(pGrabbable);
                        break;
                    }
                }
            }
        }
    }
}
