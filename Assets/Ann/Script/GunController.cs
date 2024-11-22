using System.Collections;
using UnityEngine;

public class GunController : MonoBehaviour
{
	public GameObject gun;         // Ссылка на объект пушки
	public float shootingDuration = 5f;  // Длительность стрельбы
	public float reloadDuration = 3f;    // Длительность перезарядки
	public bool isReloading = false;     // Флаг, указывающий на перезарядку
	public Animator _gunAnimator;
	public AudioSource _gunAS;

	// Компонент, отвечающий за стрельбу (должен быть у вашей пушки)
	public GameObject _shootCollider;
	
	public void GunActive() 
	{
		if (!isReloading)
			StartCoroutine(ShootAndReload());
	}

	public IEnumerator ShootAndReload()
	{
		while (gun.activeSelf) 
		{
			if (_shootCollider != null)
				_shootCollider.SetActive(true); // Начинаем стрельбу
				
			_gunAS.Play();

			yield return new WaitForSeconds(shootingDuration); // Ждем окончания стрельбы
			_gunAnimator.SetTrigger("gunReload");
			_gunAS.Stop();

			_shootCollider.SetActive(false); // Останавливаем стрельбу
			isReloading = true; // Устанавливаем флаг перезарядки

			yield return new WaitForSeconds(reloadDuration); // Ждем завершения перезарядки
			_gunAnimator.SetTrigger("gunReload");

			isReloading = false; // Сбрасываем флаг
			
			
		}
		
		yield return null;
	}
}
