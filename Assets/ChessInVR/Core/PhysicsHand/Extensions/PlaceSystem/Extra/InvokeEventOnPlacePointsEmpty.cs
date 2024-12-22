using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.PlaceSystem.Invokers
{
    /// <summary>
    /// A component that invokes an event when all referenced place point(s) are empty (or no place points are referenced)s.
    /// NOTE: The component will disable itself once it invokes it's event, you must manually re-enable it if you want the event to fire again in the future.
    /// </summary>
    /// Author: Mathew Aloisio
    public class InvokeEventOnPlacePointsEmpty : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("An array refering all PlacePoints that need to be empty for this component to invoke it's event.")]
        public PlacePoint[] placePoints;

        [Header("Events")]
        [Tooltip("An event that is invoked when all referenced placePoints are empty (or no place points are referenced).")]
        public UnityEvent Triggered;

        // Unity callback(s).
        void Update()
        {
            // Return if any place points have a valid object in them.
            foreach (PlacePoint placePoint in placePoints)
            {
                if (placePoint.PlacedItemCount > 0)
                    return;
            }

            // If we've reached this point all place points have no 'placedObject' and therefore the event can be triggered.
            Triggered?.Invoke();

            // Disable this component after triggering the event.
            enabled = false;
        }
    }
}
