using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GrabSystem;
using PhysicsHand.PlaceSystem.Data;
using PhysicsHand.PlaceSystem.Events;

namespace PhysicsHand.PlaceSystem
{
    /// <summary>
    /// A component that converts a trigger into a place point that kinematically holds GrabbableObjects that are placed in it.
    /// 
    /// A GrabbableObject is placed into a PlacePoint when it or any of its GrabbableChildObjects enter the place points trigger and remain in it while not being held (assuming it meets any whitelist requirements).
    /// A GrabbableObject is taken from a PlacePoint when it is grabbed, if it is released before leaving the place points trigger it will be placed back into the place point.
    /// 
    /// NOTE: Kinematic Rigidbodies may not be placed (except for Rigidbodys that are kinematic due to the 'makeKinematic' property) if 'parentOnPlace' is not true.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PlacePoint : MonoBehaviour
    {
        #region Editor Serialized Settings
        [Header("Settings - General")]
        [Tooltip("Should the object be force placed in this place point immediately upon triggering the OnEnterPlacePoint callback.")]
        public bool forcePlace;
        [Tooltip("Should the PlacePoints GameObject be immediately deactivated once something is released from the place point?")]
        public bool deactivateOnRelease;
        [Tooltip("Can distance grabbables be distance grabbed from this place point?")]
        public bool enableDistanceGrab = true;
        [Min(0)]
        [Tooltip("The maximum number of items that can be placed in the place point at once.")]
        public int maxPlacedItems = 1;
        [Tooltip("Should the GrabbableObjects that a re placed in this placepoint be made its child?")]
        public bool parentOnPlace = true;

        [Header("Settings - Whitelists")]
        [Tooltip("If not empty only GrabbableObjects in this list (or that match other whitelists) will be accepted by this place point.")]
        public GrabbableObject[] whitelistGrabbables;
        [Tooltip("If not empty only GrabbableObjects whose gameObjects name matches a whitelist name (or that match other whitelists) will be accepted by this place point.")]
        public string[] whitelistNames;

        [Header("Settings - Blacklists")]
        [Tooltip("If not empty any GrabbableObjects in this list will be denied by this place point no matter what whitelists they belong to.")]
        public GrabbableObject[] blacklistGrabbables;
        [Tooltip("If not empty any GrabbableObjects whose gameObjects name matches a blacklist name will be denied by this place point no matter what whitelists they belong to.")]
        public string[] blacklistNames;

        [Header("Events")]
        [Tooltip("An event that is invoked when the place points Start() method is invoked.")]
        public UnityEvent Started;

        [Header("Events - Grab & Place")]
        [Tooltip("An event that is invoked whenever an item is placed into the place point.\n\nArg0: PlacePoint - the place point the item was placed into.\nArg1: GrabbableObject - The GrabbableObject that was placed into the place point.")]
        public PlacePointUnityEvent ItemPlaced;
        [Tooltip("An event that is invoked whenever an item is released from the place point.\n\nArg0: PlacePoint - the place point the item was grabbed from.\nArg1: GrabbableObject - The GrabbableObject that was placed in the place point.")]
        public PlacePointUnityEvent ItemReleased;

        [Header("Events - Trigger")]
        [Tooltip("An event that is invoked whenever an item enters this place points trigger. Note that this is only invoked if the GrabbableObject can be placed.\n\nArg0: PlacePoint - the place point whose trigger was entered by the item.\nArg1: GrabbableObject - The GrabbableObject that entered the place points trigger.")]
        public PlacePointUnityEvent ItemTriggerEntered;
        [Tooltip("An event that is invoked whenever an item exits this place points trigger. Note that this is only invoked if the GrabbableObject can be placed.\n\nArg0: PlacePoint - the place point whose trigger was exited by the item.\nArg1: GrabbableObject - The GrabbableObject that left the place points trigger.")]
        public PlacePointUnityEvent ItemTriggerExited;
        #endregion
        #region Public Properties
        /// <summary>Returns the number of items currently placed in this place point.</summary>
        public int PlacedItemCount { get { return m_PlacedItems.Count; } }
        /// <summary>Returns the number of items currently triggeirng this place point.</summary>
        public int TriggerItemCount { get { return m_TriggerItems.Count; } }
        #endregion
        #region Private Properties
        /// <summary>The list that tracks which GrabbableObjects are placed in this place point.</summary>
        List<PlacedItemEntry> m_PlacedItems = new List<PlacedItemEntry>();
        /// <summary>The list that tracks which GrabbableObjects are currently triggering this place point.</summary>
        List<GrabbableObject> m_TriggerItems = new List<GrabbableObject>();
        #endregion

        // Unity callback(s).
        #region Unity Callbacks
        void Start()
        {
            // Invoke the 'Started' Unity event.
            Started?.Invoke();
        }

        void OnTriggerEnter(Collider pOther)
        {
            // Check if triggered by a GrabbableObject or GrabbableChildObject.
            GrabbableObject triggeredBy = pOther.GetComponent<GrabbableObject>();
            if (triggeredBy == null)
            {
                GrabbableChildObject childObject = pOther.GetComponent<GrabbableChildObject>();
                if (childObject != null && childObject.grabbable != null)
                    triggeredBy = childObject.grabbable;
            }

            // If triggered by a GrabbableObject invoke the 'ItemTriggerEntered' callback.
            if (triggeredBy != null && CanPlace(triggeredBy))
            {
                // Always call StartTrigger(...) since it only starts a trigger if the relevant GrabbableObject is not already triggering the place point.
                StartTrigger(triggeredBy);
            }
        }

        void OnTriggerStay(Collider pOther)
        {
            // Check if triggered by a GrabbableObject or GrabbableChildObject.
            GrabbableObject triggeredBy = pOther.GetComponent<GrabbableObject>();
            if (triggeredBy == null)
            {
                GrabbableChildObject childObject = pOther.GetComponent<GrabbableChildObject>();
                if (childObject != null && childObject.grabbable != null)
                    triggeredBy = childObject.grabbable;
            }

            // If triggered by a GrabbableObject invoke then start triggering with triggeredBy.
            if (triggeredBy != null && CanPlace(triggeredBy))
            {
                // Always call StartTrigger(...) since it only starts a trigger if the relevant GrabbableObject is not already triggering the place point.
                StartTrigger(triggeredBy);
            }
        }

        void OnTriggerExit(Collider pOther)
        {
            // Check if triggered by a GrabbableObject or GrabbableChildObject.
            GrabbableObject triggeredBy = pOther.GetComponent<GrabbableObject>();
            if (triggeredBy == null)
            {
                GrabbableChildObject childObject = pOther.GetComponent<GrabbableChildObject>();
                if (childObject != null && childObject.grabbable != null)
                    triggeredBy = childObject.grabbable;
            }

            // If triggered by a GrabbableObject then stop trigger with triggeredBy.
            if (triggeredBy != null)
            {
                // Always call StopTrigger(...) since it only stops a trigger if the relevant GrabbableObject is already triggering the place point.
                StopTrigger(triggeredBy);
            }
        }
        #endregion

        // Public method(s).
        #region Trigger Method(s)
        /// <summary>Notifies the place point that a GrabbableObject entered (or stayed in) its trigger and can be placed.</summary>
        /// <param name="pGrabbable"></param>
        public void StartTrigger(GrabbableObject pGrabbable)
        {
            // Ensure the grabbable is not already triggering the place point.
            if (!m_TriggerItems.Contains(pGrabbable))
            {
                // Add the pGrabbable to the list of trigger items.
                m_TriggerItems.Add(pGrabbable);

                // Subscribe to pGrabbable released event.
                pGrabbable.Released.AddListener(OnTriggeringGrabbableReleased);

                // Invoke the 'ItemTriggerEntered' Unity event.
                ItemTriggerEntered?.Invoke(this, pGrabbable);

                // If 'force place' is enabled force a placement.
                if (forcePlace)
                    Place(pGrabbable);
            }
        }

        /// <summary>Notifies the place point that a GrabbableObject exited its trigger.</summary>
        /// <param name="pGrabbable"></param>
        public void StopTrigger(GrabbableObject pGrabbable)
        {
            // Only stop triggering if the pGrabbable is in the trigger items list.
            if (m_TriggerItems.Contains(pGrabbable))
            {
                // Remove the pGrabbable from the list of trigger items.
                m_TriggerItems.Remove(pGrabbable);

                // Unsubscribe from pGrabbable released event.
                pGrabbable.Released.RemoveListener(OnTriggeringGrabbableReleased);

                // Invoke the 'ItemTriggerExited' Unity event.
                ItemTriggerExited?.Invoke(this, pGrabbable);
            }
        }
        #endregion
        #region Place & Release Method(s)
        /// <summary>Places the GrabbableObject in the place point forcing any Grabber that is holding it to release it if 'force place' is enabled.</summary>
        /// <param name="pGrabbable"></param>
        public void Place(GrabbableObject pGrabbable)
        {
            // Determine if the grabbable was a kinematic Rigidbody before being placed.
            bool wasKinematic = pGrabbable.Rigidbody != null && pGrabbable.Rigidbody.isKinematic;

            // If the pGrabbable has a Rigidbody make it kinematic.
            if (pGrabbable.Rigidbody != null)
                pGrabbable.Rigidbody.isKinematic = true;

            // Add the pGrabbable to the list of placed items.
            m_PlacedItems.Add(new PlacedItemEntry() { grabbable = pGrabbable, wasKinematic = wasKinematic });

            // Subscribe to pGrabbable pre-grabbed event.
            pGrabbable.PreGrabbed.AddListener(OnPlacedGrabbablePreGrabbed);

            // If 'parentOnPlace' is enabled make pGrabbable.transform a child of transform.
            if (parentOnPlace)
                pGrabbable.transform.SetParent(transform, true);

            // If this grabbable is a distance grabbable then subscribe to the relevant event(s).
            DistanceGrabbable distanceGrabbable = pGrabbable.GetComponent<DistanceGrabbable>();
            if (distanceGrabbable != null)
            {
                distanceGrabbable.CanDistanceGrabDelegate += OnCanDistanceGrab;
                distanceGrabbable.DistanceGrabStarted.AddListener(OnDistanceGrabStarted);
            }

            // Track this placement in the place point manager.
            PlacePointManager.SetPlacePointForGrabbable(pGrabbable, this);

            // Invoke the 'ItemPlaced' Unity event.
            ItemPlaced?.Invoke(this, pGrabbable);
        }

        /// <summary>Releases the GrabbableObject from the PlacePoint unregistering it completely.</summary>
        /// <param name="pGrabbable"></param>
        public void Release(GrabbableObject pGrabbable)
        {
            // Find the relevant 'placed item entry'.
            int placedItemIndex = -1;
            for (int i = m_PlacedItems.Count - 1; i >= 0; --i)
            {
                if (m_PlacedItems[i].grabbable == pGrabbable)
                {
                    placedItemIndex = i;
                    break;
                }
            }

            // Only execute the enclosed code if the pGrabbable was found in the placed items list.
            if (placedItemIndex != -1)
            {
                // If the pGrabbable has a Rigidbody make it non-kinematic.
                if (pGrabbable.Rigidbody != null)
                    pGrabbable.Rigidbody.isKinematic = m_PlacedItems[placedItemIndex].wasKinematic;

                // Remove the pGrabbable from the list of placed items.
                m_PlacedItems.RemoveAt(placedItemIndex);
            }

            // Unsubscribe from pGrabbable pre-grabbed event.
            pGrabbable.PreGrabbed.RemoveListener(OnPlacedGrabbablePreGrabbed);

            // If this grabbable is a distance grabbable then subscribe to the relevant event(s).
            DistanceGrabbable distanceGrabbable = pGrabbable.GetComponent<DistanceGrabbable>();
            if (distanceGrabbable != null)
            {
                distanceGrabbable.CanDistanceGrabDelegate -= OnCanDistanceGrab;
                distanceGrabbable.DistanceGrabStarted.RemoveListener(OnDistanceGrabStarted);
            }

            // Track this release in the place point manager.
            PlacePointManager.SetGrabbableUnplaced(pGrabbable);

            // Invoke the 'ItemReleased' Unity event.
            ItemReleased?.Invoke(this, pGrabbable);

            // Disable the place points gameObject if set.
            if (deactivateOnRelease)
                gameObject.SetActive(false);
        }

        /// <summary>Releases all placed items.</summary>
        public void ReleaseAll()
        {
            for (int i = PlacedItemCount - 1; i >= 0; --i)
            {
                PlacedItemEntry placedEntry = m_PlacedItems[i];
                if (placedEntry.grabbable != null)
                {
                    // If 'parentOnPlace' is enabled unparent it.
                    if (parentOnPlace)
                        placedEntry.grabbable.transform.SetParent(null, true);

                    // Release the object from the place point.
                    Release(placedEntry.grabbable);
                }
            }
        }

        /// <summary>Returns true if the GrabbableObject, pGrabbable, may be placed in the place pint, otherwise false.</summary>
        /// <param name="pGrabbable"></param>
        /// <returns>true if pGrabbable may be placed in the place point, otherwise false.</returns>
        public bool CanPlace(GrabbableObject pGrabbable)
        {
            // Kinematic Rigidbodies may not be placed (except for Rigidbodys that are kinematic due to the 'makeKinematic' property) if 'parentOnPlace' is not true.
            if (!parentOnPlace && (pGrabbable.Rigidbody != null && pGrabbable.Rigidbody.isKinematic && !pGrabbable.makeKinematic))
                return false;

            // Cannot place when PlacedItemCount is greater than or equal to the maximum placed items.
            if (PlacedItemCount >= maxPlacedItems)
                return false;

            // Check GrabbableObject blacklist.
            if (blacklistGrabbables != null && blacklistGrabbables.Length > 0)
            {
                foreach (GrabbableObject blacklistedGrabbable in blacklistGrabbables)
                {
                    // If a match is found return false, pGrabbable is blacklisted.
                    if (pGrabbable == blacklistedGrabbable)
                        return false;
                }
            }

            // Check GrabbableObject gameObject name blacklist.
            if (blacklistNames != null && blacklistNames.Length > 0)
            {
                foreach (string blacklistedName in blacklistNames)
                {
                    // If a match is found return false, pGrabbable is blacklisted by name.
                    if (pGrabbable.gameObject.name == blacklistedName)
                        return false;
                }
            }

            // Track 'canPlace', by default we can place.
            bool canPlace = true;

            // Check for 'whitelistedGrabbables'.
            bool useGrabbablesWhitelist = whitelistGrabbables != null && whitelistGrabbables.Length > 0;
            if (useGrabbablesWhitelist)
            {
                // Check if the grabbable is in the whitelist.
                canPlace = false; // Cannot place unless found in whitelist.
                foreach (GrabbableObject whitelistedGrabbable in whitelistGrabbables)
                {
                    if (pGrabbable == whitelistedGrabbable)
                    {
                        canPlace = true;
                        break;
                    }
                }
            }

            // Check for 'whitelistNames' if the grabbable is not in the whitelist or the whitelist was not used.
            if (!canPlace || !useGrabbablesWhitelist)
            {
                bool useNameWhitelist = whitelistNames != null && whitelistNames.Length > 0;
                if (useNameWhitelist)
                {
                    // Check if the grabbable name is in the whitelist.
                    canPlace = false; // Cannot place unless found in whitelist.
                    foreach (string name in whitelistNames)
                    {
                        if (pGrabbable.gameObject.name == name)
                        {
                            canPlace = true;
                            break;
                        }
                    }
                }
                else
                {
                    // No whitelists were used, so we can place the grabbable.
                    canPlace = true;
                }
            }

            return canPlace;
        }
        #endregion
        #region Serialized Setting Method(s)
        /// <summary>Sets the 'forcePlace' field of this component. Useful for use with Unity Editor events.</summary>
        /// <param name="pEnabled"></param>
        public void SetForcePlaceEnabled(bool pEnabled) { forcePlace = pEnabled; }
        /// <summary>Sets the 'deactivateOnRelease' field of this component. Useful for use with Unity Editor events.</summary>
        /// <param name="pEnabled"></param>
        public void SetDeactivateOnRelease(bool pEnabled) { deactivateOnRelease = pEnabled; }
        /// <summary>Sets the 'enableDistanceGrab' field of this component. Useful for use with Unity Editor events.</summary>
        /// <param name="pEnabled"></param>
        public void SetDistanceGrabEnabled(bool pEnabled) { enableDistanceGrab = pEnabled; }
        /// <summary>Sets the 'maxPlacedItems' field of this component. Useful for use with Unity Editor events.</summary>
        /// <param name="pMax"></param>
        public void SetMaxPlacedItems(int pMax) { maxPlacedItems = pMax; }
        #endregion

        // Private callback(s).
        #region GrabbableObject Callbacks
        /// <summary>Invoked whenever a GrabbableObject that is currently placed in the place point is about to be grabbed.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnPlacedGrabbablePreGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // If 'parentOnPlace' is enabled unparent it.
            if (parentOnPlace)
                pGrabbable.transform.SetParent(null, true);

            // Release the object from the place point.
            Release(pGrabbable);
        }

        /// <summary>Invoked whenever a GrabbableObject that is currently trigger the place point is releaed.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnTriggeringGrabbableReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        { 
            // Check if there is nothing left holding the pGrabbable, if there is nothing then place it.
            if (pGrabbable.HeldByCount == 0)
            {
                // Place the grabbable in the place point.
                Place(pGrabbable);
            }
        }
        #endregion
        #region Distance Grabbable Callbacks
        /// <summary>Invoked before a distance grab starts on a placed distance grabbable that allows the actions permission to be overridden.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        /// <param name="pCanDistanceGrab"></param>
        void OnCanDistanceGrab(Grabber pGrabber, GrabbableObject pGrabbable, ref bool pCanDistanceGrab)
        {
            // Disallow distance grab if 'enableDistanceGrab' is false.
            if (!enableDistanceGrab)
                pCanDistanceGrab = false;
        }

        /// <summary>Invoked whenever a placed distance grabbable starts being distance grabbed.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnDistanceGrabStarted(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Release the grabbable from the place point when a distance grab starts.
            Release(pGrabbable);
        }
        #endregion
    }
}
