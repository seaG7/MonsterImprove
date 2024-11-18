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
		isFollowing = false;
		buttonPressed = false;
		
		if (_object != null)
			_object.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
	}
}
