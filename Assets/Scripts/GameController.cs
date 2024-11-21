using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public class GameController : MonoBehaviour
{
	[SerializeField] private InventorySystem _inventory;
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] private MenuController _menuController;
	public DragonBehaviour _cdController;
	[SerializeField] private LayerMask _layer;
	[SerializeField] GameObject[] _eggs;
	[SerializeField] GameObject[] _CDs;
	[SerializeField] GameObject[] _EDs;
	[SerializeField] GameObject _target;
	public GameObject _objectToPlace;
	public List<GameObject> _targets = new List<GameObject>();
	public List<GameObject> _selectedTargets = new List<GameObject>();
	public int _countSelectedTargets = 0;
	public int _targetsCount = 0;
    public GameObject _currentDragon;
	public GameObject _enemyDragon;
	public bool needToFight = false;
	public bool isMiniGaming = false;
	public int _destroyedTargetsAmount = 0;
	public Animator _cdAnimator;
	public int _cdIndex = -1;
	public bool isSwitching = false;
	void Start()
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
		ToQueueSpawn(_EDs[0]);
	}
	public void SelectCD(int index) // поставить префабы в порядке синий мал ср бол, красный мал ср бол и тд
	{
		ClearQueueSpawn();
		switch (_inventory.CalculateLevel(index))
		{
			case 0:
				ToQueueSpawn(_eggs[index]);
				break;
			case 1:
				ToQueueSpawn(_CDs[3*(index+1)-3]);
				break;
			case 3:
				ToQueueSpawn(_CDs[3*(index-1)-2]);
				break;
			case 5:
				ToQueueSpawn(_CDs[3*(index-1)-1]);
				break;
		}
	}
	public void SelectED(int index)
	{
		ClearQueueSpawn();
		ToQueueSpawn(_EDs[index]);
	}
	public IEnumerator MinigameFireball(int _countOfTargets)
	{
		_targetsCount = _countOfTargets;
		SpawnTargets();
		needToFight = false;
		isMiniGaming = true;
		_destroyedTargetsAmount = 0;
		StartCoroutine(_placementManager.AimTarget());
		StartCoroutine(_cdController.DestroyTargets());
		while (isMiniGaming)
		{
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
		
		bool isColliding = true;

		while (isColliding)
		{
			_spawnPos.x = _startPos.x + Random.Range(-1f, 1f);
			_spawnPos.z = _startPos.z + Random.Range(-1f, 1f);
			_spawnPos.y = _startPos.y + Random.Range(0.25f, 1.2f);

			Ray _ray = new Ray(_startPos, _spawnPos - _startPos);
			RaycastHit _hit;

			Collider _colliderPlane = FindAnyObjectByType<ARPlane>().GetComponent<Collider>();
			isColliding = _colliderPlane.Raycast(_ray, out _hit, Vector3.Distance(_startPos, _spawnPos));
		}

		return _spawnPos;
	}
	public void GestureAttack1()
	{
		StartCoroutine(TurnCD(_enemyDragon.transform.position));
		StartCoroutine(_cdController.SetAttackState(1));
	}
	public void GestureAttack2()
	{
		StartCoroutine(TurnCD(_enemyDragon.transform.position));
		StartCoroutine(_cdController.SetAttackState(2));
	}
	public void GestureAttack3()
	{
		StartCoroutine(TurnCD(_enemyDragon.transform.position));
		StartCoroutine(_cdController.SetAttackState(3));
	}
	public void GestureAttack4()
	{
		StartCoroutine(TurnCD(_enemyDragon.transform.position));
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
		yield return new WaitForSecondsRealtime(4f);
		
		Destroy(_object);
	}
	public void ShootFromGCScript()
	{
		_cdController.FlyIdleShoot();
	}
	public IEnumerator TurnCD(Vector3 lookAt)
	{
		lookAt = lookAt - _currentDragon.transform.position;
		Quaternion _targetRot = Quaternion.LookRotation(lookAt);
		_targetRot.x = _currentDragon.transform.rotation.x;
		_targetRot.z = _currentDragon.transform.rotation.z;
		while (transform.rotation != _targetRot)
		{
			_currentDragon.transform.rotation = Quaternion.Slerp(_currentDragon.transform.rotation, _targetRot, 4 * Time.deltaTime);
			yield return null;
		}
	}
	public void SwitchGrowth()
	{
		Transform _transform = _currentDragon.transform;
		Destroy(_currentDragon);
		switch (_inventory.CalculateLevel(_cdIndex))
		{
			case 3:
				_currentDragon = Instantiate(_CDs[3 * (_cdIndex - 1) - 2]);
				isSwitching = true;
				break;
			case 5:
				_currentDragon = Instantiate(_CDs[3 * (_cdIndex - 1) - 1]);
				isSwitching = true;
				break;
		}
	}
}
