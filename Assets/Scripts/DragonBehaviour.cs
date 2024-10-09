using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;
using System.Transactions;
using UnityEngine.Rendering;

public class DragonBehaviour : MonoBehaviour
{

	private GameController _game;
	public Animator _animator;
	[SerializeField] public float _hp;
	[SerializeField] public float _speed;
	
	[SerializeField] public float _stamina;
	[SerializeField] public float _strength;
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
	}
	void Update()
	{
		if (_game.needToFight)
		{
			
		}
	}
	
}
