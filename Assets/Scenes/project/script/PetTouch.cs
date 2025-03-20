using System.Collections;
using UnityEngine;

public class PetTouch : MonoBehaviour
{
    public GameObject heartPrefab;  // Префаб сердечка
    private PetStats petStats;

    void Start()
    {
        petStats = GetComponent<PetStats>(); // Получаем ссылку на характеристики питомца
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand")) // Проверяем, коснулся ли питомца игрок
        {
            ShowHeart(); // Показываем сердечко
            petStats.IncreaseHappiness(10); // Увеличиваем радость
        }
    }

    void ShowHeart()
    {
        Vector3 spawnpose = transform.position;
        spawnpose.y += 0.4f;
        GameObject Heart = Instantiate(heartPrefab, spawnpose, new Quaternion(0, 0, 0, 0));
        Destroy(Heart, 3f); // Удаляем через 3 секунды
    }
}