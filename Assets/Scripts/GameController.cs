using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	[SerializeField] private PlacementManager _placementManager;
	public DragonBehaviour _dragonController;
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
		
		int _destroyedTargetsAmount = 0;
		isMiniGaming = true;
		while (isMiniGaming)
		{
			// if выйти по кнопке -> isMiniGaming = false; break;
			
			if (_targets.Count < 4)
			{
				_destroyedTargetsAmount++;
				if (_destroyedTargetsAmount >= _countOfTargets)
				{
					isMiniGaming = false;
					StopCoroutine(MinigameFireball(_countOfTargets));
					break;
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
			_targets[i].tag = "Target";
		}
	}
	public Vector3 RandomPosAroundDragon()
	{
		Vector3 _startPos = _currentDragon.transform.position;
		Vector3 _spawnPos = _startPos;
		_spawnPos.x += Random.Range(-1.5f, 1.5f);
		_spawnPos.z += Random.Range(-1.5f, 1.5f);
		_spawnPos.y += Random.Range(0.15f, 1.4f);
		while (Physics.Raycast(_startPos, _spawnPos, 10, _layer))
		{
			_spawnPos = _startPos;
			_spawnPos.x += Random.Range(-1.5f, 1.5f);
			_spawnPos.z += Random.Range(-1.5f, 1.5f);
			_spawnPos.y += Random.Range(0.15f, 1.4f);
		}
		return _spawnPos;
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
}
