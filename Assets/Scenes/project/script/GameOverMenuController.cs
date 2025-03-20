using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // Для работы с кнопками UI

public class GameOverMenuController : MonoBehaviour
{
    public GameObject gameOverMenu;
    public Button restartButton;     // Кнопка для перезапуска
    public Transform playerCamera; // Камера игрока
    private bool menuDisplayed = false; // Флаг для отслеживания, чтобы меню появилось только один раз

    void Start()
    {
        gameOverMenu.SetActive(false);  // меню смерти скрыто при старте игры

        restartButton.onClick.AddListener(RestartGame);  // кнопка перезапуска игры
    }

    public void ShowGameOverMenu()
    {
        if (!menuDisplayed)
        {
            // Позиционируем меню перед камерой
            gameOverMenu.transform.position = playerCamera.position + playerCamera.forward * 2f; // Расстояние 2 метра перед камерой
            gameOverMenu.transform.LookAt(playerCamera); //  меню на камеру

            // Показываем меню смерти
            gameOverMenu.SetActive(true);

            menuDisplayed = true; // Меню было отображено, флаг установлен
        }
    }

    public void RestartGame()
    {
        // Скрываем меню
        gameOverMenu.SetActive(false);

        // Перезапускаем сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}