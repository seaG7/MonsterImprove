using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EggBehaviour : MonoBehaviour
{
	[SerializeField] private float _hatchingTime;
	[SerializeField] private GameObject _dragon;
	GameController _game;
	Animator _animator;
	void Start()
	{
		StartCoroutine(Init());
	}
	void Update()
	{
		
	}
	private IEnumerator HatchingDragon()
	{
		Vector3 _spawnPos = transform.position;
		
		yield return new WaitForSecondsRealtime(_hatchingTime);
		_game._currentDragon = Instantiate(_dragon, _spawnPos, Quaternion.identity);
		
		_animator.SetInteger("Crack", 1);
		
		yield return new WaitForSecondsRealtime(2f);
		Destroy(gameObject);
	}
	private IEnumerator Init()
	{
		while (!FindAnyObjectByType<PlacementManager>().isDragged)
		{
			yield return null;
		}
		_animator = GetComponent<Animator>();
		_game = FindAnyObjectByType<GameController>();
		StartCoroutine(HatchingDragon());		
	}
}
