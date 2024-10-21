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
	[SerializeField] private PlaneClassification targetPlaneClassification;
	public List<GameObject> _prefabsToSpawn = new List<GameObject>(0);
	public List<GameObject> _spawnedPlaneObjects = new List<GameObject>(0);
	public bool isDragged = true;
	private bool pointAtPerformed = false;
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
		isDragged = true;
		_spawnedPlaneObjects.Add(_object);
	}
	private IEnumerator DropToPlane()
	{
		bool inPlane = false;
		while (!inPlane)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _) && isDragged)
			{
				if (raycastHit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
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
				if (raycastHit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
				{
					_object.transform.position = raycastHit.point;
				}
			}
			yield return null;
		}
	}
	private IEnumerator AimTarget()
	{
		while (_game.isMiniGaming)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
			{
				if (raycastHit.transform.gameObject.CompareTag("Target"))
				{
					Destroy(raycastHit.transform.gameObject);
				}
			}
			yield return null;
		}
		pointAtPerformed = false;
	}
	public void StartAimCheck()
	{
		if (!pointAtPerformed && _game.isMiniGaming)
			StartCoroutine(AimTarget());
		pointAtPerformed = true;
	}
	void Update()
	{
		
	}
}
