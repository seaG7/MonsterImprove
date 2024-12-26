using UnityEngine;
using UnityEngine.UI;

namespace ChessEngine.Game.EventHandlers
{

    [RequireComponent(typeof(ChessGameManager))]
    public class GameOverEventHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("A reference to the Text that represents the game over message.")]
        public Text gameOverText;

        public ChessGameManager GameManager { get; private set; }

        void Awake()
        {
            GameManager = GetComponent<ChessGameManager>();
        }

        void OnEnable()
        {
            GameManager.GameOver.AddListener(OnGameOver);
        }

        void OnDisable()
        {
            GameManager.GameOver.RemoveListener(OnGameOver);
        }

        /// <param name="pText"></param>
        public void SetGameOverText(string pText)
        {
            if (gameOverText != null)
                gameOverText.text = pText;
        }

        void OnGameOver(ChessColor pTurn, GameOverReason pReason)
        {
            switch (pReason)
            {
                case GameOverReason.Won:
                    SetGameOverText("Game Over!\n" + pTurn.ToString() + " wins!");
                    break;
                case GameOverReason.Draw:
                    SetGameOverText("Game Over!\nDraw");
                    break;
                case GameOverReason.Forfeit:
                    if (pTurn == ChessColor.Black)
                    {
                        SetGameOverText("Game Over!\nWhite wins!");
                    }
                    else { SetGameOverText("Game Over!\nBlack wins!"); }
                    break;
                case GameOverReason.TimeExpired:
                    if (pTurn == ChessColor.Black)
                    {
                        SetGameOverText("Game Over!\nWhite wins!");
                    }
                    else { SetGameOverText("Game Over!\nBlack wins!"); }
                    break;
                default:
                    SetGameOverText("Game Over!");

                    Debug.LogWarning("Unhandled GameOverReason found '" + pReason.ToString() + "'!");
                    break;
            }
        }
    }
}
