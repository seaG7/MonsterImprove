using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;  // Добавляем XR Toolkit

public class PointGestureController : MonoBehaviour
{
    public PetMovement petMovement;
    public XRRayInteractor leftHandRay;  // Лазерный луч из левой руки
    public bool IsPointGestureDetected = false;

    void Update()
    {
        if (IsPointGestureDetected)
        {
            // Проверяем, попал ли луч в объект
            if (leftHandRay != null && leftHandRay.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                petMovement.MoveToPoint(hit.point);
            }
        }
    }

    public void GestureTrue()
    {
        // Здесь добавляем проверку на жест "указание пальцем"
        IsPointGestureDetected = true; // Пока просто всегда включен
    }

    public void GestureFalse()
    {
        // Здесь добавляем проверку на жест "указание пальцем"
        IsPointGestureDetected = false; // Пока просто всегда включен
    }
}