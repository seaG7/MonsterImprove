using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FarmController : MonoBehaviour
{
	[SerializeField] private GameObject[] _taverns;
	[SerializeField] private GameObject[] _buttonsUI;
	[SerializeField] private GameObject _settings;
	[SerializeField] private GameObject _pinchTransform;
	
	public void OpenCloseMenu() 
	{
		_settings.SetActive(!_settings.activeSelf);
		_pinchTransform.SetActive(!_pinchTransform.activeSelf);
	}
	
	public void BuyButton(int _tavernID) 
	{
		_taverns[_tavernID].GetComponent<TavernController>().GetAnimal().SetActive(true);
		_buttonsUI[_tavernID].SetActive(false);
	}
}
