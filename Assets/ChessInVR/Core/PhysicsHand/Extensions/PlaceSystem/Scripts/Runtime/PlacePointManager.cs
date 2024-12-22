using UnityEngine;
using System.Collections.Generic;
using GrabSystem;

namespace PhysicsHand.PlaceSystem
{
    /// <summary>
    /// A static class that implements efficient place point management methods such as:
    /// - The ability to determine which PlacePoint (if any) a GrabbableObject is in.
    /// - 
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public static class PlacePointManager
    {
        #region Private Static Field(s)
        /// <summary>A dictionary where the key is GrabbableObject and the value is the PlacePoint it is in, otherwise null or not in dictionary if not in a PlacePoint. </summary>
        static Dictionary<GrabbableObject, PlacePoint> m_GrabbableToPlacePointMap = new Dictionary<GrabbableObject, PlacePoint>();
        #endregion

        #region Public GrabbableObject Method(s)
        /// <summary>Returns the PlacePoint the GrabbableObject is in, otherwise null.</summary>
        /// <param name="pGrabbable"></param>
        /// <returns>the PlacePoint the GrabbableObject is in, otherwise null.</returns>
        public static PlacePoint GetPlacePoint(GrabbableObject pGrabbable)
        {
            // Attempt to return the place point that is holding pGrabbable.
            if (m_GrabbableToPlacePointMap.TryGetValue(pGrabbable, out PlacePoint placePoint))
                return placePoint;

            // Not in map, return null.
            return null;
        }
        #endregion

        #region Internal GrabbableObject Method(s)
        /// <summary>Sets the tracked PlacePoint that pGrabbable is placed into equal to pPlacePoint.</summary>
        /// <param name="pGrabbable"></param>
        /// <param name="pPlacePoint"></param>
        internal static void SetPlacePointForGrabbable(GrabbableObject pGrabbable, PlacePoint pPlacePoint)
        {
            m_GrabbableToPlacePointMap[pGrabbable] = pPlacePoint;
        }

        /// <summary>Removes the specified grabbable from placed grabbable tracking, this means no place point is holding it.</summary>
        /// <param name="pGrabbable"></param>
        internal static void SetGrabbableUnplaced(GrabbableObject pGrabbable)
        {
            m_GrabbableToPlacePointMap.Remove(pGrabbable);
        }
        #endregion
    }
}
