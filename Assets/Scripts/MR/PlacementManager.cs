using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class PlacementManager : MonoBehaviour
{
	private GameController _game;
	[SerializeField] private XRRayInteractor xrRayInteractor;
	[SerializeField] private PlaneClassification targetPlaneClassification;
	[SerializeField] private GameObject _menuWindow;
	[SerializeField] private InputActionProperty _handPositionProperty;
	[SerializeField] private InputActionProperty _cameraPositionProperty;
	public List<GameObject> _prefabsToSpawn = new List<GameObject>(0);
	public List<GameObject> _spawnedPlaneObjects = new List<GameObject>(0);
	public bool isDragged = true;
	private bool _isMenuActive;
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
			isDragged = true;
			_object.transform.Find("SelectionVisualization").gameObject.SetActive(false);
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
	public IEnumerator AimTarget()
	{
		while (_game.isMiniGaming)
		{
			if (xrRayInteractor.enabled && xrRayInteractor.TryGetCurrent3DRaycastHit(out var raycastHit, out _))
			{
				if (raycastHit.transform.gameObject.CompareTag("Target") && !_game._selectedTargets.Contains(raycastHit.transform.gameObject))
				{
					if (_game._selectedTargets.Count == 0)
						_game._cdController.needToShoot = true;
					_game._selectedTargets.Add(raycastHit.transform.gameObject);
					Debug.Log("новая цель выбрана");
				}
			}
			yield return null;
		}
	}
	public void OpenMenu()
	{
		if (!_isMenuActive)
		{
			_isMenuActive = true;
			_menuWindow.SetActive(_isMenuActive);
			StartCoroutine(UpdateMenuPosition());
		}
	}
	public void CloseMenu()
	{
		_isMenuActive = false;
		_menuWindow.SetActive(_isMenuActive);
	}
	private IEnumerator UpdateMenuPosition()
	{
		while (_isMenuActive)
		{
			Vector3 _menuPosition = _handPositionProperty.action.ReadValue<Vector3>();
			_menuWindow.transform.LookAt(_cameraPositionProperty.action.ReadValue<Vector3>());
			_menuWindow.transform.localRotation = Quaternion.Euler(0, 180, 0);
			_menuWindow.transform.position = _menuPosition;
			yield return null;
		}
	}
}
