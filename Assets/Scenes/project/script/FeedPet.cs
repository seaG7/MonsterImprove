using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedPet : MonoBehaviour
{
    public GameObject smileyPrefab;  // Префаб смайлика
    public Transform pet;  // Ссылка на питомца 
    private PetStats petStats;

    void Start()
    {
        if (pet != null)
        {
            petStats = pet.GetComponent<PetStats>(); // Получаем PetStats у питомца
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apple"))  
        {
            Destroy(other.gameObject); 

            if (petStats != null)
            {
                petStats.IncreaseHunger(20); // Повышаем сытость
                ShowSmiley(); 
            }
            else
            {
                Debug.LogError("PetStats не найден! Убедись, что у питомца есть этот компонент.");
            }
        }
    }

    void ShowSmiley()
    {
        Vector3 spawnpose = transform.position;
        spawnpose.y += 0.3f;
        GameObject smiley = Instantiate(smileyPrefab, spawnpose, new Quaternion(0,0,0,0));
        Destroy(smiley, 3f); // Удаляем через 3 секунды
    }
}