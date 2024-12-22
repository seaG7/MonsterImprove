using UnityEditor;
using UnityEngine;
using TimeSystem;

namespace PhysicsHand.Editor.TimeSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PhysicsHandTimeModeOverride))]
    public class PhysicsHandTimeModeOverrideEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (!Application.isPlaying)
            {
                DrawDefaultInspector();
            }
            else { EditorGUILayout.HelpBox("You should not modify the time mode wihle the game is running!", MessageType.Info); }
        }
    }
}
