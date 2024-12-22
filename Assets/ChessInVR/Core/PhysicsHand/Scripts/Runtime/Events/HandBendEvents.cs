using System;
using UnityEngine;
using UnityEngine.Events;
using AdaptiveHands;
using GrabSystem.Math;

namespace PhyiscsHand.Events
{
    /// <summary>
    /// A simple component that references a KinematicHand that can perform events based on average bend conditions.
    /// The first entry whose condition passes is used each frame. Repeats will only fire when a different 'entry' than the last fired one is used.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class HandBendEvents : MonoBehaviour
    {
        // HandBendUnityEvent.
        /// <summary>
        /// Arg0: HandBendEvents - The HandBendEvents compontent involved in the event.
        /// Arg1: HandBendEvents.Entry - The entry that is involved in the event.
        /// </summary>
        [Serializable]
        public class HandBendUnityEvent : UnityEvent<HandBendEvents, Entry> {}

        // Entry.
        [Serializable]
        public class Entry
        {
            [Tooltip("The comparator to use when testing average finger bend (lhs) against 'averageBend' (rhs).\n\nGT - Greater than.\nGTE - Greater than or equal to.\nLT - Less than.\nLTE - Less than or equal to.\nAPPRX - Approximately equal to.\nNAPPRX - Not approximately equal to.")]
            public FloatMath.Comparator comparator;
            [Range(0f, 1f)]
            [Tooltip("The average bend target for this condition.")]
            public float averageBend;
            [Tooltip("An event that is invoked when this entry is used by a grabber.\n\nArg0: Grabber - the Grabber that used the entry.")]
            public UnityEvent EntryUsed;

            // Internal method(s).
            /// <summary>Invokes the 'EntryUsed' event.</summary>
            internal void Internal_OnUsed() { EntryUsed?.Invoke(); }
        }

        // HandBendEvents.
        [Header("Settings")]
        [Tooltip("A reference to the KinematicHand used to check event conditions.")]
        public KinematicHand kinematicHand;
        [Tooltip("An array of HandBendEvents.Entrys that contain rules that decide which bend event entry (if any) to fire. Checked from first to last, first passed condition test is used each frame if it wasn't used last frame.")]
        public Entry[] entries;

        [Header("Events")]
        [Tooltip("Invoked whenever a HandBendEvents.Entry is 'used'/fired.\n\nArg0: HandBendEvents.Entry - the 'Entry' that was used.")]
        public HandBendUnityEvent BendEventInvoked;

        /// <summary>A reference to the last HandBendEvents.Entry that was fired.</summary>
        public Entry LastFiredEntry { get; private set; }

        // Unity callback(s0.
        void Reset()
        {
            // Look for 'KinematicHand' component if null.
            if (kinematicHand == null)
                kinematicHand = GetComponent<KinematicHand>();
        }

        void Update()
        {
            // Only run if a valid kinematicHand reference is set.
            if (kinematicHand != null)
            {
                // Check each entry in order.
                foreach (Entry entry in entries)
                {
                    // Compare average finger bend for hand to entry.averageBend using entry.comparator.
                    if (FloatMath.CompareFloat(kinematicHand.AverageFingerBend, entry.averageBend, entry.comparator))
                    {
                        // Float match! Use entry.
                        if (entry != LastFiredEntry)
                            entry.Internal_OnUsed();
                        LastFiredEntry = entry;

                        // Invoke the 'BendEventInvoked' Unity event.
                        BendEventInvoked?.Invoke(this, entry);
                        break;
                    }
                }
            }
        }
    }
}
