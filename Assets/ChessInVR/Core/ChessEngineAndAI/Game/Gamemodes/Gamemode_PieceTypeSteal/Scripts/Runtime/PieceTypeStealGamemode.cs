using UnityEngine;

namespace ChessEngine.Game.Gamemodes.PieceTypeSteal
{
    public class PieceTypeStealGamemode : MonoBehaviour
    {
        void OnEnable()
        {
            Instance.Initialized += OnInstanceInitialized;
            Instance.Deinitialized += OnInstanceDeinitialized;
        }

        void OnDisable()
        {
            Instance.Initialized -= OnInstanceInitialized;
            Instance.Deinitialized -= OnInstanceDeinitialized;
        }

        #region Chess Instance Static Callback(s)
        /// <param name="pInstance"></param>
        protected void OnInstanceInitialized(Instance pInstance)
        {
            pInstance.ChessPieceMoved += OnChessPieceMoved;
        }

        /// <summary>Invoked just after any chess Instance is deconstructed.</summary>
        /// <param name="pInstance"></param>
        protected void OnInstanceDeinitialized(Instance pInstance)
        {
            pInstance.ChessPieceMoved -= OnChessPieceMoved;
        }
        #endregion
        #region Chess Instance Specific Callback(s)
        /// <param name="pMoveInfo"></param>
        protected void OnChessPieceMoved(MoveInfo pMoveInfo)
        {
            ChessPieceType movedPieceType = pMoveInfo.piece.GetChessPieceType();
            if (movedPieceType != ChessPieceType.King)
            {

                if (pMoveInfo.capturedPiece != null)
                {
                    bool isPawnQueenedMove = false;
                    if (movedPieceType == ChessPieceType.Pawn)
                    {
                        if (pMoveInfo.piece.Color == ChessColor.White)
                        {
                            // White team.
                            if (pMoveInfo.piece.TileIndex.y == 7)
                                isPawnQueenedMove = true;
                        }
                        else
                        {
                            // Black team.
                            if (pMoveInfo.piece.TileIndex.y == 0)
                                isPawnQueenedMove = true;
                        }
                    }

                    if (!isPawnQueenedMove)
                    {
                        ChessPieceType victimPieceType = pMoveInfo.capturedPiece.GetChessPieceType();
                        if (movedPieceType != victimPieceType && victimPieceType != ChessPieceType.King)
                        {
                            ChessTable table = pMoveInfo.piece.Table;
                            ChessColor color = pMoveInfo.piece.Color;

                            table.DestroyPiece(pMoveInfo.piece);

                            table.CreatePieceByType(victimPieceType, pMoveInfo.capturedPiece.TileIndex, color);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
