using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenu;  // Ссылка на панель с меню
    public GameObject pet;           // Ссылка на питомца, чтобы отследить его состояние

    void Start()
    {
        // Изначально меню скрыто
        gameOverMenu.SetActive(false);
    }

    void Update()
    {
        // Проверяем, если питомец мертв, показываем меню и останавливаем игру
        if (pet == null)
        {
            ShowGameOverMenu();
        }
    }

    public void ShowGameOverMenu()
    {
        // Останавливаем игру
        Time.timeScale = 0;  // Останавливаем все время в игре (это приостановит все действия)

        // Показываем меню
        gameOverMenu.SetActive(true);
    }

    // Метод для перезапуска игры
    public void RestartGame()
    {
        // Возвращаем время в нормальное состояние
        Time.timeScale = 1;

        // Перезагружаем сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}