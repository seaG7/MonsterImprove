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
	private AudioSource _mainAS;
	[SerializeField] private TextMeshProUGUI _coinAmount;
	private GameController _gameController;
	[SerializeField] private AudioClip _sellSomething;
	[SerializeField] private AudioClip _buySomething;
	
	public void Awake()
	{
		_mainAS = GameObject.FindGameObjectWithTag("MainAS").GetComponent<AudioSource>();
		_gameController = FindFirstObjectByType<GameController>();
	}
	
	public void OpenCloseMenu(int id) 
	{
		UpdateUI();
		
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
		
		
	}
	
	public void UpdateUI() 
	{
		_coinAmount.text = GameController._coinAmount.ToString();
	}
	
	public void BuyButton(int _tavernID) 
	{
		_taverns[_tavernID].GetComponent<TavernController>().GetAnimal().SetActive(true);
		_buttonsUI[_tavernID].SetActive(false);
		
		_mainAS.clip = _buySomething;
		_mainAS.Play();
	}
}
