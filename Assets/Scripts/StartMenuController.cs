using UnityEngine;
public class StartMenuController : MonoBehaviour
{
    public GameObject startMenu; // —сылка на Canvas с меню
    private bool schet = false;
    public Camera mainCamera;

    void Start()
    {
        // ѕолучаем ссылку на основную камеру
        mainCamera = Camera.main;

        // –азмещаем меню перед камерой
        PositionMenu();

        // ѕоказываем стартовое меню
        startMenu.SetActive(true);
    }

    void PositionMenu()
    {
        if (schet == false)
        {        
            Vector3 position = mainCamera.transform.position + mainCamera.transform.forward * 2f; // 2 метра перед камерой
            startMenu.transform.position = position;  // «адаем позицию меню перед камерой
            startMenu.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);        // «адаем ориентацию меню, чтобы оно было обращено к камере
            schet = true;
        }

    }
}