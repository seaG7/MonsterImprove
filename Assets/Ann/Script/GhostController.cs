using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GhostController : MonoBehaviour
{
	private MeshRenderer _meshRenderer;
	public bool _isInvisible = true;
	[SerializeField] public GameController _GC;
	public InputActionProperty _playerPosition;
	public AudioSource _AS;
	[SerializeField] public AudioClip _whoosh;
	[SerializeField] public AudioClip _spawn;
	[SerializeField] public AudioClip _die;
	[SerializeField] private PlaneClassification targetPlaneClassification;
	
	void Start()
	{
		_meshRenderer = GetComponent<MeshRenderer>();	
		_AS = GetComponent<AudioSource>();
		_GC = FindAnyObjectByType<GhostController>().GetComponent<GameController>();
		StartCoroutine(GhostWalk());
	}
	
	void Update()
	{
		_meshRenderer.enabled = _isInvisible ? false : true;
		
		Vector3 _playerPos = _playerPosition.action.ReadValue<Vector3>();
		
		Vector3 targetPostition = new Vector3(_playerPos.x, 
									   transform.position.y, 
									   _playerPos.z ) ;
									   
		transform.LookAt( targetPostition ) ;
	}
	
	private IEnumerator GhostWalk() 
	{
		while (true) 
		{
			yield return new WaitForSeconds(5f);
		
			Vector3 _newGhostPosition = gameObject.transform.position;
			Vector3 _playerPos = _playerPosition.action.ReadValue<Vector3>();
			
			_newGhostPosition.x = _playerPos.x + Random.Range(-2f, 2f);
			_newGhostPosition.y = _playerPos.y - 1f;
			_newGhostPosition.z = _playerPos.z + Random.Range(-2f, 2f);
			
			while (Physics.Raycast(new Ray(_newGhostPosition, Vector3.down), out var hit)) 
			{
				if (hit.transform.TryGetComponent(out ARPlane arPlane) && (arPlane.classification & targetPlaneClassification) != 0)
				{
					gameObject.transform.position = _newGhostPosition;
					_AS.clip = _whoosh;
					_AS.Play();
					
					break;
				}
				
				else 
				{
					_newGhostPosition.x = _playerPos.x + Random.Range(-1f, 1f);
					_newGhostPosition.y = _playerPos.y - 1f;
					_newGhostPosition.z = _playerPos.z + Random.Range(-1f, 1f);
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Cone") 
		{
			_isInvisible = false;
		}
		
		if (other.gameObject.tag == "KillCollider") 
		{
			AudioSource _SFXAS = GameObject.FindGameObjectWithTag("SFXAS").GetComponent<AudioSource>();
			_SFXAS.clip = _die;
			_SFXAS.Play();
			Destroy(gameObject);
			_GC._currentGhostOnMapAmount--;
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