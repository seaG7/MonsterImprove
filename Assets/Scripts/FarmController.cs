using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FarmController : MonoBehaviour
{
	[SerializeField] private GameObject[] _taverns;
	[SerializeField] private GameObject[] _buttonsUI;
	[SerializeField] private GameObject _settings;
	[SerializeField] private GameObject _information;
	[SerializeField] private GameObject _pinchTransform;
	[SerializeField] public TextMeshProUGUI _harvestAmountTMP;
	[SerializeField] public TextMeshProUGUI _harvestAmountBackTMP;
	public GameController _gameController;

	public int _maxHarvestAmount = 30;
	public int _harvestAmount;
	
	
	public void Awake()
	{
		
		_gameController = FindFirstObjectByType<GameController>();
	}

    public void OpenCloseMenu(int id) 
	{
		UpdateUI();
		
		_gameController._mainAS.clip = _gameController._click;
		
		if (id == 0) 
		{
			_settings.SetActive(!_settings.activeSelf);
			_information.SetActive(false);
			_pinchTransform.SetActive(!_pinchTransform.activeSelf);
		}
		
		if (id == 1) 
		{
			_settings.SetActive(false);
			_information.SetActive(!_information.activeSelf);
			_pinchTransform.SetActive(!_pinchTransform.activeSelf);
		}
		
		if (_information.activeSelf || _settings.activeSelf) 
		{
			_pinchTransform.SetActive(false);
		}

		_gameController._mainAS.clip = _gameController._click;
		_gameController._mainAS.Play();
	}
	
	public void UpdateUI() 
	{
		// _coinAmountTMP.text = GameController._coinAmount.ToString();
		_harvestAmountTMP.text = _harvestAmount.ToString() + "/" + _maxHarvestAmount.ToString();
		_harvestAmountBackTMP.text = _harvestAmount.ToString() + "/" + _maxHarvestAmount.ToString();
	}
	
	public void BuyButton(int _tavernID) 
	{
		if (GameController._coinAmount >= 15) 
		{
			GameController._coinAmount -= 15;
			
			GameController.SaveData();
			
			_gameController._coinAmountTMP.text = GameController._coinAmount.ToString();
			_gameController._coinAmountBackTMP.text = GameController._coinAmount.ToString();
			
			TavernController _tavernC = _taverns[_tavernID].GetComponent<TavernController>();
		
			_tavernC.GetAnimal().SetActive(true);
			_buttonsUI[_tavernID].SetActive(false);
			
			_gameController._mainAS.clip = _gameController._buySomething;
			_gameController._mainAS.Play();

			StartCoroutine(_tavernC.LifeCycle());
		}
		
		else 
		{
			StartCoroutine(_gameController._showNotification(0));
			_gameController._mainAS.clip = _gameController._error;
			_gameController._mainAS.Play();
		}
	}
	
	public void SellButton() 
	{
		if (_harvestAmount > 0) 
		{
			_harvestAmount--;

			GameController._coinAmount += 5;
			_gameController._mainAS.clip = _gameController._sellSomething;
			_gameController._mainAS.Play();
			
			GameController.SaveData();
			
			_harvestAmountTMP.text = _harvestAmount.ToString() + "/" + _maxHarvestAmount.ToString();
			_harvestAmountBackTMP.text = _harvestAmount.ToString() + "/" + _maxHarvestAmount.ToString();
			
			_gameController._coinAmountTMP.text = GameController._coinAmount.ToString();
			_gameController._coinAmountBackTMP.text = GameController._coinAmount.ToString();
		}
		
		else 
		{
			StartCoroutine(_gameController._showNotification(2));
			_gameController._mainAS.clip = _gameController._error;
			_gameController._mainAS.Play();
		}
	}
}
