using UnityEngine;
using ChessEngine;
using ChessEngine.Game;
using GrabSystem;
using System.Collections.Generic;

namespace ChessInVR.Grabbing
{
    /// <summary>
    /// A component that is intended to be attached to the same GameObject as a Grabber.
    /// This component overrides a Grabbers conditions to implement chess rules and behaviour meaning:
    /// - Any piece that is not 'selectable' is not grabbable.
    /// - A released piece not in a valid tile trigger will return to it's current tile.
    /// - A released piece in a valid tile trigger will be moved to said tile (and/or attack said tile).
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(Grabber))]
    public class ChessGrabber : MonoBehaviour
    {
        #region Editor Serialized Setting(s)
        [Tooltip("A reference to the ChessGameManager to test rules against. (If null one will be attempted to be found at Awake().)")]
        public ChessGameManager gameManager;
        [Tooltip("(Optional) The 'off hand(s)', or other grabber(s). This ChessGrabber will be prohibited from grabbing visual pieces while any 'other grabbers' are grabbing one.")]
        public ChessGrabber[] otherGrabbers;
        #endregion
        #region Public Properties
        /// <summary>A reference to the Grabber whose rules are being driven by this component.</summary>
        public Grabber Grabber { get; private set; }
        /// <summary>A reference to the VisualChessPiece currently being grabbed by this chess grabber, otherwise null.</summary>
        public VisualChessPiece GrabbingVisualPiece { get; private set; }
        /// <summary>A reference to the ChessTilePieceTrigger being triggered by the piece being held by this chess grabber, otherwise null.</summary>
        public ChessTilePieceTrigger TriggeringTile { get; private set; }
        #endregion
        #region Private Field(s)
        /// <summary>A list of all ChessTilePieceTrigger components in the gameManager since the last 'post reset' event or call to CacheTriggerableTiles().</summary>
        List<ChessTilePieceTrigger> m_TriggerableTiles = new List<ChessTilePieceTrigger>();
        #endregion

        #region Unity Callback(s)
        void Awake()
        {
            // Find 'Grabber' reference.
            Grabber = GetComponent<Grabber>();

            // Find 'gameManager' if null.
            if (gameManager == null)
                gameManager = FindObjectOfType<ChessGameManager>();
            if (gameManager == null)
                Debug.LogWarning("No 'gameManager' specified in the editor or found for ChessGrabberRules component!", gameObject);
        }

        void OnEnable()
        {
            // Subscribe to Grabber event(s).
            Grabber.Grabbed.AddListener(OnGrabberGrabbed);
            Grabber.CanGrabDelegate += OnCanGrabDelegate;

            // Subscribe to gameManager event(s).
            gameManager.PostGameReset.AddListener(OnPostGameReset);
        }

        void OnDisable()
        {
            // Unsubscribe from Grabber event(s).
            if (Grabber != null)
            {
                Grabber.Grabbed.RemoveListener(OnGrabberGrabbed);
                Grabber.CanGrabDelegate -= OnCanGrabDelegate;
            }

            // Unsubscribe from gameManager event(s).
            if (gameManager != null)
            {
                gameManager.PostGameReset.RemoveListener(OnPostGameReset);
            }
        }
        #endregion

        #region Public Tile Trigger Method(s)
        /// <summary>Triggers a tile to mark it as the move/attack tile if some conditions are met (grabbing visual piece, etc).</summary>
        /// <param name="pTileTrigger"></param>
        public void TriggerTile(ChessTilePieceTrigger pTileTrigger)
        {
            // Tiles may only be triggered while a visual piece is being grabbed.
            if (GrabbingVisualPiece != null)
            {
                // Track the 'TriggeringTile'.
                TriggeringTile = pTileTrigger;
            }
        }

        /// <summary>Untriggers any currently triggered tile.</summary>
        public void UntriggerTile()
        {
            TriggeringTile = null;
        }

        /// <summary>Caches all triggerable tiles.</summary>
        public void CacheTriggerableTiles()
        {
            ChessTilePieceTrigger[] tileTriggers = FindObjectsByType<ChessTilePieceTrigger>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            m_TriggerableTiles.Clear();
            for (int i = 0; i < tileTriggers.Length; ++i)
            {
                // Only add tiles that belong to the relevant game manager.
                if (tileTriggers[i].visualTile != null && tileTriggers[i].visualTile.GameManager == gameManager)
                {
                    m_TriggerableTiles.Add(tileTriggers[i]);
                }
            }
        }

        /// <summary>
        /// Iterates over all triggerable tiles and enables ones that have a valid move or attack for the 'GrabbingVisualPiuece', disable the others.
        /// NOTE: This method is only invoked if 'GrabbingVisualPiece' is non-null.
        /// </summary>
        public void UpdateTriggerableTiles()
        {
            // We can only update triggerable tiles if there is a valid 'GrabbingVisualPiece', otherwise we cannot tell which tiles should be triggerable for the held chess piece.
            if (GrabbingVisualPiece != null)
            {
                // Cache all valid moves and attacks for the grabbing piece.
                var validAttacks = GrabbingVisualPiece.Piece.GetValidAttacks();
                var validMoves = GrabbingVisualPiece.Piece.GetValidMoves();

                // Iterate over valid attacks & valid moves, build a list of all tiles that may be triggered.
                HashSet<ChessTableTile> triggerableTiles = new HashSet<ChessTableTile>();
                triggerableTiles.Add(GrabbingVisualPiece.VisualTable.GetVisualTile(GrabbingVisualPiece.Piece.TileIndex).Tile); // Add the tile its currently on to enable cancelling moves.
                foreach (AttackInfo validAttack in validAttacks) { triggerableTiles.Add(validAttack.moveToTile); }
                foreach (ChessTableTile validMove in validMoves) { triggerableTiles.Add(validMove); }
                foreach (ChessTilePieceTrigger tileTrigger in m_TriggerableTiles)
                {
                    // Enable the tile trigger if the tile is the target of any valid attack or move.
                    if (triggerableTiles.Contains(tileTrigger.visualTile.Tile))
                    {
                        tileTrigger.enabled = true;
                    }
                    // Otherwise disable the tile trigger.
                    else { tileTrigger.enabled = false; }
                }
            }
        }
        #endregion

        #region Private Grab Callback(s)
        /// <summary>Invoked after a grabber grabs something.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnGrabberGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Check if the grabbable has a 'VisualChessPiece' component.
            VisualChessPiece visualPiece = pGrabbable.GetComponent<VisualChessPiece>();
            if (visualPiece != null)
            {
                // Since the grabbed object is a VisualChessPiece track it.
                GrabbingVisualPiece = visualPiece;

                // Select the piece in the relevnat game manager.
                gameManager.SelectPiece(visualPiece, visualPiece.VisualTable.GetVisualTile(visualPiece.Piece.TileIndex));

                // Update the trigger-able tiles.
                UpdateTriggerableTiles();

                // Subscribe to the released event for this piece.
                pGrabbable.Released.AddListener(OnVisualChessPieceReleased);
            }
        }

        /// <summary>Invoked after a grabbable VisualChessPiece that was being grabbed by this chess grabber is released.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        void OnVisualChessPieceReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            // Ensure a valid visual piece was being grabbed (in case it was destroyed).
            if (GrabbingVisualPiece != null)
            {
                // Only move the piece if a valid 'TriggeringTile' is triggered.
                if (TriggeringTile != null)
                {
                    // Select the triggering tile.
                    gameManager.SelectTile(TriggeringTile.visualTile);

                    // Stop triggering tile.
                    TriggeringTile.UntriggerTile(this);
                }

                // Stop selecting anything.
                gameManager.Deselect();

                // Update the piece's position once more to ensure it is in the tile that was moved to (or left on).
                GrabbingVisualPiece.UpdatePosition();

                // Reset the piece's rotation.
                GrabbingVisualPiece.ResetRotation();
            }

            // No longer grabbing any visual piece or triggering any tile.
            GrabbingVisualPiece = null;
            TriggeringTile = null;

            // Unsubscribe from the released event for this grabbable piece.
            pGrabbable.Released.RemoveListener(OnVisualChessPieceReleased);
        }

        /// <summary>Overrides 'can grab' rules for a chess grabber.</summary>
        /// <param name="pGrabber"></param>
        /// <param name="pGrabbable"></param>
        /// <param name="pCanGrab"></param>
        void OnCanGrabDelegate(Grabber pGrabber, GrabbableObject pGrabbable, ref bool pCanGrab)
        {
            // If a grab is currently allowed check if it should be disallowed.
            if (pCanGrab)
            {
                // Check if the grabbable has a 'VisualChessPiece' component.
                VisualChessPiece visualPiece = pGrabbable.GetComponent<VisualChessPiece>();
                if (visualPiece != null)
                {
                    // This piece can only be grabbed if it is selectable.
                    pCanGrab = gameManager.CanSelectPiece(visualPiece, gameManager.visualTable.GetVisualTile(visualPiece.Piece.TileIndex));
                }
            }

            // If a grab is still allowed check if it is overrided by 'other grabbers'.
            if (otherGrabbers != null)
            {
                // Iterate over each 'other grabber', if one is holding a visual piece then grabbing is disabled for this chess grabber.
                foreach (ChessGrabber otherGrabber in otherGrabbers)
                {
                    if (otherGrabber.GrabbingVisualPiece != null)
                    {
                        pCanGrab = false;
                        break;
                    }
                }
            }
        }
        #endregion
        #region Private GameManager Callback(s)
        /// <summary>Invoked just after the gameManager's game is reset.</summary>
        void OnPostGameReset()
        {
            // Cache all 'ChessTilePieceTrigger' components in the gameManager table's tiles.
            CacheTriggerableTiles();
        }
        #endregion
    }
}
