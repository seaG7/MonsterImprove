using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PetInteraction : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            //  од дл€ того, чтобы питомец начал гладитьс€
            Debug.Log("ѕитомец гладитс€!");
            // ¬оспроизведение анимации или других действий дл€ гладки
        }
    }
}