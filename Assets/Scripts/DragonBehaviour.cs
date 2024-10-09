using UnityEngine;
using UnityEngine.XR.Hands.Samples.GestureSample;
public class DragonBehaviour : MonoBehaviour
{
	[SerializeField] private StaticHandGesture _gestures;
	
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
		
	}
	public void FirstAttack()
	{
		if (_game.needToFight && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 1);
		}
	}
	public void SecondAttack()
	{
		if (_game.needToFight && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 2);
		}
	}
	public void ThirdAttack()
	{
		if (_game.needToFight && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 3);
		}
	}
	public void FourthAttack()
	{
		if (_game.needToFight && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 4);
		}
	}
}
