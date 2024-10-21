using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingBehaviour : MonoBehaviour
{
	PlacementManager _placementManager;
	Color _color;
	Color _placingColor = new Color(1,1,1,1);
	void Start()
	{
		_color = GetComponent<Renderer>().material.color;
		StartCoroutine(WaitForConfirm());
	}

	private IEnumerator WaitForConfirm()
	{
		GetComponent<Renderer>().material.color = _placingColor;
		while (!_placementManager.isDragged)
		{
			yield return null;
		}
		GetComponent<Renderer>().material.color = _color;
	}
}
