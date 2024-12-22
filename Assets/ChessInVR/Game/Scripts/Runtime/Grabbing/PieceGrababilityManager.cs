using UnityEngine;
using ChessEngine.Game;
using ChessEngine;
using GrabSystem;

namespace ChessInVR.Grabbing
{
    /// <summary>
    /// A component that is intended to be attached to the same GameObject as a ChessGameManager.
    /// This component:
    ///     - Disables the grabbing of pieces that belong to a team whose turn it is not.
    ///     - Disables grabability on all pieces except grabbed one.
    ///     - Forces pieces that should no longer be held to be released.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(ChessGameManager))]
    public class PieceGrababilityManager : MonoBehaviour
    {
        #region Public Properties
        /// <summary>A reference to the ChessGameManager driving piece grability.</summary>
        public ChessGameManager GameManager { get; private set; }
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            // Find 'game manager' reference.
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            // Subscribe to GameManager event(s).
            GameManager.PostGameReset.AddListener(OnPostGameReset);
            GameManager.TurnStarted.AddListener(OnTurnStarted);
            GameManager.TurnEnded.AddListener(OnTurnEnded);
        }

        void OnDisable()
        {
            // Unsubscribe from GameManager event(s).
            if (GameManager != null)
            {
                GameManager.PostGameReset.RemoveListener(OnPostGameReset);
                GameManager.TurnStarted.RemoveListener(OnTurnStarted);
                GameManager.TurnEnded.RemoveListener(OnTurnEnded);
            }
        }
        #endregion

        #region Public Grability Method(s)
        /// <summary>
        /// Iterates over all grabbable visual chess pieces and sets their grabability based on whether or not they belong to the team whose turn it currently is.
        /// Only enables grability for pieces with valid moves or attacks.
        /// </summary>
        /// <param name="pForceReleaseTurn">Should grabbable pieces who are on the team whose turn it currently is be force released?</param>
        /// <param name="pForceReleaseNotTurn">Should grabbable pieces who are on the team whose turn it is not be force released?</param>
        public void SetGrabilityByTurn(bool pForceReleaseTurn, bool pForceReleaseNotTurn)
        {
            // Enable grab only for pieces of the team whose turn it currently is.
            // Loop over all visual chess pieces, look for a GrabbableObject on them.
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    // Look for grabbable object.
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null) // As a fallback look for GrabbableChildObject.
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null) // Ensure the visual piece has a grabbable object (or grabbable child object) component.
                    {
                        // Disable grabbing for opposite team (whose turn it is not).
                        if (visualPiece.Piece.Color != GameManager.ChessInstance.turn)
                        {
                            grabbablePiece.grabEnabled = false;

                            // Force release for anything grabbing this piece (without throwing) if set. (NOT TURN)
                            if (pForceReleaseNotTurn && grabbablePiece.HeldByCount > 0)
                                grabbablePiece.ForceGrabbersReleaseNoThrow();
                        }
                        // Otherwise enable grabbing for the same team (whose turn it currently is).
                        else
                        {
                            // Only enable grability for pieces with valid moves and/or attacks.
                            var validAttacks = visualPiece.Piece.GetValidAttacks();
                            bool hasMovesOrAttacks = validAttacks.Count > 0; // Check for valid attacks.
                            if (!hasMovesOrAttacks)
                            {
                                // No valid attacks, check for valid moves.
                                var validMoves = visualPiece.Piece.GetValidMoves();
                                hasMovesOrAttacks = validMoves.Count > 0;
                            }
                            if (hasMovesOrAttacks)
                            {
                                grabbablePiece.grabEnabled = true;

                                // Force release for anything grabbing this piece (without throwing) if set. (TURN)
                                if (pForceReleaseTurn && grabbablePiece.HeldByCount > 0)
                                    grabbablePiece.ForceGrabbersReleaseNoThrow();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Iterates over all grabbable chess pieces and disables grability for all of them except for pGrabbable.</summary>
        /// <param name="pGrabbable"></param>
        /// <param name="pForceReleaseDisabled">Should a force release be executed on grabbable objects disabled by this function?</param>
        public void DisableGrabilityExcept(GrabbableObject pGrabbable, bool pForceReleaseDisabled)
        {
            // Loop over all visual chess pieces, look for a GrabbableObject on them.
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    // Look for grabbable object.
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null) // As a fallback look for GrabbableChildObject.
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null) // Ensure the visual piece has a grabbable object (or grabbable child object) component.
                    {
                        // Disable all grabbable pieces except for pGrabbable.
                        if (grabbablePiece != pGrabbable)
                        {
                            // Force release if set (with no throw).
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
        /// <summary>Invoked whenever a grabbable chess piece in the current game is grabbed.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnPieceGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Disable grability for all grabbable pieces except for pGrabbable.
            DisableGrabilityExcept(pGrabbable, true);
        }

        /// <summary>Invoked whenever a grabbable chess piece in the current game is released.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnPieceReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // On release set grability back to by-turn.
            SetGrabilityByTurn(true, true);
        }     
        #endregion
        #region Private GameManager Callback(s)
        /// <summary>Invoked when the relevant GameManager dispatches a 'post game reset' event.</summary>
        void OnPostGameReset()
        {
            // Set grability by turn.
            SetGrabilityByTurn(true, true);

            // Loop over all pieces and subscribe to grab events.
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    // Look for grabbable object.
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null) // As a fallback look for GrabbableChildObject.
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null) // Ensure the visual piece has a grabbable object (or grabbable child object) component.
                    {
                        grabbablePiece.Grabbed.AddListener(OnPieceGrabbed);
                        grabbablePiece.Released.AddListener(OnPieceReleased);
                    }
                }
            }
        }

        /// <summary>Invoked when the relevant GameManager dispatches a 'turn started' event.</summary>
        /// <param name="pTeam">The team whose turn started.</param>
        void OnTurnStarted(ChessColor pTeam)
        {
            // Set grability by turn.
            SetGrabilityByTurn(false, true);
        }

        /// <summary>Invoked when the relevant GameManager dispatches a 'turn ended' event.</summary>
        /// <param name="pTeam">The team whose turn ended.</param>
        /// <param name="pMoveInfo">The move the turn ended on.</param>
        void OnTurnEnded(ChessColor pTeam, MoveInfo pMoveInfo)
        {
            // Disable grab for all pieces at the end of a turn.
            // Loop over all visual chess pieces, look for a GrabbableObject on them.
            for (int i = 0; i < GameManager.visualTable.VisualPieceCount; ++i)
            {
                VisualChessPiece visualPiece = GameManager.visualTable.GetVisualPieceByIndex(i);
                if (visualPiece != null)
                {
                    // Look for grabbable object.
                    GrabbableObject grabbablePiece = visualPiece.GetComponent<GrabbableObject>();
                    if (grabbablePiece == null) // As a fallback look for GrabbableChildObject.
                    {
                        GrabbableChildObject grabbableChild = visualPiece.GetComponent<GrabbableChildObject>();
                        if (grabbableChild != null)
                            grabbablePiece = grabbableChild.grabbable;
                    }
                    if (grabbablePiece != null) // Ensure the visual piece has a grabbable object (or grabbable child object) component.
                    {
                        // Force release for anything grabbing this piece (without throwing).
                        if (grabbablePiece.HeldByCount > 0)
                            grabbablePiece.ForceGrabbersReleaseNoThrow();

                        // Disable any/all pieces.
                        grabbablePiece.grabEnabled = false;
                    }
                }
            }
        }
        #endregion
    }
}
