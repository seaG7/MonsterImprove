using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class GhostController : MonoBehaviour
{
	private MeshRenderer _meshRenderer;
	public bool _isInvisible = true;
	void Start()
	{
		_meshRenderer = GetComponent<MeshRenderer>();	
	}
	
	void Update()
	{
		_meshRenderer.enabled = _isInvisible ? false : true;
		
		if (FindAnyObjectByType<HuntingController>() == null) 
		{
			_isInvisible = true;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Cone") 
		{
			_isInvisible = false;
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Cone") 
		{
			_isInvisible = true;
		}
	}
}