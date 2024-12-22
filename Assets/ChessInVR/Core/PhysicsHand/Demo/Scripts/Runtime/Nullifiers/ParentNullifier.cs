using UnityEngine;

namespace PhysicsHand.Demo.Nullifiers
{
    /// <summary>
    /// A component that provides a public method, SetParentToNull(), that can be invoked by Unity editor events.
    /// This component exists because using the Transform.SetTransform() method in the Unity editor does not support None.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ParentNullifier : MonoBehaviour
    {
        // Public method(s).
        /// <summary>
        /// A public method that invokes transform.SetParent(null) when executed.
        /// This exists for use with Unity editor events because using the built-in SetParent with the None reference in editor events causes a type conversion error.
        /// </summary>
        public void SetParentToNull()
        {
            transform.SetParent(null);
        }
    }
}
