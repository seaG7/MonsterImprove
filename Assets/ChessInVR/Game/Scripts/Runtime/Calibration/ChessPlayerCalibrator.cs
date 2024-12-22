using UnityEngine;
using ChessEngine;
using ChessEngine.Game;

namespace ChessInVR.Calibration
{
    /// <summary>
    /// A component that allows the player's avatar to be calibrated.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class ChessPlayerCalibrator : IChessPlayerCalibrator
    {
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("The offset in local space relative to the 'referenceTransform'.")]
        public Vector3 offset;

        [Header("References")]
        [Tooltip("A reference to the ChessGameManager that calibration conditions are checked against.")]
        public ChessGameManager gameManager;
        [Tooltip("A reference to a Transform that is either equal to 'positionTransform' or a child of it whose offset is considered when positioning.")]
        public Transform relativeTransform;
        [Tooltip("The actual Transform whose position/rotation is modified.")]
        public Transform positionTransform;

        [Header("References - White")]
        [Tooltip("A reference to the Transform that the 'positionTransform' is being placed in reference to (relative to) when considering the white team.")]
        public Transform whiteReferenceTransform;

        [Header("References - Black")]
        [Tooltip("A reference to the Transform that the 'positionTransform' is being placed in reference to (relative to) when considering the black team.")]
        public Transform blackReferenceTransform;
        #endregion

        #region Unity Callback(s)
        protected virtual void Awake()
        {
            // Look for gameManager if null.
            if (gameManager == null)
                gameManager = FindObjectOfType<ChessGameManager>();
            if (gameManager == null)
                Debug.LogWarning("ChessPlayerCalibrator could not find any ChessGameManager component in the scene, and no reference was set in the editor!", gameObject);
        }
        #endregion

        #region Public Overridden Calibration Callback(s)
        /// <summary>Invoked when the calibrator is being calibrated.</summary>
        protected override void OnCalibrate()
        {
            // If it is white's turn calibrate to white pose.
            Vector3 desiredPosition;
            if (gameManager.ChessInstance.turn == ChessColor.White)
            {
                // Calibrate to white pose.
                desiredPosition = whiteReferenceTransform.TransformPoint(offset);
                positionTransform.forward = whiteReferenceTransform.forward;
            }
            // If it is black's turn calibrate to black pose.
            else
            {
                // Calibrate to black pose.
                desiredPosition = blackReferenceTransform.TransformPoint(offset);
                positionTransform.forward = blackReferenceTransform.forward;
            }

            // Move the entire 'positionTransform' so that the 'relativeTransform' is in the same position as the 'desiredPosition'.
            positionTransform.position += desiredPosition - relativeTransform.position;
        }
        #endregion
    }
}
