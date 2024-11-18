using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernController : MonoBehaviour
{
	[SerializeField] private GameObject[] _icons; // 0 - harvest, 1 - eat
	[SerializeField] private GameObject _animal;
	
	public GameObject GetAnimal() 
	{
		return _animal;
	}
}
