using UnityEngine;
using TMPro;
using System;

public class FoodGeneratorController : MonoBehaviour
{
	[SerializeField] private GameObject _pinchTransform;
	[SerializeField] public TextMeshProUGUI _foodAmountTMP;
	[SerializeField] public TextMeshProUGUI _foodAmountBackTMP;
	public static event Action ActionGeneratorSpawned;
	public GameController _gameController;
	
	public int _maxFoodAmount = 30;
	public int _foodAmount = 0;
	
	[SerializeField] public GameObject _foodMenu;
	
	public void Awake()
	{
		ActionGeneratorSpawned?.Invoke();
		_gameController = FindFirstObjectByType<GameController>();
	}
	
	public void OpenCloseMenu() 
	{
		UpdateUI();
		
		_gameController._mainAS.clip = _gameController._click;
		
		_pinchTransform.SetActive(!_pinchTransform.activeSelf);
		_foodMenu.SetActive(!_foodMenu.activeSelf);
		
		_gameController._mainAS.Play();
	}
	
	public void UpdateUI() 
	{
		// _coinAmountTMP.text = GameController._coinAmount.ToString();
		_foodAmountTMP.text = _foodAmount.ToString() + "/" + _maxFoodAmount.ToString();
		_foodAmountBackTMP.text = _foodAmount.ToString() + "/" + _maxFoodAmount.ToString();
	}
	
	public void BuyButton() 
	{
		if (GameController._coinAmount >= 5) 
		{
			GameController._coinAmount -= 5;
			_foodAmount++;
			
			GameController.SaveData();
			
			_gameController._coinAmountTMP.text = GameController._coinAmount.ToString();
			_gameController._coinAmountBackTMP.text = GameController._coinAmount.ToString();
			
			UpdateUI();
			
			_gameController._mainAS.clip = _gameController._buySomething;
			_gameController._mainAS.Play();
		}
		
		else 
		{
			StartCoroutine(_gameController._showNotification(0));
			_gameController._mainAS.clip = _gameController._error;
			_gameController._mainAS.Play();
		}
		
		GameController.SaveData();
	}
}
