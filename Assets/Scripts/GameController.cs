using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	private DragonBehaviour _dragonController;
	[SerializeField] GameObject[] _eggs;
	[SerializeField] GameObject[] _dragons;
	[SerializeField] GameObject[] _enemyDragons;
	[SerializeField] GameObject _target;
	[SerializeField] public float _hatchingTime;
	public List<GameObject> _targets = new List<GameObject>();
	public GameObject _currentDragon;
	public GameObject _enemyDragon;
	public bool needToFight = false;
	public bool isMiniGaming = false;
	public bool isFighting = false;
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
		_spawnPos.x += 4f;
		_spawnPos.z += 4f;
		_enemyDragon = Instantiate(_enemyDragons[0], _spawnPos, Quaternion.identity);
		_currentDragon.transform.LookAt(_enemyDragon.transform);
		isFighting = true;
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
		
		yield return new WaitForSecondsRealtime(2f);
		Destroy(_hatchingEgg);
	}
	public IEnumerator MinigameFireball()
	{
		SpawnTargets();
		while (isMiniGaming)
		{
			
			yield return null;
		}
	}
	public void SpawnTargets()
	{
		for (int i = 0; i < 4; i++)
		{
			Vector3 _spawnPos = _currentDragon.transform.position;
			switch (i)
			{
				case 0:
					_spawnPos.x += 5;
					_spawnPos.z += 5;
					break;
				case 1:
					_spawnPos.x -= 5;
					_spawnPos.z -= 5;
					break;
				case 2:
					_spawnPos.x += 5;
					_spawnPos.z -= 5;
					break;
				case 3:
					_spawnPos.x -= 5;
					_spawnPos.z += 5;
					break;
			}
			_spawnPos.y += Random.Range(0f, 3f);
			_targets.Add(Instantiate(_target, _spawnPos, Quaternion.identity));
		}
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
