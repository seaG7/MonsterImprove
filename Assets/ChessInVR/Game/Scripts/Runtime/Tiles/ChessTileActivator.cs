using UnityEngine;
using ChessEngine.Game;

namespace ChessInVR
{
    /// <summary>
    /// A simple component that triggers ChessTilePieceTriggers.
    /// </summary>
    /// Author: Mathew Aloisio
    public class ChessTileActivator : MonoBehaviour
    {
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("A reference to the VisualChessPiece this activator is for.")]
        public VisualChessPiece visualPiece;
        #endregion

        #region Unity Callback(s)
        void Start() // NOTE: This method is included to force the 'enabled' checkbox to show in the editor.
        {
            if (visualPiece == null)
                visualPiece = GetComponentInParent<VisualChessPiece>();
            if (visualPiece == null)
                Debug.LogWarning("ChessTileActivator component is missing 'visualPiece' reference and none was found! Make sure to set this reference in your prefab/in the editor.", gameObject);
        }

        void Reset()
        {
            // Look for 'visualPiece' reference.
            if (visualPiece == null)
                visualPiece = GetComponentInParent<VisualChessPiece>();
        }
        #endregion
    }
}
