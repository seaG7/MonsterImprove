using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacingBehaviour : MonoBehaviour
{
	PlacementManager _placementManager;
	Material _material;
	[SerializeField] Material _placingMaterial;
	void Start()
	{
		_material = GetComponent<SkinnedMeshRenderer>().materials.First();
		StartCoroutine(WaitForConfirm());
	}

	private IEnumerator WaitForConfirm()
	{
		GetComponent<SkinnedMeshRenderer>().material = _placingMaterial;
		while (!_placementManager.isDragged)
		{
			yield return null;
		}
		GetComponent<SkinnedMeshRenderer>().material = _material;
	}
}
