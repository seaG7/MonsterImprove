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
	[SerializeField] Camera _mainCamera;
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
		if (!needToFight)
			StartCoroutine(HatchingDragon());
	}
	public void StartFight()
	{
		Vector3 _spawnPos = _currentDragon.transform.position;
		_spawnPos.x += 4f;
		_spawnPos.z += 10f;
		_enemyDragon = Instantiate(_enemyDragons[0], _spawnPos, Quaternion.identity);
		_currentDragon.transform.LookAt(_enemyDragon.transform);
		isFighting = true;
	}
	public IEnumerator HatchingDragon()
	{
		// GameObject _hatchingEgg = Instantiate(_eggs[0]);
		GameObject _hatchingEgg = SpawnObject(_eggs[0]);
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
	public GameObject SpawnObject(GameObject _object)
	{
		Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			// Спавн объекта на месте удара
			GameObject _spawnedObject = Instantiate(_object, hit.point, Quaternion.identity);
			return _spawnedObject;
		}
		return null;
	}
	public IEnumerator MinigameFireball(int _countOfTargets)
	{
		SpawnTargets();
		
		int _destroyedTargetsAmount = 0;
		while (isMiniGaming)
		{
			
			// if выйти по кнопке -> isMiniGaming = false; break;
			
			if (_targets.Count < 4)
			{
				_destroyedTargetsAmount++;
				if (_destroyedTargetsAmount < _countOfTargets)
				{
					isMiniGaming = false;
					break; // мб надо стопить короутину, хз))
				}
				_targets.Add(Instantiate(_target, RandomPosAroundDragon(), Quaternion.identity));
				
				// звук появления target и чет еще мэйби
			}
			yield return null;
		}
	}
	public void SpawnTargets()
	{
		for (int i = 0; i < 4; i++)
		{
			_targets.Add(Instantiate(_target, RandomPosAroundDragon(), Quaternion.identity));
		}
	}
	public Vector3 RandomPosAroundDragon()
	{
		Vector3 _spawnPos = _currentDragon.transform.position;
		_spawnPos.x += Random.Range(-5f, 5f);
		_spawnPos.z += Random.Range(-5f, 5f);
		_spawnPos.y += Random.Range(0.8f, 3f);
		return _spawnPos;
	}
	public void GestureAttack1()
	{
		if (needToFight)
			_dragonController.FirstAttack();
	}
	public void GestureAttack2()
	{
		if (needToFight)
			_dragonController.SecondAttack();
	}
	public void GestureAttack3()
	{
		if (needToFight)
			_dragonController.ThirdAttack();
	}
	public void GestureAttack4()
	{
		if (needToFight)
			_dragonController.FourthAttack();
	}
	public void StopGestureAttack()
	{
		if (needToFight)
			_dragonController.StopAttack();
	}
}
