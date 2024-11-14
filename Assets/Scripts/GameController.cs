using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] GameObject[] _farms;
	[SerializeField] private int _maxFarmsAmount = 5;
	public int _farmsAmount = 0;
	public bool needToFight = false;
	
	void Start()
	{
		
	}
	void Update()
	{
		
	}
	public void SpawnFarm()
	{
		if (_farmsAmount < _maxFarmsAmount)
		{
			ClearQueueSpawn();
			ToQueueSpawn(_farms[0]);
		}
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
}
