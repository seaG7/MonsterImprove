using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public static int _coinAmount;
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] public GameObject[] _farms;
	[SerializeField] private int _maxFarmsAmount = 5;
	public int _farmsAmount = 0;
	public GameObject _object;
	public AudioSource _mainAS;
	[SerializeField] public AudioClip _sellSomething;
	[SerializeField] public AudioClip _buySomething;
	[SerializeField] public AudioClip _placeFarm;
	[SerializeField] public AudioClip[] _clicks;
	
	
	public void Start()
	{
		_mainAS = GameObject.FindGameObjectWithTag("MainAS").GetComponent<AudioSource>();
		_coinAmount = PlayerPrefs.GetInt("Coins");
	}
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
	
	public void PlayClick() 
	{
		_mainAS.clip = _clicks[Random.Range(0, 2)];
		_mainAS.Play();
	}
	
	public void ClearQueueSpawn()
	{
		_placementManager._prefabsToSpawn.Clear();
	}
	
	public IEnumerator SaveCoins() 
	{
		yield return new WaitForSeconds(5f);
		PlayerPrefs.SetInt("Coins", _coinAmount);
	}
}
