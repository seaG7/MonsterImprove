using UnityEditor;
using UnityEngine;

namespace PhyiscsHand.Editor.Setup
{
    [InitializeOnLoad]
    public static class SetupHandLayers
    {
        /// <summary>The key name for the editor preference that tracks whether or not the 'hand layer wizard' has been ran on the current project.</summary>
        public static string HAS_RUN_WIZARD_KEY
        {
            get { return $"MyProject.{PlayerSettings.productGUID}.HasRunHandLayerWizard"; }
        }

        static SetupHandLayers()
        {
            int handLayerIndex = -1;

            // Check all layers for a 'hand' layer (case insensitive).
            for (int i = 0; i < 32; ++i)
            {
                if (LayerMask.LayerToName(i).ToLower() == "hand")
                {
                    handLayerIndex = i;
                    break;
                }
            }

            bool hasRunWizard = handLayerIndex != -1;
            if (!hasRunWizard)
                hasRunWizard = EditorPrefs.GetBool(HAS_RUN_WIZARD_KEY, false);
            if (!hasRunWizard)
            {
                int result = EditorUtility.DisplayDialogComplex(
                    "Hand Layer Setup Wizard",
                    "This project requires a 'Hand' layer. Do you want to create it now?",
                    "Yes",
                    "No",
                    "Don't ask again"
                );

                if (result == 0)
                {
                    if (handLayerIndex == -1)
                    {
                        // If no 'hand' layer is found, find the first empty layer index.
                        for (int i = 31; i >= 0; --i)
                        {
                            if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                            {
                                handLayerIndex = i;
                                break;
                            }
                        }

                        // If no empty layer is found, set handLayerIndex to -1 to indicate failure.
                        if (handLayerIndex == -1)
                        {
                            Debug.LogError("SetupHandLayers: Failed to create 'Hand' layer - all layer slots are full.");
                            return;
                        }

                        // Create the 'Hand' layer in the first empty layer index found.
                        SerializedObject tagManager = new SerializedObject(
                            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                        SerializedProperty layers = tagManager.FindProperty("layers");
                        layers.GetArrayElementAtIndex(handLayerIndex).stringValue = "Hand";
                        tagManager.ApplyModifiedProperties();

                        Debug.Log("SetupHandLayers: 'Hand' layer created in layer index " + handLayerIndex + "!");

                        // Set the HAS_RUN_WIZARD_KEY to true
                        EditorPrefs.SetBool(HAS_RUN_WIZARD_KEY, true);
                    }
                }
                else if (result == 2)
                {
                    // Set the HAS_RUN_WIZARD_KEY to true
                    EditorPrefs.SetBool(HAS_RUN_WIZARD_KEY, true);
                }
            }
        }
    }
}