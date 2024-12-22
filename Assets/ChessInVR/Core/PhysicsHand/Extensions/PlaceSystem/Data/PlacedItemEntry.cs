using GrabSystem;

namespace PhysicsHand.PlaceSystem.Data
{
    /// <summary>
    /// A class that holds all data for a 'placed item' that is placed in a place point.
    /// </summary>
    /// Author: Mathew Aloisio
    public class PlacedItemEntry
    {
        /// <summary>A reference to the GrabbableObject that is placed in the point.</summary>
        public GrabbableObject grabbable;
        /// <summary>Was the item kinematic before it was placed?</summary>
        public bool wasKinematic;
    }
}
