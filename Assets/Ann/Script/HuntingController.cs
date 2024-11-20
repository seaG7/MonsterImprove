using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HuntingController : MonoBehaviour
{
	[SerializeField] private AudioSource _flickeringAS;
	public bool _isWorking = false;
	
	public void SFXPlay() 
	{
		_flickeringAS.loop = true;
		_flickeringAS.Play();
	}
	
	public void SFXStop() 
	{
		_flickeringAS.loop = false;
		_flickeringAS.Stop();
	}
}
