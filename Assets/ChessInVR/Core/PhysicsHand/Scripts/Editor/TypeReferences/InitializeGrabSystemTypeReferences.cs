using UnityEditor;
using TypeReferences.Editor;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializeGrabSystemTypeReferences
	{
		// Constructor(s).
		static InitializeGrabSystemTypeReferences()
		{
			// Register relevant assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("GrabSystem-Runtime"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("GrabSystem-Runtime");
		}
	}
}