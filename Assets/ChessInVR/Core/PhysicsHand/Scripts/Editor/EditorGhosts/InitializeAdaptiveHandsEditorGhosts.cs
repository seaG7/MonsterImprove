using UnityEditor;
using EditorGhosts.Editor;
using AdaptiveHands;
using AdaptiveHands.Poser;
using AdaptiveHands.BendStates;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializeAdaptiveHandsEditorGhosts
	{
		// Constructor(s).
		static InitializeAdaptiveHandsEditorGhosts()
		{
			// Register relevant assemblies.
			if (!EditorGhostUtility.VisualComponentsWhitelist.Contains(typeof(KinematicHand)))
				EditorGhostUtility.VisualComponentsWhitelist.Add(typeof(KinematicHand));
			if (!EditorGhostUtility.VisualComponentsWhitelist.Contains(typeof(KinematicFinger)))
				EditorGhostUtility.VisualComponentsWhitelist.Add(typeof(KinematicFinger));
			if (!EditorGhostUtility.VisualComponentsWhitelist.Contains(typeof(HandPoser)))
				EditorGhostUtility.VisualComponentsWhitelist.Add(typeof(HandPoser));
			if (!EditorGhostUtility.VisualComponentsWhitelist.Contains(typeof(BendStateSwapper)))
				EditorGhostUtility.VisualComponentsWhitelist.Add(typeof(BendStateSwapper));
		}
	}
}