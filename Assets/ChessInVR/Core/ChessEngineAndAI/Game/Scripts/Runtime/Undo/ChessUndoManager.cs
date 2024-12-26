using System;
using UnityEngine;
using UnityEngine.Events;
using ChessEngine.Undo;

namespace ChessEngine.Game
{

    [RequireComponent(typeof(ChessGameManager))]
    public class ChessUndoManager : MonoBehaviour
    {
        [Serializable]
        public class HistoryEntryUnityEvent : UnityEvent<HistoryEntry> { }

        [Header("Events - History")]
        [Tooltip("An event that is invoked when the last move is undone.")]
        public HistoryEntryUnityEvent MoveUndone;
        [Tooltip("An event that is invoked when the last undone move was redone.")]
        public HistoryEntryUnityEvent MoveRedone;
        [Tooltip("An event that is invoked when this undo managers move history stack becomes empty.")]
        public UnityEvent MoveHistoryEmptied;
        [Tooltip("An event that is invoked when this undo managers move history stack becomes non-empty after being empty.")]
        public UnityEvent MoveHistoryValid;
        [Tooltip("An event that is invoked when this undo managers undo history stack becomes empty.")]
        public UnityEvent UndoHistoryEmptied;
        [Tooltip("An event that is invoked when this undo managers undo history stack becomes non-empty after being empty.")]
        public UnityEvent UndoHistoryValid;

        public ChessGameManager GameManager { get; private set; }
        public InstanceHistory HistoryManager { get; private set; }

        void Awake()
        {
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            GameManager.GameInitialized.AddListener(OnGameInitialized);
        }

        void OnDisable()
        {
            if (GameManager != null)
            {
                GameManager.GameInitialized.RemoveListener(OnGameInitialized);
            }
        }

        public void Undo()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.MoveHistoryCount > 0)
                HistoryManager.UndoMove();
        }

        public void Redo()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.UndoneMovesCount > 0)
                HistoryManager.RedoMove();
        }
		
        public void ClearHistory()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.MoveHistoryCount > 0)
                HistoryManager.ClearHistory();
        }

        public void ClearUndoneMoves()
        {
            if (HistoryManager != null && HistoryManager.Instance != null && HistoryManager.UndoneMovesCount > 0)
                HistoryManager.ClearUndoneMoves();
        }

        void OnGameInitialized()
        {
            HistoryManager = new InstanceHistory(GameManager.ChessInstance);

            HistoryManager.MoveUndone += OnMoveUndone;
            HistoryManager.MoveRedone += OnMoveRedone;
            HistoryManager.MoveHistoryEmptied += OnMoveHistoryEmptied;
            HistoryManager.MoveHistoryValid += OnMoveHistoryValid;
            HistoryManager.UndoHistoryEmptied += OnUndoHistoryEmptied;
            HistoryManager.UndoHistoryValid += OnUndoHistoryValid;
        }

        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        /// <param name="pEntry">The HistoryEntry describing the move that was undone.</param>
        void OnMoveUndone(Instance pChessInstance, HistoryEntry pEntry)
        {
            MoveUndone?.Invoke(pEntry);
        }

        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        /// <param name="pEntry">The HistoryEntry describing the move that was redone.</param>
        void OnMoveRedone(Instance pChessInstance, HistoryEntry pEntry)
        {
            MoveRedone?.Invoke(pEntry);
        }

        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        void OnMoveHistoryEmptied(Instance pChessInstance)
        {
            MoveHistoryEmptied?.Invoke();
        }

        /// <param name="pChessInstance">The chess engine Instance whose move history is being tracked.</param>
        void OnMoveHistoryValid(Instance pChessInstance)
        {
            MoveHistoryValid?.Invoke();
        }

        /// <param name="pChessInstance">The chess engine Instance whose undo history is being tracked.</param>
        void OnUndoHistoryEmptied(Instance pChessInstance)
        {
            UndoHistoryEmptied?.Invoke();
        }

        /// <param name="pChessInstance">The chess engine Instance whose undo history is being tracked.</param>
        void OnUndoHistoryValid(Instance pChessInstance)
        {
            UndoHistoryValid?.Invoke();
        }
    }
}
