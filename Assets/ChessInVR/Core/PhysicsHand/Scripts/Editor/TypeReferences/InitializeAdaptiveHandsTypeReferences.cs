using UnityEditor;
using TypeReferences.Editor;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializeAdaptiveHandsTypeReferences
	{
		// Constructor(s).
		static InitializeAdaptiveHandsTypeReferences()
		{
			// Register relevant assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("AdaptiveHands-Runtime"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("AdaptiveHands-Runtime");
		}
	}
}