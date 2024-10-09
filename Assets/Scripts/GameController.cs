using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using Unity.XR.CoreUtils.GUI;
using UnityEngine;

public class GameController : MonoBehaviour
{
	private DragonBehaviour _dragonController;
	[SerializeField] GameObject[] _eggs;
	[SerializeField] GameObject[] _dragons;
	[SerializeField] GameObject[] _enemyDragons;
	[SerializeField] public float _hatchingTime;
	public GameObject _currentDragon;
	public GameObject _enemyDragon;
	public bool needToFight = false;
	public bool isHatching = false;
	void Start()
	{
		
	}
	void Update()
	{
		
	}
	public void SpawnHatchingEgg()
	{
		StartCoroutine(HatchingDragon());
	}
	public void StartFight()
	{
		Vector3 _spawnPos = _currentDragon.transform.position;
		_spawnPos.x += 8f;
		_spawnPos.z += 8f;
		_enemyDragon = Instantiate(_enemyDragons[0], _spawnPos, Quaternion.identity);
		_currentDragon.transform.LookAt(_enemyDragon.transform);
	}
	public IEnumerator HatchingDragon()
	{
		GameObject _hatchingEgg = Instantiate(_eggs[0]);
		Animator _eggAnimator = _hatchingEgg.GetComponent<Animator>();
		
		Vector3 _spawnPos = _hatchingEgg.transform.position;
		_spawnPos.y += 0.08f;
		_spawnPos.z -= 0.05f;
		
		yield return new WaitForSecondsRealtime(_hatchingTime);
		isHatching = true;
		_currentDragon = Instantiate(_dragons[0], _spawnPos, Quaternion.identity);
		_currentDragon.transform.LookAt(FindAnyObjectByType<Camera>().transform);
		_dragonController = _currentDragon.GetComponent<DragonBehaviour>();
		
		_eggAnimator.SetInteger("Crack", 1);
		
		yield return new WaitForSecondsRealtime(3f);
		Destroy(_hatchingEgg);
	}
	public void GestureAttack1()
	{
		_dragonController.FirstAttack();
	}
	public void GestureAttack2()
	{
		_dragonController.SecondAttack();
	}
	public void GestureAttack3()
	{
		_dragonController.ThirdAttack();
	}
	public void GestureAttack4()
	{
		_dragonController.FourthAttack();
	}
	public void StopGestureAttack()
	{
		_dragonController.StopAttack();
	}
}
