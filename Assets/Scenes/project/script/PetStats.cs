using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class PetStats : MonoBehaviour
{
    public int experience = 0;  // Очки опыта

    public GameOverMenuController gameOverMenuController;  // Ссылка на скрипт UI
    public int evolvecount = 0; // Переменная, отслеживающая количество эволюций
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
        if (isDead) return; // Питомец уже мертв, не повторять

        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die"); // Запуск анимации смерти
        }

        if (gameOverMenuController != null)
        {
            gameOverMenuController.ShowGameOverMenu(); // меню смерти
        }

        // Удаляем чз 5 секунд после анимации
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (evolvecount == 2)
        {
            // Если эволюция завершена (evolvecount == 2), не выполняем дальше
            Debug.Log("Эволюция завершена, питомец больше не будет эволюционировать.");
            return;
        }

        if (evolvecount != 2)
        {
            // Обновляем таймер эволюции
            evolutionTime -= Time.deltaTime;
            if (evolutionTime <= 0)
            {
                Evolve();
            }
        }

        // Если характеристики счастья или голода = 0, питомец умирает
        if (happiness <= 5 || hunger <= 5)
        {
            Die();
        }
    }

    // Корутин для уменьшения характеристик питомца с течением времени
    IEnumerator DecreaseStatsOverTime()
    {
        while (!isDead) // Пока питомец жив
        {
            yield return new WaitForSeconds(3f); // Ждем 3 секунды перед каждым уменьшением характеристик

            happiness = Mathf.Max(happiness - 3, 0); // Уменьшаем счастье, но не ниже 0
            hunger = Mathf.Max(hunger - 3, 0); // Уменьшаем голод, но не ниже 0

        }
    }

    public void IncreaseHappiness(int amount)
    {
        happiness = Mathf.Min(happiness + amount, 100); // Увеличиваем счастье, но не выше 100
    }

    public void IncreaseHunger(int amount)
    {
        hunger = Mathf.Min(hunger + amount, 100); // Увеличиваем голод, но не выше 100
    }

    public GameObject evolutionEffectPrefab;  // Префаб эффекта эволюции

    // Метод эволюции питомца
    void Evolve()
    {
        if (isDead || evolvecount == 2)
        {
            // Если питомец мертв или эволюция завершена, ничего не делаем
            Debug.Log("Эволюция не происходит, так как питомец мертв или уже завершил эволюцию.");
            return;
        }

        // Создание нового питомца, если это первая или вторая эволюция
        GameObject newPet = gameObject;

        if (evolvecount == 0)
        {
            newPet = Instantiate(Resources.Load<GameObject>("EvolvedPet"), transform.position, Quaternion.identity); // Создаём новую модель питомца
        }
        else if (evolvecount == 1)
        {
            newPet = Instantiate(Resources.Load<GameObject>("EvolvedPet2"), transform.position, Quaternion.identity); // Создаём новую модель питомца
        }

        // Создаём эффект эволюции
        GameObject effect = Instantiate(Resources.Load<GameObject>("EvolutionEffect"), transform.position, Quaternion.identity);
        Destroy(effect, 4f); // Удаляем эффект через 4 секунды

        // Переносим характеристики на нового питомца
        PetStats newPetStats = newPet.GetComponent<PetStats>();
        if (newPetStats != null)
        {
            newPetStats.happiness = happiness;
            newPetStats.hunger = hunger;
            newPetStats.evolutionTime = 90f; // Сбрасываем таймер для следующей эволюции
            newPetStats.gameOverMenuController = gameOverMenuController;
            FindObjectOfType<PointGestureController>().petMovement = newPet.GetComponent<PetMovement>();
            newPetStats.evolvecount = evolvecount + 1; // Обновляем счётчик эволюции на новом питомце
            FindObjectOfType<PetUIController>().petStats = newPetStats.GetComponent<PetStats>();
        }

        // Удаляем старого питомца
        Destroy(gameObject);
    }
}