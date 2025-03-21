using UnityEngine;
public class StartMenuController : MonoBehaviour
{
    public GameObject startMenu; // Ссылка на Canvas с меню
    private bool schet = false;
    public Camera mainCamera;

    void Start()
    {
        // Получаем ссылку на основную камеру
        mainCamera = Camera.main;

        // Размещаем меню перед камерой
        PositionMenu();

        // Показываем стартовое меню
        startMenu.SetActive(true);
    }

    void PositionMenu()
    {
        if (schet == false)
        {        
            Vector3 position = mainCamera.transform.position + mainCamera.transform.forward * 0.5f; //  перед камерой
            startMenu.transform.position = position;  // Задаем позицию меню перед камерой
            startMenu.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);        // Задаем ориентацию меню, чтобы оно было обращено к камере
            schet = true;
        }

    }
}