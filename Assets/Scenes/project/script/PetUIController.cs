using UnityEngine;
using TMPro;  // Добавляем поддержку TextMeshPro

public class PetUIController : MonoBehaviour
{

    [SerializeField] public PetStats petStats;   // Ссылка на скрипт с характеристиками питомца
    public TMP_Text happinessText;  // Используем TMP_Text для отображения счастья
    public TMP_Text hungerText;     // Используем TMP_Text для отображения голода
    public TMP_Text evolutionTimeText;  // Используем TMP_Text для отображения времени до эволюции

    void Update()
    {
        // Обновляем значения на UI каждый кадр
        if (petStats != null && petStats.evolvecount != 2)
        {
            happinessText.text = "Радость: " + petStats.happiness.ToString(); // Отображаем значение счастья
            hungerText.text = "Голод: " + petStats.hunger.ToString(); // Отображаем значение голода
            evolutionTimeText.text = "До эволюции: " + Mathf.Max(petStats.evolutionTime, 0f).ToString("F1") + " сек."; // Отображаем время до эволюции
        }
        else
        {
            happinessText.text = "Радость: бесконечна"; // Отображаем значение счастья
            hungerText.text = "Голод: бесконечна"; // Отображаем значение голода
            evolutionTimeText.text = "До эволюции: достигнута последняя стадия эволюции"; // Отображаем время до эволюции
        }
    }
}