using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PetStats : MonoBehaviour
{
    public static PetStats instance;

    public int happiness = 50;
    public int hunger = 50;
    public float evolutionTime = 90f; // 1.5 минуты

    private bool isDead = false;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        StartCoroutine(DecreaseStatsOverTime()); // Запускаем корутину при старте
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

    void Evolve()
    {
        if (isDead) return;
        Debug.Log("Питомец эволюционировал!");
        GameObject newPet = Instantiate(Resources.Load<GameObject>("EvolvedPet"), transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Питомец умер... 😢");
        GetComponent<PetMovement>().Die(); // Вызов анимации смерти
    }

}