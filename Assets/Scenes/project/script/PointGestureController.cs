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
                petMovement.MoveToPoint(hit.point, hit.transform);
            }
        }
    }

    public void GestureTrue()
    {
        
        IsPointGestureDetected = true; // просто всегда включен
    }

    public void GestureFalse()
    {
        
        IsPointGestureDetected = false; // просто всегда включен
    }
}