using UnityEditor;
using TypeReferences.Editor;

namespace PhysicsHand.Editor
{
	[InitializeOnLoad]
	public static class InitializePhysicsToolsTypeReferences
	{
		// Constructor(s).
		static InitializePhysicsToolsTypeReferences()
		{
			// Register relevant assemblies.
			if (!ClassTypeReferenceEditorSettings.IncludeAssemblies.Contains("PhysicsTools-Runtime"))
				ClassTypeReferenceEditorSettings.IncludeAssemblies.Add("PhysicsTools-Runtime");
		}
	}
}