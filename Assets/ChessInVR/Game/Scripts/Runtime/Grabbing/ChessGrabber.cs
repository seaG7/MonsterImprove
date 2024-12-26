using UnityEngine;
using ChessEngine;
using ChessEngine.Game;
using GrabSystem;
using System.Collections.Generic;

namespace ChessInVR.Grabbing
{
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
        public void TriggerTile(ChessTilePieceTrigger pTileTrigger)
        {
            if (GrabbingVisualPiece != null)
            {
                TriggeringTile = pTileTrigger;
            }
        }

        public void UntriggerTile()
        {
            TriggeringTile = null;
        }

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

        public void UpdateTriggerableTiles()
        {
        
            if (GrabbingVisualPiece != null)
            {
                var validAttacks = GrabbingVisualPiece.Piece.GetValidAttacks();
                var validMoves = GrabbingVisualPiece.Piece.GetValidMoves();

                HashSet<ChessTableTile> triggerableTiles = new HashSet<ChessTableTile>();
                triggerableTiles.Add(GrabbingVisualPiece.VisualTable.GetVisualTile(GrabbingVisualPiece.Piece.TileIndex).Tile); 
                foreach (AttackInfo validAttack in validAttacks) { triggerableTiles.Add(validAttack.moveToTile); }
                foreach (ChessTableTile validMove in validMoves) { triggerableTiles.Add(validMove); }
                foreach (ChessTilePieceTrigger tileTrigger in m_TriggerableTiles)
                {
                    if (triggerableTiles.Contains(tileTrigger.visualTile.Tile))
                    {
                        tileTrigger.enabled = true;
                    }
                    else { tileTrigger.enabled = false; }
                }
            }
        }
        #endregion

        #region Private Grab Callback(s)
        void OnGrabberGrabbed(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            VisualChessPiece visualPiece = pGrabbable.GetComponent<VisualChessPiece>();
            if (visualPiece != null)
            {
                GrabbingVisualPiece = visualPiece;

                gameManager.SelectPiece(visualPiece, visualPiece.VisualTable.GetVisualTile(visualPiece.Piece.TileIndex));

                UpdateTriggerableTiles();

                pGrabbable.Released.AddListener(OnVisualChessPieceReleased);
            }
        }
        void OnVisualChessPieceReleased(Grabber pGrabber, GrabbableObject pGrabbable)
        {
            if (GrabbingVisualPiece != null)
            {
                if (TriggeringTile != null)
                {
                    gameManager.SelectTile(TriggeringTile.visualTile);

                    TriggeringTile.UntriggerTile(this);
                }

                gameManager.Deselect();

                GrabbingVisualPiece.UpdatePosition();

                GrabbingVisualPiece.ResetRotation();
            }

            GrabbingVisualPiece = null;
            TriggeringTile = null;

            pGrabbable.Released.RemoveListener(OnVisualChessPieceReleased);
        }

        void OnCanGrabDelegate(Grabber pGrabber, GrabbableObject pGrabbable, ref bool pCanGrab)
        {
            if (pCanGrab)
            {
                VisualChessPiece visualPiece = pGrabbable.GetComponent<VisualChessPiece>();
                if (visualPiece != null)
                {
                    pCanGrab = gameManager.CanSelectPiece(visualPiece, gameManager.visualTable.GetVisualTile(visualPiece.Piece.TileIndex));
                }
            }

            if (otherGrabbers != null)
            {
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
