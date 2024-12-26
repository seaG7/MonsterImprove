using UnityEngine;
using System.Collections.Generic;
using ChessEngine.Delegates;

namespace ChessEngine.Game
{
    public class VisualChessTable : MonoBehaviour
    {
        #region Editor Serialized Settings
        [Header("Settings")]
        [Tooltip("An editor-set reference to the tile prefab.")]
        [SerializeField] GameObject m_TilePrefab = null;
        [Tooltip("An editor-set reference to the king piece prefab.")]
        [SerializeField] GameObject m_KingPiecePrefab = null;
        [Tooltip("An editor-set reference to the queen piece prefab.")]
        [SerializeField] GameObject m_QueenPiecePrefab = null;
        [Tooltip("An editor-set reference to the bishop piece prefab.")]
        [SerializeField] GameObject m_BishopPiecePrefab = null;
        [Tooltip("An editor-set reference to the knight piece prefab.")]
        [SerializeField] GameObject m_KnightPiecePrefab = null;
        [Tooltip("An editor-set reference to the rook piece prefab.")]
        [SerializeField] GameObject m_RookPiecePrefab = null;
        [Tooltip("An editor-set reference to the pawn piece prefab.")]
        [SerializeField] GameObject m_PawnPiecePrefab = null;
        #endregion
        #region Public Properties
        public int VisualPieceCount { get { return m_VisualChessPieces.Count; } }
        public ChessTable Table { get; private set; }
        public VisualChessTableTile[][] VisualTiles { get; private set; }
        #endregion
        #region Private Fields
        /// <summary>A list of visual chess pieces on the board.</summary>
        List<VisualChessPiece> m_VisualChessPieces = new List<VisualChessPiece>();
        #endregion
        #region C# Delegate Event(s)
        /// <summary>
        /// A delegate event that is invoked with a a reference to a GameObject that will override the default instantiation behaviour.
        /// If the referenced argument is null the default instantiation behaviour will continue, if non-null it will be skipped and the relevant reference will be returned.
        /// Arg0: GameObject - a reference to the prefab GameObject instance that is being instantiated.
        /// Arg1: Transform - the parent Transform for the instantiated GameObject.
        /// Arg2: bool - world position stays?
        /// Arg3: ref GameObject - a reference to the instantiated override GameObject.
        /// </summary>
        public ValueActionRef<GameObject, Transform, bool, GameObject> InstantiateChessPieceDelegate;
        #endregion

        #region Unity Callbacks
        void OnDestroy()
        {
            // unsubscribe from chess instance events.
            UnsubscribeFromChessInstanceEvents();
        }
        #endregion

        #region Initialization & Reset
        public void Initialize(ChessTable pTable)
        {
            Table = pTable;

            VisualTiles = new VisualChessTableTile[8][];
            for (int i = 0; i < VisualTiles.Length; ++i)
            {
                VisualTiles[i] = new VisualChessTableTile[8];
            }

            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    GameObject tileObject = Instantiate(m_TilePrefab, transform);
                    if (tileObject != null)
                    {
                        VisualChessTableTile tile = tileObject.GetComponent<VisualChessTableTile>();
                        tile.Initialize(this, Table.Tiles[x][y]);

                        VisualTiles[x][y] = tile;

                        tile.ResetMaterial();
                    }
                    else { Debug.LogWarning("Failed to instantiate visual tile on VisualChessTable!"); }
                }
            }

            SyncChessPiecesWithTable();

            SubscribeToChessInstanceEvents();
        }
        public void SyncChessPiecesWithTable()
        {
            foreach (VisualChessPiece visualPiece in m_VisualChessPieces)
            {
                if (visualPiece != null)
                    Destroy(visualPiece);
            }
            m_VisualChessPieces.Clear();
            for (int i = 0; i < Table.PieceCount; ++i)
            {
                if (!CreateVisualPiece(Table.GetPieceByIndex(i)))
                    Debug.LogWarning("Failed to create visual representation of chess piece in index '" + i + "'!", gameObject);
            }
        }
        #endregion
        #region Visual Tiles
        public VisualChessTableTile GetVisualTile(TileIndex pTileIndex) { return GetVisualTile(pTileIndex.x, pTileIndex.y); }

        public VisualChessTableTile GetVisualTile(int pX, int pY) { return VisualTiles[pX][pY]; }

        public VisualChessTableTile GetVisualTile(ChessTableTile pTile) { return VisualTiles[pTile.TileIndex.x][pTile.TileIndex.y]; }

        public VisualChessTableTile GetVisualTileByID(string pID)
        {
            for (int y = 0; y < 8; ++y)
            {
                for (int x = 0; x < 8; ++x)
                {
                    VisualChessTableTile tile = GetVisualTile(x, y);
                    if (tile.Tile.TileIndex.GetTileID() == pID)
                        return tile;
                }
            }

            return null;
        }
        #endregion Tiles
        #region Visual Chess Pieces
        public VisualChessPiece GetVisualPiece(ChessPiece pPiece)
        {
            foreach (VisualChessPiece visualPiece in m_VisualChessPieces)
            {
                if (visualPiece != null && visualPiece.Piece == pPiece)
                    return visualPiece;
            }

            return null;
        }

        public VisualChessPiece GetVisualPieceByIndex(int pIndex) { return m_VisualChessPieces[pIndex]; }

        public VisualChessPiece GetVisualPieceByTileIndex(TileIndex pIndex)
        {
            foreach (VisualChessPiece chessPiece in m_VisualChessPieces)
            {
                if (chessPiece.Piece.TileIndex == pIndex)
                    return chessPiece;
            }

            return null;
        }
        #endregion

        #region Chess Instance Event Subscription & Unsubscription
        void SubscribeToChessInstanceEvents()
        {
            if (Table != null)
            {
                Table.ChessPieceCreated += OnChessPieceCreated;
                Table.ChessPieceDestroyed += OnChessPieceDestroyed;
                Table.ChessPieceMoved += OnChessPieceMoved;
            }
        }

        void UnsubscribeFromChessInstanceEvents()
        {
            if (Table != null)
            {
                Table.ChessPieceCreated -= OnChessPieceCreated;
                Table.ChessPieceDestroyed -= OnChessPieceDestroyed;
                Table.ChessPieceMoved -= OnChessPieceMoved;
            }
        }
        #endregion
        #region Visual Piece Creation
        GameObject InstantiateChessPiecePrefab(GameObject pPrefab, Transform pParent, bool pWorldPositionStays)
        {
            GameObject overrideInstance = null;
            InstantiateChessPieceDelegate?.Invoke(pPrefab, pParent, pWorldPositionStays, ref overrideInstance);

            return overrideInstance != null ? overrideInstance : Instantiate(pPrefab, pParent, pWorldPositionStays);
        }

        VisualChessPiece CreateVisualKing(ChessPiece pPiece)
        {
            GameObject kingObject = InstantiateChessPiecePrefab(m_KingPiecePrefab, transform, true);
            if (kingObject != null)
            {
                VisualChessPiece piece = kingObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        public VisualChessPiece CreateVisualQueen(ChessPiece pPiece)
        {
            GameObject queenObject = InstantiateChessPiecePrefab(m_QueenPiecePrefab, transform, true);
            if (queenObject != null)
            {
                VisualChessPiece piece = queenObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        VisualChessPiece CreateVisualBishop(ChessPiece pPiece)
        {
            GameObject bishopObject = InstantiateChessPiecePrefab(m_BishopPiecePrefab, transform, true);
            if (bishopObject != null)
            {
                VisualChessPiece piece = bishopObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        VisualChessPiece CreateVisualKnight(ChessPiece pPiece)
        {
            GameObject knightObject = InstantiateChessPiecePrefab(m_KnightPiecePrefab, transform, true);
            if (knightObject != null)
            {
                VisualChessPiece piece = knightObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        VisualChessPiece CreateVisualRook(ChessPiece pPiece)
        {
            GameObject rookObject = InstantiateChessPiecePrefab(m_RookPiecePrefab, transform, true);
            if (rookObject != null)
            {
                VisualChessPiece piece = rookObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        VisualChessPiece CreateVisualPawn(ChessPiece pPiece)
        {
            GameObject pawnObject = InstantiateChessPiecePrefab(m_PawnPiecePrefab, transform, true);
            if (pawnObject != null)
            {
                VisualChessPiece piece = pawnObject.GetComponent<VisualChessPiece>();
                piece.Initialize(this, pPiece);

                m_VisualChessPieces.Add(piece);
                return piece;
            }

            return null;
        }

        VisualChessPiece CreateVisualPiece(ChessPiece pPiece)
        {
            if (pPiece is Pawn)
            {
                return CreateVisualPawn(pPiece);
            }
            else if (pPiece is Rook)
            {
                return CreateVisualRook(pPiece);
            }
            else if (pPiece is Knight)
            {
                return CreateVisualKnight(pPiece);
            }
            else if (pPiece is Bishop)
            {
                return CreateVisualBishop(pPiece);
            }
            else if (pPiece is Queen)
            {
                return CreateVisualQueen(pPiece);
            }
            else if (pPiece is King)
            {
                return CreateVisualKing(pPiece);
            }
            else
            {
                Debug.LogWarning("VisualChessTable component failed to create visual piece for a chess piece with an invalid type! (Not pawn, rook, knight, bishop, queen, nor king.)", gameObject);
                return null; 
            }
        }
        #endregion

        #region Chess Instance Event Callbacks
        void OnChessPieceCreated(ChessPiece pPiece, TileIndex pTileIndex)
        {
            // Create visual piece if a visualization does not already exist for it.
            VisualChessPiece visualPiece = GetVisualPiece(pPiece);
            if (visualPiece == null)
            {
                // Create visual piece.
                if (!CreateVisualPiece(pPiece))
                    Debug.LogWarning("Failed to create visual representation of chess piece!", gameObject);
            }
        }

        void OnChessPieceDestroyed(ChessPiece pPiece)
        {
            VisualChessPiece visualPiece = GetVisualPiece(pPiece);
            if (visualPiece != null)
            {
                Destroy(visualPiece.gameObject);
            }
        }

        void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            //NOTE: Piece movement visualizations are handled by their own components, they will automatically call UpdatePositions() when their underlying ChessPiece moves.
        }
        #endregion
    }
}
