using UnityEngine;
using ChessEngine;
using ChessEngine.Game.AI;

namespace ChessInVR.Calibration
{
    public class AIChessPlayerCalibrator : IChessPlayerCalibrator
    {
        #region Editor Serialized Setting(s)
        [Header("Settings")]
        [Tooltip("The offset in local space relative to the 'referenceTransform'.")]
        public Vector3 offset;

        [Header("References")]
        [Tooltip("A reference to the ChessAIGameManager that calibration conditions are checked against.")]
        public ChessAIGameManager gameManager;
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
            if (gameManager == null)
                gameManager = FindObjectOfType<ChessAIGameManager>();
            if (gameManager == null)
                Debug.LogWarning("AIChessPlayerCalibrator could not find any ChessAIGameManager component in the scene, and no reference was set in the editor!", gameObject);
        }
        #endregion

        #region Public Overridden Calibration Callback(s)
        protected override void OnCalibrate()
        {
            Vector3 desiredPosition;
            if ((!gameManager.IsWhiteAIEnabled && !gameManager.IsBlackAIEnabled) || (gameManager.IsWhiteAIEnabled && gameManager.IsBlackAIEnabled))
            {
                if (gameManager.ChessInstance.turn == ChessColor.White)
                {
                    desiredPosition = whiteReferenceTransform.TransformPoint(offset);
                    positionTransform.forward = whiteReferenceTransform.forward;
                }
                else
                {
                    desiredPosition = blackReferenceTransform.TransformPoint(offset);
                    positionTransform.forward = blackReferenceTransform.forward;
                }
            }
            else
            {
                if (!gameManager.IsWhiteAIEnabled)
                {

                }
                else
                {
                    desiredPosition = blackReferenceTransform.TransformPoint(offset);
                    positionTransform.forward = blackReferenceTransform.forward;
                }
            }

        }
        #endregion
    }
}
