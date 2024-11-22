using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GestureController : MonoBehaviour
{
	[SerializeField] private GameObject _hand;
	[SerializeField] private GameObject _toChange;
	[SerializeField] private bool _isGun;
	private HuntingController _currentHC;
	
	public void Start()
	{
		_currentHC = FindAnyObjectByType<HuntingController>();
	}
	
	public void GestureEnded() 
	{
		StartCoroutine(GestureEndCD());
	}
	
	public void GunGo() 
	{
		GunController _gunController = null;
		
		_gunController= FindFirstObjectByType<GunController>();
		
		StartCoroutine(_gunController.ShootAndReload());
	}
	
	public IEnumerator GestureEndCD() 
	{
		yield return new WaitForSeconds(0.2f);
		
		_hand.SetActive(true);
		_toChange.SetActive(false);
		
		if (!_isGun) 
		{
			_currentHC.SFXStop();
		}
	}
	
	
}
