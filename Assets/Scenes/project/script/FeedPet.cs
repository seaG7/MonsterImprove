using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
public class FeedPet : MonoBehaviour
{
    public GameObject smileyPrefab;  // Префаб смайлика
    public Transform pet;  // Питомец

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apple"))
        {
            Destroy(other.gameObject);  // Удаляем яблоко
            ShowSmiley();
            PetStats.instance.IncreaseHunger(10);
        }
    }

    void ShowSmiley()
    {
        GameObject smiley = Instantiate(smileyPrefab, pet.position + new Vector3(0, 0.1167f, 0), Quaternion.identity);
        Destroy(smiley, 1.5f); // Удаляем через 1.5 секунды
    }
}