using System;
using UnityEngine.Events;
using GrabSystem;

namespace PhysicsHand.PlaceSystem.Events
{
    /// <summary>
    /// Arg0: PlacePoint - The place point involved in the event.
    /// Arg1: GrabbableObject - The GrabbableObject involved in the event.
    /// </summary>
    [Serializable]
    public class PlacePointUnityEvent : UnityEvent<PlacePoint, GrabbableObject> { }
}
