using UnityEditor;
using TypeReferences.Editor;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializePhysicsHandTypeReferences
	{
		// Constructor(s).
		static InitializePhysicsHandTypeReferences()
		{
			// Register relevant assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("PhysicsHand-Runtime"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("PhysicsHand-Runtime");
		}
	}
}