using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameController : MonoBehaviour
{
	[SerializeField] private PlacementManager _placementManager;
	[SerializeField] GameObject[] _camera;
	public GameObject _objectToPlace;
	public GameObject _currentDragon;
	public GameObject _enemyDragon;
	public bool _cameraSpawned = false;
	public InputActionProperty _playerPosition;
	[SerializeField] public GameObject _ghostPrefab;
	
	public int _currentGhostOnMapAmount;
	public int _maxGhostOnMapAmount = 1;
	[SerializeField] public AudioClip _spawnEffect;
	[SerializeField] public AudioSource _effectsAS;
	[SerializeField] private PlaneClassification targetPlaneClassification;
	public bool _boardSpawned = false;
	[SerializeField] private GameObject _boardPrefab;
	
	void Start()
	{
		StartCoroutine(GhostSpawn());
	}
	
	void Update()
	{
		if (!_boardSpawned)
			{
				Vector3 _newBoardPosition;
				Vector3 _playerPos = _playerPosition.action.ReadValue<Vector3>();
				
				_newBoardPosition.x = _playerPos.x + Random.Range(-1f, 1f);
				_newBoardPosition.y = _playerPos.y;
				_newBoardPosition.z = _playerPos.z + Random.Range(-1f, 1f);
				
				if (Physics.Raycast(new Ray(_newBoardPosition, Vector3.down), out var hit))
				{
					if (hit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
					{
						var _board = Instantiate(_boardPrefab);
					
						_board.transform.position = new Vector3(_newBoardPosition.x, hit.point.y, _newBoardPosition.z);
						_boardSpawned = true;
					}
				}
			}
	}
	
	public IEnumerator GhostSpawn() 
	{
		while (true) 
		{
			yield return new WaitForSeconds(Random.Range(10f, 25f));
			
			if (_currentGhostOnMapAmount < _maxGhostOnMapAmount)
			{
				Vector3 _newGhostPosition;
				Vector3 _playerPos = _playerPosition.action.ReadValue<Vector3>();
				
				_newGhostPosition.x = _playerPos.x + Random.Range(-1f, 1f);
				_newGhostPosition.y = _playerPos.y - 1.35f;
				_newGhostPosition.z = _playerPos.z + Random.Range(-1f, 1f);
				
				while (Physics.Raycast(new Ray(_newGhostPosition, Vector3.down), out var hit))
				{
					if (hit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
					{
						var _newGhost = Instantiate(_ghostPrefab);
					
						_newGhost.transform.position = _newGhostPosition;
						
						_newGhost.GetComponent<GhostController>()._AS.clip = _newGhost.GetComponent<GhostController>()._spawn;
						_newGhost.GetComponent<GhostController>()._AS.Play();
						_currentGhostOnMapAmount++;
						break;
					}
					
					else 
					{
						float randomNumber = Random.Range(-1f, 1f) > 0.3f ? Random.Range(0.5f, 1f) : Random.Range(-1.5f, -0.5f);
						
						_newGhostPosition.x = _playerPos.x + randomNumber;
						_newGhostPosition.y = _playerPos.y - 1.35f;
						_newGhostPosition.z = _playerPos.z + randomNumber;
					}
				}
			}
		}
	}
	
	public void SpawnCamera()
	{
		if (!_cameraSpawned)
		{
			ClearQueueSpawn();
			ToQueueSpawn(_camera[0]);
		}
	}
	public Vector3 RandomPosAround()
	{
		Vector3 _startPos = _currentDragon.transform.position;
		Vector3 _spawnPos = _startPos;
		_spawnPos.x += Random.Range(-1f, 1f);
		_spawnPos.z += Random.Range(-1f, 1f);
		_spawnPos.y += Random.Range(0.25f, 1.2f);
		
		return _spawnPos;
	}
	
	public void ToQueueSpawn(GameObject _prefab)
	{
		_placementManager._prefabsToSpawn.Add(_prefab);
	}
	public void ClearQueueSpawn()
	{
		_placementManager._prefabsToSpawn.Clear();
	}
}