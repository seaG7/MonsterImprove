using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.Demo.Invokers
{
    /// <summary>
    /// A component that invokes an event, Triggered, when the component's Trigger() method is invoked.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class Invoker : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked after this component's Trigger() method invoked.")]
        public UnityEvent Triggered;

        // Public method(s).
        /// <summary>
        /// Triggers the event associated with this component.
        /// </summary>
        public void Trigger()
        {
            // Invoke the 'Triggered' event.
            Triggered?.Invoke();
        }
    }
}
