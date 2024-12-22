using UnityEngine;

namespace TimeSystem
{
    /// <summary>
    /// A simple component that can be added in any scene as a simple way to override the TimeManager.TIME_MODE setting.
    /// Alternatively you can modify the mode directly via code.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PhysicsHandTimeModeOverride : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The time retrieval mode to use.")]
        public TimeRetrievalMode timeRetrievalMode;

        // Unity callback(s).
        void Awake()
        {
            // Override the time retrieval mode.
            TimeManager.TIME_RETRIEVAL_MODE = timeRetrievalMode;
        }
    }
}
