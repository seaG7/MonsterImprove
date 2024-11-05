using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	[SerializeField] private PlacementManager _placementManager;
	public DragonBehaviour _cdController;
	[SerializeField] private LayerMask _layer;
	[SerializeField] GameObject[] _eggs;
	[SerializeField] GameObject[] _dragons;
	[SerializeField] GameObject[] _enemyDragons;
	[SerializeField] GameObject _target;
	public GameObject _objectToPlace;
	public List<GameObject> _targets = new List<GameObject>();
	public List<GameObject> _selectedTargets = new List<GameObject>();
	public GameObject _currentDragon;
	public GameObject _enemyDragon;
	public bool needToFight = false;
	public bool isMiniGaming = false;
	public int _destroyedTargetsAmount = 0;
	public Animator _cdAnimator;
	void Start()
	{
		
	}
	void Update()
	{
		
	}
	public void SpawnHatchingEgg()
	{
		if (!needToFight)
		{
			ClearQueueSpawn();
			ToQueueSpawn(_eggs[0]);
		}
	}
	public void StartFight()
	{
		ClearQueueSpawn();
		ToQueueSpawn(_enemyDragons[0]);
	}
	public IEnumerator MinigameFireball(int _countOfTargets)
	{
		SpawnTargets();
		isMiniGaming = true;
		_destroyedTargetsAmount = 0;
		StartCoroutine(_placementManager.AimTarget());
		StartCoroutine(_cdController.DestroyTargets());
		while (isMiniGaming)
		{
			// if выйти по кнопке -> isMiniGaming = false; break;
			if (_targets.Count < 4)
			{
				if (_destroyedTargetsAmount >= _countOfTargets)
				{
					isMiniGaming = false;
				}
				else if (_countOfTargets - _destroyedTargetsAmount >= 4)
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
			_targets[i].tag = "Target";
		}
	}
	public Vector3 RandomPosAroundDragon()
	{
		Vector3 _startPos = _currentDragon.transform.position;
		Vector3 _spawnPos = _startPos;
		_spawnPos.x += Random.Range(-1.2f, 1.2f);
		_spawnPos.z += Random.Range(-1.2f, 1.2f);
		_spawnPos.y += Random.Range(0.25f, 1.4f);
		// while (Physics.Raycast(_startPos, _spawnPos, 10, _layer))
		// {
		// 	_spawnPos = _startPos;
		// 	_spawnPos.x += Random.Range(-1.2f, 1.2f);
		// 	_spawnPos.z += Random.Range(-1.2f, 1.2f);
		// 	_spawnPos.y += Random.Range(0.25f, 1.4f);
		// }
		return _spawnPos;
	}
	public void GestureAttack1()
	{
		_cdController.Turn(_enemyDragon.transform.position);
		StartCoroutine(_cdController.SetAttackState(1));
	}
	public void GestureAttack2()
	{
		_cdController.Turn(_enemyDragon.transform.position);
		StartCoroutine(_cdController.SetAttackState(2));
	}
	public void GestureAttack3()
	{
		_cdController.Turn(_enemyDragon.transform.position);
		StartCoroutine(_cdController.SetAttackState(3));
	}
	public void GestureAttack4()
	{
		_cdController.Turn(_enemyDragon.transform.position);
		StartCoroutine(_cdController.SetAttackState(4));
	}
	public void StopGestureAttack()
	{
		StartCoroutine(_cdController.SetAttackState(0));
	}
	public void ToQueueSpawn(GameObject _prefab)
	{
		_placementManager._prefabsToSpawn.Add(_prefab);
	}
	public void ClearQueueSpawn()
	{
		_placementManager._prefabsToSpawn.Clear();
	}
	public GameObject TakeObject()
	{
		if (_placementManager._spawnedPlaneObjects.Count > 0)
		{
			GameObject _object = _placementManager._spawnedPlaneObjects[0];
			_placementManager._spawnedPlaneObjects.RemoveAt(0);
			return _object;
		}
		Debug.Log("ERROR: NO OBJECT TO TAKE");
		return null;
	}
	public IEnumerator Kill(GameObject _object)
	{
		yield return new WaitForSecondsRealtime(2f);
		
		Destroy(_object);
	}
	public void ShootFromGCScript()
	{
		_cdController.FlyIdleShoot();
	}
}
