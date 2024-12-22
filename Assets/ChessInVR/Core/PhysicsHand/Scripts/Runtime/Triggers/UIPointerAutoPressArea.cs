using UnityEngine;
using System.Collections.Generic;
using PhysicsHand.UI;
using PhyiscsHand.Events;

namespace PhysicsHand.Triggers
{
    /// <summary>
    /// A component that allows for a UIPointers that enter the trigger to have their 'auto click distance' overridden while in the area.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class UIPointerAutoPressArea : MonoBehaviour
    {
        // Entry.
        public class Entry
        {
            /// <summary>A reference to the RigidbodyHandUIPointer associated with this entry.</summary>
            public RigidbodyHandUIPointer pointer;
            /// <summary>The cached auto click distance from when the pointer entered the area.</summary>
            public float cachedDistance;
        }

        // UIPointerAutoClickArea.
        [Header("Settings")]
        [Tooltip("If not found on the triggering Collider's gameObject should this component check any 'attachedRigidbody' for the HandPoser?")]
        public bool checkRigidbody;
        [Tooltip("Restore the original 'auto press UI distance' of pointers that leave the area?")]
        public bool restoreDistance = true;
        [Tooltip("The auto press distance to use for UI pointers in the area.")]
        public float autoPressDistance = 0.13f;

        /// <summary>A list of all Entrys that are in the 'area' trigger.</summary>
        List<Entry> m_PointersInArea = new List<Entry>();

        // Unity callback(s).
        void OnDisable()
        {
            // When the component is disabled clear all 'pointers in area'.
            for (int i = 0; i < m_PointersInArea.Count; ++i)
            {
                // Check for match.
                OnPointerExitedArea(m_PointersInArea[i]);
            }

            // Clear 'pointers in area' list.
            m_PointersInArea.Clear();
        }

        void OnTriggerEnter(Collider pOther)
        {
            // Check if the 'other' Collider has a 'RigidbodyHandUIPointer' component.
            RigidbodyHandUIPointer pointer = pOther.GetComponent<RigidbodyHandUIPointer>();
            if (pointer == null & pOther.attachedRigidbody != null && checkRigidbody)
                pointer = pOther.attachedRigidbody.GetComponent<RigidbodyHandUIPointer>();
            if (pointer != null)
            {
                // Ensure the 'pointer' is not already in the 'pointers in area' list.
                if (!IsPointerInArea(pointer))
                {
                    OnPointerEnteredArea(pointer);
                }
            }
        }

        void OnTriggerExit(Collider pOther)
        {
            // Check if the 'other' Collider has a 'RigidbodyHandUIPointer' component.
            RigidbodyHandUIPointer pointer = pOther.GetComponent<RigidbodyHandUIPointer>();
            if (pointer == null & pOther.attachedRigidbody != null && checkRigidbody)
                pointer = pOther.attachedRigidbody.GetComponent<RigidbodyHandUIPointer>();
            if (pointer != null)
            {
                for (int i = 0; i < m_PointersInArea.Count; ++i)
                {
                    // Check for match.
                    if (pointer == m_PointersInArea[i].pointer)
                    {
                        // Pointer exited the area.
                        OnPointerExitedArea(m_PointersInArea[i]);

                        // Unregister the pointer from the list.
                        m_PointersInArea.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        // Public method(s).
        /// <summary>Returns true if pPointer is in the area, otherwise false.</summary>
        /// <param name="pPointer"></param>
        /// <returns>true if pPointer is in the area, otherwise false.</returns>
        public bool IsPointerInArea(RigidbodyHandUIPointer pPointer)
        {
            // Look for pPointer in the m_PointersInArea array.
            for (int i = 0; i < m_PointersInArea.Count; ++i)
            {
                if (m_PointersInArea[i].pointer == pPointer)
                    return true;
            }

            // Not in area, return false.
            return false;
        }

        // Private callback(s).
        /// <summary>Invoked whenever a RigidbodyHandUIPointer enters the area trigger.</summary>
        /// <param name="pPointer"></param>
        void OnPointerEnteredArea(RigidbodyHandUIPointer pPointer)
        {
            // Look for 'HandBendEvents' component, if found subscribe.
            HandBendEvents handBendEvents = pPointer.GetComponent<HandBendEvents>();
            if (handBendEvents != null)
                handBendEvents.BendEventInvoked.AddListener(OnHandBendEventInvoked);

            // Register the pointer in the list.
            m_PointersInArea.Add(new Entry() { pointer = pPointer, cachedDistance = pPointer.autoPressUIDistance });

            // Override pointer 'auto press UI distance'.
            pPointer.autoPressUIDistance = autoPressDistance;
        }

        /// <summary>Invoked whenever a RigidbodyHandUIPointer exits the area trigger.</summary>
        /// <param name="pEntry"></param>
        void OnPointerExitedArea(Entry pEntry)
        {
            // Ensure 'pEntry.pointer' is valid.
            if (pEntry.pointer != null)
            {
                // Look for 'HandBendEvents' component, if found unsubscribe.
                HandBendEvents handBendEvents = pEntry.pointer.GetComponent<HandBendEvents>();
                if (handBendEvents != null)
                    handBendEvents.BendEventInvoked.RemoveListener(OnHandBendEventInvoked);

                // Restore pointer 'auto press UI distance'.
                if (restoreDistance)
                    pEntry.pointer.autoPressUIDistance = pEntry.cachedDistance;
            }
        }

        /// <summary>Invoked whenever some HandBendEvents entry is used/fired by a HandBendEvents component on the UI pointer gameObject.</summary>
        void OnHandBendEventInvoked(HandBendEvents pComponent, HandBendEvents.Entry pEntry)
        {
            // Look for RigidbodyHandUIPointer, if found override 'auto press UI distance'.
            RigidbodyHandUIPointer pointer = pComponent.GetComponent<RigidbodyHandUIPointer>();
            if (pointer != null)
                pointer.autoPressUIDistance = autoPressDistance;
        }
    }
}
