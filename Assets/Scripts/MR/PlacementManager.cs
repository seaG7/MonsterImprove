using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
public class PlacementManager : MonoBehaviour
{
	private GameController _game;
	[SerializeField] private InputActionProperty positionAction;
	[SerializeField] private InputActionProperty rotationAction;
	[SerializeField] private XRRayInteractor xrRayInteractor;
	[SerializeField] private PlaneClassification targetPlaneClassification;
	public List<GameObject> _prefabsToSpawn = new List<GameObject>(0);
	public List<GameObject> _spawnedPlaneObjects = new List<GameObject>(0);
	public bool buttonPressed = false;
	public bool isFollowing = false;
	public bool gestureCompleted = false;
	private GameObject _object;
	private void Start()
	{
		_game = FindFirstObjectByType<GameController>();
	}
	
	void Update()
	{
		if (isFollowing) 
		{
			_object.transform.position = positionAction.action.ReadValue<Vector3>();
			_object.transform.rotation = rotationAction.action.ReadValue<Quaternion>();
			_object.transform.Rotate(180f, 180f, 0);
		}
	}
	
	public void PerformedSpawn() 
	{
		if (buttonPressed) 
		{
			if (_prefabsToSpawn.Count > 0) 
			{
				_object = Instantiate(_prefabsToSpawn[0]);
				_prefabsToSpawn.RemoveAt(0);
				
								
				isFollowing = true;
			}
		}
	}
	public void EndedSpawn() 
	{
		if (buttonPressed) 
		{
			isFollowing = false;
			buttonPressed = false;
			
			_game._mainAS.clip = _game._placeFarm;
			_game._mainAS.Play();
		
			if (_object != null) 
			{
				_object.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				
				if (Physics.Raycast(new Ray(_object.transform.position, -transform.up), out var hit)) 
				{
					if (hit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
						{
							_object.transform.position = hit.point;
							_object.transform.rotation = Quaternion.Euler(0, _object.transform.rotation.y, 0);
						}
				}
			}
			_game._farmsAmount++;
			
			_game._farmAmountTMP.text = _game._farmsAmount.ToString() + "/" + _game._maxFarmsAmount;
			_game._farmAmountBackTMP.text = _game._farmsAmount.ToString() + "/" + _game._maxFarmsAmount;
			
			_object = null;
		}
	}
}
