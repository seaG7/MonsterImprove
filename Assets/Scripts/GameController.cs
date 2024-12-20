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
	public AudioSource _mainAS;
	[SerializeField] public AudioClip _sellSomething;
	[SerializeField] public AudioClip _buySomething;
	[SerializeField] public AudioClip _placeFarm;
	[SerializeField] public AudioClip _click;
	[SerializeField] public AudioClip _noMoney;
	[SerializeField] public TextMeshProUGUI _farmAmountTMP;
	[SerializeField] public TextMeshProUGUI _farmAmountBackTMP;
	[SerializeField] public TextMeshProUGUI _coinAmountTMP;
	[SerializeField] public TextMeshProUGUI _coinAmountBackTMP;
	[SerializeField] public TextMeshProUGUI _collectedTMP;
	[SerializeField] public TextMeshProUGUI _collectedBackTMP;
	[SerializeField] public GameObject _canvasNotification;
	[SerializeField] public GameObject _goldNotification;
	[SerializeField] public GameObject _foodNotification;
	[SerializeField] public GameObject _collectiblesNotification;
 	public int id;
	public bool _generatorPlaced = false;
	
	public IEnumerator _showNotification(int id) 
	{
		if (id == 0) 
		{
			_canvasNotification.SetActive(true);
			_goldNotification.SetActive(true);
			
			yield return new WaitForSeconds(3f);
		
			_goldNotification.SetActive(false);
			_canvasNotification.SetActive(false);
		}
		
		if (id == 1) 
		{
			_canvasNotification.SetActive(true);
			_foodNotification.SetActive(true);
			
			yield return new WaitForSeconds(3f);
		
			_foodNotification.SetActive(false);
			_canvasNotification.SetActive(false);
		}
		
		if (id == 2) 
		{
			_canvasNotification.SetActive(true);
			_collectiblesNotification.SetActive(true);
			
			yield return new WaitForSeconds(3f);
			
			_collectiblesNotification.SetActive(false);
			_canvasNotification.SetActive(false);
		}
	}
	

	public void Start()
	{
		_mainAS = GameObject.FindGameObjectWithTag("MainAS").GetComponent<AudioSource>();
		if (PlayerPrefs.GetInt("Coins") <= 14) 
		{
			_coinAmount = 100;
		}
		
		else 
		{
			_coinAmount = PlayerPrefs.GetInt("Coins");
		}
		
		_collected = PlayerPrefs.GetInt("Collected");
		
		_coinAmountTMP.text = _coinAmount.ToString();
		_coinAmountBackTMP.text = _coinAmount.ToString();
		
		_collectedTMP.text = _collected.ToString();
		_collectedBackTMP.text = _collected.ToString();
	}
	public void SpawnFarm()
	{
		ClearQueueSpawn();
		
		if ((id == 1) || (id == 2)) 
		{
			if (_farmsAmount < _maxFarmsAmount)
				{
					ToQueueSpawn(_farms[id - 1]);
					_placementManager.buttonPressed = true;
				}
		}
		
		else if (id != 0)
		{
			if (!_generatorPlaced) 
			{
				ToQueueSpawn(_farms[id - 1]);
				_placementManager.buttonPressed = true;
				_generatorPlaced = true;
			}
		}
	}
	public void ToQueueSpawn(GameObject _prefab)
	{
		_placementManager._prefabsToSpawn.Add(_prefab);
	}
	
	public void PlayClick() 
	{
		_mainAS.clip = _click;
		_mainAS.Play();
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
