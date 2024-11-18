using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] public GameObject[] _farms;
	[SerializeField] private int _maxFarmsAmount = 5;
	public int _farmsAmount = 0;
	public GameObject _object;
	public void SpawnFarm()
	{
		if (_farmsAmount < _maxFarmsAmount)
		{
			ClearQueueSpawn();
			ToQueueSpawn(_farms[0]);
			_placementManager.buttonPressed = true;
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
}
