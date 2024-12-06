using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameController : MonoBehaviour
{
	public static int _coinAmount;
	public static int _collected;
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] public GameObject[] _farms;
	[SerializeField] public int _maxFarmsAmount = 3;
	public int _farmsAmount = 0;
	public GameObject _object;
	public GameObject _objectPrefab;
	public Transform _playerTransform;
	

	public void Start()
	{ 	
		
	}
	public void SpawnFarm()
	{
		float _newxpos = _playerTransform.position.x + Random.Range(-3f, 1.5f);
        float _newypos = _playerTransform.position.y + Random.Range(1f, 2f);
        float _newzpos = _playerTransform.position.z + Random.Range(-3f, 1.5f);

        Instantiate(_objectPrefab, new Vector3(_newxpos, _newypos, _newzpos), new Quaternion(0, 0, 0, 0));
	}
	public void ToQueueSpawn(GameObject _prefab)
	{
		_placementManager._prefabsToSpawn.Add(_prefab);
	}
	
	
	public void ClearQueueSpawn()
	{
		_placementManager._prefabsToSpawn.Clear();
	}
	
	public static void SaveData() 
	{
		PlayerPrefs.SetInt("Coins", _coinAmount);
		PlayerPrefs.SetInt("Collected", _collected);
	}
}
