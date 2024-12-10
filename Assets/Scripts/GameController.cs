using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameController : MonoBehaviour
{
	[SerializeField] private InventorySystem _inventory;
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] private MenuController _menuController;
	[SerializeField] private PlaneClassification targetPlaneClassification;
	public DragonBehaviour _cdController;
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
	public int _enemyStrength = 0;
	public bool isSwitching = false;
	[SerializeField] public GameObject[] _fightEffects;
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
		if (_CDs.Length > 3*(index+1)-1 && _eggs.Length > index)
		{
			switch (_inventory.CalculateLevel(index))
			{
				case 0:
					ToQueueSpawn(_eggs[index]);
					break;
				case 1:
					ToQueueSpawn(_CDs[3*(index+1)-3]);
					break;
				case 2:
					ToQueueSpawn(_CDs[3*(index+1)-3]);
					break;
				case 3:
					ToQueueSpawn(_CDs[3*(index-1)-2]);
					break;
				case 4:
					ToQueueSpawn(_CDs[3*(index-1)-1]);
					break;
				case 5:
					ToQueueSpawn(_CDs[3*(index-1)-1]);
					break;
			}
		}
	}
	public void SelectED(int index)
	{
		ClearQueueSpawn();
		if (_EDs.Length > index)
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
			if (_destroyedTargetsAmount >= _countOfTargets)
			{
				_menuController.ExitMode();
				isMiniGaming = false;
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
		
		bool isColliding = false;
		
		while (!isColliding)
		{
			_spawnPos.x = _startPos.x + Random.Range(-1f, 1f);
			_spawnPos.z = _startPos.z + Random.Range(-1f, 1f);
			_spawnPos.y = _startPos.y + Random.Range(0.25f, 1.2f);
			isColliding = true;
			// Ray _ray = new Ray(_spawnPos, Vector3.down);
			// RaycastHit _hit;
			// if (Physics.Raycast(_ray, out _hit))
			// {
			// 	isColliding = _hit.transform.TryGetComponent(out ARPlane arPlane) && ((arPlane.classification & targetPlaneClassification) != 0);
			// }
		}
		return _spawnPos;
	}
	public void GestureAttack1()
	{
		if (needToFight)
		{
			StartCoroutine(TurnCD(_enemyDragon.transform.position));
			StartCoroutine(_cdController.SetAttackState(1));
			FindAnyObjectByType<DragonBehaviour>().isAttacking = true;
		}
	}
	public void GestureAttack2()
	{
		if (needToFight)
		{
			StartCoroutine(TurnCD(_enemyDragon.transform.position));
			StartCoroutine(_cdController.SetAttackState(2));
			FindAnyObjectByType<DragonBehaviour>().isAttacking = true;
		}
	}
	public void GestureAttack3()
	{
		if (needToFight)
		{
			StartCoroutine(TurnCD(_enemyDragon.transform.position));
			StartCoroutine(_cdController.SetAttackState(3));
			FindAnyObjectByType<DragonBehaviour>().isAttacking = true;
		}
	}
	public void GestureAttack4()
	{
		if (needToFight)
		{
			StartCoroutine(TurnCD(_enemyDragon.transform.position));
			StartCoroutine(_cdController.SetAttackState(4));
			FindAnyObjectByType<DragonBehaviour>().isAttacking = true;
		}
	}
	public void StopGestureAttack()
	{
		if (needToFight)
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
