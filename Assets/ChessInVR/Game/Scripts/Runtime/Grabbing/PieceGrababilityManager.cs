using UnityEngine;
using ChessEngine.Game;
using ChessEngine;
using GrabSystem;

namespace ChessInVR.Grabbing
{
    [RequireComponent(typeof(ChessGameManager))]
    public class PieceGrababilityManager : MonoBehaviour
    {
        #region Public Properties
        public ChessGameManager GameManager { get; private set; }
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            GameManager.PostGameReset.AddListener(OnPostGameReset);
            GameManager.TurnStarted.AddListener(OnTurnStarted);
            GameManager.TurnEnded.AddListener(OnTurnEnded);
        }

        void OnDisable()
        {
            if (GameManager != null)
            {
                GameManager.PostGameReset.RemoveListener(OnPostGameReset);
                GameManager.TurnStarted.RemoveListener(OnTurnStarted);
                GameManager.TurnEnded.RemoveListener(OnTurnEnded);
            }
        }
        #endregion

        #region Public Grability Method(s)
        public void SetGrabilityByTurn(bool pForceReleaseTurn, bool pForceReleaseNotTurn)
        {
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null)
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null)
                    {
                        if (visualPiece.Piece.Color != GameManager.ChessInstance.turn)
                        {
                            grabbablePiece.grabEnabled = false;

                            if (pForceReleaseNotTurn && grabbablePiece.HeldByCount > 0)
                                grabbablePiece.ForceGrabbersReleaseNoThrow();
                        }
                        else
                        {
                            var validAttacks = visualPiece.Piece.GetValidAttacks();
                            bool hasMovesOrAttacks = validAttacks.Count > 0;
                            if (!hasMovesOrAttacks)
                            {
                                var validMoves = visualPiece.Piece.GetValidMoves();
                                hasMovesOrAttacks = validMoves.Count > 0;
                            }
                            if (hasMovesOrAttacks)
                            {
                                grabbablePiece.grabEnabled = true;

                                if (pForceReleaseTurn && grabbablePiece.HeldByCount > 0)
                                    grabbablePiece.ForceGrabbersReleaseNoThrow();
                            }
                        }
                    }
                }
            }
        }

        public void DisableGrabilityExcept(GrabbableObject pGrabbable, bool pForceReleaseDisabled)
        {
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null)
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null)
                    {
                        if (grabbablePiece != pGrabbable)
                        {
                            if (pForceReleaseDisabled && grabbablePiece.HeldByCount > 0)
                                grabbablePiece.ForceGrabbersReleaseNoThrow();

                            grabbablePiece.grabEnabled = false;   
                        }
                    }
                }
            }
        }
        #endregion

        #region Private Grab Callback(s)
        void OnPieceGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            DisableGrabilityExcept(pGrabbable, true);
        }

        void OnPieceReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            SetGrabilityByTurn(true, true);
        }     
        #endregion
        #region Private GameManager Callback(s)
        void OnPostGameReset()
        {
            SetGrabilityByTurn(true, true);

            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null)
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null)
                    {
                        grabbablePiece.Grabbed.AddListener(OnPieceGrabbed);
                        grabbablePiece.Released.AddListener(OnPieceReleased);
                    }
                }
            }
        }

        void OnTurnStarted(ChessColor pTeam)
        {
            SetGrabilityByTurn(false, true);
        }

        void OnTurnEnded(ChessColor pTeam, MoveInfo pMoveInfo)
        {

            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null)
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null)
                    {
                        if (grabbablePiece.HeldByCount > 0)
                            grabbablePiece.ForceGrabbersReleaseNoThrow();

                        grabbablePiece.grabEnabled = false;
                    }
                }
            }
        }
        #endregion
    }
}
