using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
public class PetStats : MonoBehaviour
{
    public GameObject gameOverMenu; // Меню смерти (Canvas)
    public Button restartButton;    // Кнопка для перезапуска
    public GameOverMenuController gameOverMenuController;  // Ссылка на скрипт UI

    public static PetStats instance;

    public int happiness = 50;
    public int hunger = 50;
    public float evolutionTime = 90f; // 1.5 минуты
    private Animator animator;

    private bool isDead = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        StartCoroutine(DecreaseStatsOverTime()); // Запускаем корутину при старте
        animator = GetComponent<Animator>();
    }
    public void Die()
    {

        if (isDead) return;
        isDead = true;
        gameOverMenuController.ShowGameOverMenu();
        animator.SetTrigger("Die");
        Destroy(gameObject, 5f); // Удаляем питомца через 2 секунды после анимации
    }


    void Update()
    {
        evolutionTime -= Time.deltaTime;
        if (evolutionTime <= 0)
        {
            Evolve();
        }

        if (happiness <= 0 || hunger <= 0)
        {
            Die();
        }
    }

    IEnumerator DecreaseStatsOverTime()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(2f); // Ждём 2 секунды
            happiness = Mathf.Max(happiness - 2, 0);
            hunger = Mathf.Max(hunger - 2, 0);
            Debug.Log($"Характеристики падают: Радость {happiness}, Голод {hunger}");
        }
    }

    public void IncreaseHappiness(int amount)
    {
        happiness = Mathf.Min(happiness + amount, 100);
    }

    public void IncreaseHunger(int amount)
    {
        hunger = Mathf.Min(hunger + amount, 100);
    }

    public GameObject evolutionEffectPrefab;  // Префаб эффекта

    void Evolve()
    {
        if (isDead) return;

        Debug.Log("Питомец эволюционировал! 🎉");

        // Создаём эффект эволюции
        GameObject effect = Instantiate(Resources.Load<GameObject>("EvolutionEffect"), transform.position, Quaternion.identity);
        Destroy(effect, 4f); // Удаляем эффект через 4 секунды

        // Сохраняем ссылку на старый PointGestureController
        PointGestureController oldGestureController = GetComponent<PointGestureController>();
        XRRayInteractor oldRay = oldGestureController != null ? oldGestureController.leftHandRay : null;

        // Создаём новую модель питомца
        GameObject newPet = Instantiate(Resources.Load<GameObject>("EvolvedPet"), transform.position, Quaternion.identity);

        // Переносим характеристики
        PetStats newPetStats = newPet.GetComponent<PetStats>();
        if (newPetStats != null)
        {
            newPetStats.happiness = happiness;
            newPetStats.hunger = hunger;
            newPetStats.evolutionTime = 90f; // Сбрасываем таймер для следующей эволюции
        }

        // Копируем все важные скрипты
        CopyComponent<PetMovement>(gameObject, newPet);
        CopyComponent<FeedPet>(gameObject, newPet);
        CopyComponent<PointGestureController>(gameObject, newPet);

        // Восстанавливаем ссылку на leftHandRay
        PointGestureController newGestureController = newPet.GetComponent<PointGestureController>();
        if (newGestureController != null)
        {
            newGestureController.leftHandRay = oldRay;
        }

        // Удаляем старого питомца
        Destroy(gameObject);
    }

    // Функция копирования компонентов
    void CopyComponent<T>(GameObject oldObj, GameObject newObj) where T : Component
    {
        T oldComponent = oldObj.GetComponent<T>();
        if (oldComponent != null)
        {
            T newComponent = newObj.AddComponent<T>();
            System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                field.SetValue(newComponent, field.GetValue(oldComponent));
            }
        }
    }
}