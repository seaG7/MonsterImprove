using System;
using UnityEngine;
using UnityEngine.Events;

namespace PhysicsHand.Demo.BasketballGame
{
    /// <summary>
    /// A component that is attached to basketball net trugger that detects when the ball goes in the hoop and keeps a count of baskets sunk.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class BasketballNet : MonoBehaviour
    {
        // BallSunkUnityEvent.
        /// <summary>
        /// Arg0: BasketballNet - The BasketballNet the ball was sunk on.
        /// Arg1: Basketball - The Basketball that was sunk.
        /// Arg2: int - The total number of basketballs sunk.
        /// </summary>
        [Serializable]
        public class BallSunkUnityEvent : UnityEvent<BasketballNet, Basketball, int> { }

        // GoalCountUnityEvent.
        /// <summary>
        /// Arg0: string - A string representing the number of goals sunk. (Useful for updating text.)
        /// </summary>
        [Serializable]
        public class GoalCountUnityEvent : UnityEvent<string> { }

        // BasketballNet.
        [Header("Events")]
        [Tooltip("An event that is invoked whenever a basketball is sunk on the net.\n\nArg0: BasketballNet - the net the ball was sunk on.\nArg1: Basketball - the basketball that was sunk.\nArg2: int - the total number of basketballs sunk.")]
        public BallSunkUnityEvent BasketballSunk;
        [Tooltip("Invoked whenever 'GoalCount' for this basketball net changes.\n\nArg0: int - the new goal count.")]
        public GoalCountUnityEvent GoalCountChanged;

        /// <summary>The number of basketballs that have been sunk in the net since the last reset.</summary>
        public int GoalCount
        {
            get { return m_GoalCount; }
            set
            {
                m_GoalCount = value;

                // Invoke the 'Goal Count Changed' Unity event.
                GoalCountChanged?.Invoke(m_GoalCount.ToString());
            }
        }

        /// <summary>The hidden backing field for the 'GoalCount' property.</summary>
        int m_GoalCount;

        // Unity callback(s).
        void OnTriggerEnter(Collider pOther)
        {
            Basketball basketball = pOther.GetComponent<Basketball>();
            if (basketball != null)
                OnBasketballSunk(basketball);
        }

        // Public method(s).
        /// <summary>Resets goal count to 0.</summary>
        public void ResetGoalCount()
        {
            GoalCount = 0;
        }

        // Private callback(s).
        /// <summary>Invoked whenever a basketball is sunk on the hoop.</summary>
        /// <param name="pBall"></param>
        void OnBasketballSunk(Basketball pBall)
        {
            // Increment goal count.
            ++GoalCount;

            // Invoke the 'BasketballSunk' Unity event.
            BasketballSunk?.Invoke(this, pBall, GoalCount);
        }
    }
}
