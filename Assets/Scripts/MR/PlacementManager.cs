using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
public class PlacementManager : MonoBehaviour
{
	private GameController _game;
	[SerializeField] private XRRayInteractor xrRayInteractor;
	[SerializeField] private List<PlaneClassification> targetPlaneClassifications = new List<PlaneClassification>();
	public List<GameObject> _prefabsToSpawn = new List<GameObject>(0);
	public List<GameObject> _spawnedPlaneObjects = new List<GameObject>(0);
	public bool isDragged = true;
	private GameObject _object;
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
	}
	public void GrabSpawnableObject()
	{
		if (_prefabsToSpawn.Count > 0 && isDragged)
		{
			StartCoroutine(DropToPlane());
		}
	}
	public void SpawnObject()
	{
		if (!isDragged)
		{
			StartCoroutine(ObjectPlacementConfirmation());
			_spawnedPlaneObjects.Add(_object);
		}
	}
	private IEnumerator DropToPlane()
	{
		bool inPlane = false;
		while (!inPlane)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _) && isDragged)
			{
				if (raycastHit.transform.TryGetComponent(out ARPlane arPlane) && targetPlaneClassifications.Contains(arPlane.classification))
				{
					inPlane = true;
					isDragged = false;
					_object = Instantiate(_prefabsToSpawn[0], raycastHit.point, Quaternion.identity);
					_prefabsToSpawn.RemoveAt(0);
					StartCoroutine(Drag());
				}
			}
			yield return null;
		}
	}
	private IEnumerator Drag()
	{
		while (!isDragged)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
			{
				if (raycastHit.transform.TryGetComponent(out ARPlane arPlane) && targetPlaneClassifications.Contains(arPlane.classification))
				{
					_object.transform.position = raycastHit.point;
				}
			}
			yield return null;
		}
	}
	public IEnumerator AimTarget()
	{
		while (_game.isMiniGaming)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
			{
				if (raycastHit.transform.gameObject.CompareTag("Target"))
				{
					_game._selectedTargets.Add(raycastHit.transform.gameObject);
				}
			}
			yield return null;
		}
	}
	private IEnumerator ObjectPlacementConfirmation()
	{
		Vector3 _localScale = _object.transform.localScale;
		while (_object.transform.localScale.x > _localScale.x*0.8f)
		{
			_object.transform.localScale = new Vector3(_object.transform.localScale.x*0.97f,_object.transform.localScale.y*0.97f,_object.transform.localScale.z*0.97f);
			yield return null;
		}
		while (_object.transform.localScale.x < _localScale.x)
		{
			_object.transform.localScale = new Vector3(_object.transform.localScale.x*1.03f,_object.transform.localScale.y*1.03f,_object.transform.localScale.z*1.03f);
			yield return null;
		}
		_object.transform.Find("SelectionVisualization").gameObject.SetActive(false);
		isDragged = true;
	}
}
