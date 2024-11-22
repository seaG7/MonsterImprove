using UnityEngine;

public class GunShooting : MonoBehaviour
{
    public GameObject projectilePrefab; // Префаб снаряда
    public Transform shootPoint;        // Точка вылета снаряда
    public float shootRate = 0.2f;      // Скорость стрельбы (каждые 0.2 сек)
    private bool isShooting = false;

    public void StartShooting()
    {
        isShooting = true;
        InvokeRepeating(nameof(Shoot), 0f, shootRate);
    }

    public void StopShooting()
    {
        isShooting = false;
        CancelInvoke(nameof(Shoot));
    }

    private void Shoot()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
    }
}
