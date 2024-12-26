using UnityEngine;
using UnityEngine.Events;
using System;

namespace ChessEngine.Game
{
    public class VisualChessPiece : MonoBehaviour
    {
        public class MoveUnityEvent : UnityEvent<MoveInfo> { }

        #region Editor Serialized Fields & Events
        [Header("Settings - Positioning")]
        [Tooltip("An offset to apply to this pieces' position.")]
        public Vector3 offset;

        [Header("Settings - Materials")]
        [Tooltip("(Optional) An editor set reference to the Renderer for this chess piece.")]
        [SerializeField] Renderer m_RendererOverride;
        [Tooltip("An editor set reference to the material of this piece when it's on the white team.")]
        [SerializeField] Material m_WhiteMaterial = null;
        [Tooltip("An editor set reference to the material of this piece when it's on the black team.")]
        [SerializeField] Material m_BlackMaterial = null;

        [Header("Events")]
        [Tooltip("An event that is invoked when this chess piece is moved.\n\nArg0: MoveInfo - The MoveInfo about the move.")]
        public MoveUnityEvent Moved;
        [Tooltip("An event that is invoked when this chess piece is captured.\n\nArg0: MoveInfo - The MoveInfo about the move the piece was captured on.")]
        public MoveUnityEvent Captured;
        [Tooltip("An event that is invoked when the visual piece is initialized.")]
        public UnityEvent Initialized;
        [Tooltip("An event that is invoked just before the visual piece is destroyed.")]
        public UnityEvent Destroyed;
        #endregion

        #region Public Properties
        public ChessPiece Piece { get; private set; }
        public VisualChessTable VisualTable { get; private set; }
        public Renderer Renderer { get; private set; }
        public Quaternion DefaultLocalRotation { get; set; }
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            // Find Renderer refrence if no override set.
            if (m_RendererOverride == null)
            {
                Renderer = GetComponentInChildren<Renderer>();
            }
            else { Renderer = m_RendererOverride; }
        }

        void Start()
        {
            // Store default local rotation.
            DefaultLocalRotation = transform.localRotation;
        }

        void OnDestroy()
        {
            // Unsubscribe from piece events.
            UnsubscribeFromPieceEvents();

            // Invoke the 'Destroyed' Unity event.
            Destroyed?.Invoke();
        }
        #endregion

        #region Initialization
        public void Initialize(VisualChessTable pTable, ChessPiece pPiece)
        {
            VisualTable = pTable;
            Piece = pPiece;

            if (Renderer != null)
            {
                if (Piece.Color == ChessColor.White)
                {
                    Renderer.material = m_WhiteMaterial;
                }
                else { Renderer.material = m_BlackMaterial; }
            }
            UpdatePosition();

            SubscribeToPieceEvents();

            Initialized?.Invoke();
        }
        #endregion
        #region Positioning
        public void UpdatePosition()
        {
            transform.localPosition = VisualTable.GetVisualTile(Piece.Tile).GetLocalPosition(VisualTable) + offset;
        }

        public void ResetRotation()
        {
            transform.localRotation = DefaultLocalRotation;
        }
        #endregion
        #region Generic Methods
        public bool IsPiece<T>() where T : ChessPiece { return Piece.GetType() == typeof(T); }

        public T GetPiece<T>() where T : ChessPiece { return Piece as T; }
        #endregion

        #region Piece Event Subscription & Unsubscription
        void SubscribeToPieceEvents()
        {
            if (Piece != null)
            {
                Piece.Captured += OnCaptured;
                Piece.Moved += OnMoved;
                if (Piece is Rook rook)
                    rook.Castled += OnRookCastled;
            }
            else { Debug.LogWarning("Attempted to 'VisualChessPiece.SubscribeToPieceEvents()' while 'Piece' referenced is null.", gameObject); }
        }

        void UnsubscribeFromPieceEvents()
        {
            if (Piece != null)
            {
                Piece.Captured -= OnCaptured;
                Piece.Moved -= OnMoved;
                if (Piece is Rook rook)
                    rook.Castled -= OnRookCastled;
            }
        }
        #endregion

        #region Piece Event Callbacks
        void OnMoved(MoveInfo pMoveInfo)
        {
            UpdatePosition();
            Moved?.Invoke(pMoveInfo);
        }
        void OnCaptured(MoveInfo pMoveInfo)
        {
            Captured?.Invoke(pMoveInfo);
        }

        void OnRookCastled(ChessPiece pKing, TileIndex pPreCastleRookTile, TileIndex pPostCastleRookTile)
        {
            UpdatePosition();
        }
        #endregion
    }
}
