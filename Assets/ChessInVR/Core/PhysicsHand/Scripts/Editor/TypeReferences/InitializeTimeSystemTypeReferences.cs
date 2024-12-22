using UnityEditor;
using TypeReferences.Editor;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializeTimeSystemTypeReferences
	{
		// Constructor(s).
		static InitializeTimeSystemTypeReferences()
		{
			// Register relevant assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("TimeSystem-Runtime"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("TimeSystem-Runtime");
		}
	}
}