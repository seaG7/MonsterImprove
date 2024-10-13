using UnityEngine;
using System.Collections;
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
		if (_game.isHatching)
			StartCoroutine(SetHatchingFalse());
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		_hp -= _game._enemyDragon.GetComponent<EnemyDragonController>()._strength;
	}
	public IEnumerator SetHatchingFalse()
	{
		_animator.SetBool("IsHatching", true);
		yield return new WaitForSecondsRealtime(2f);
		_animator.SetBool("IsHatching", false);
	}
	public void FirstAttack()
	{
		if (_game.isFighting && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 1);
			Debug.Log("first attack activated by the gesture");
		}
	}
	public void SecondAttack()
	{
		if (_game.isFighting && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 2);
			Debug.Log("second attack activated by the gesture");
		}
	}
	public void ThirdAttack()
	{
		if (_game.isFighting && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 3);
			Debug.Log("third attack activated by the gesture");
		}
	}
	public void FourthAttack()
	{
		if (_game.isFighting && (_game._enemyDragon != null))
		{
			_animator.SetInteger("AttackState", 4);
			Debug.Log("fourth attack activated by the gesture");
		}
	}
	public void StopAttack()
	{
		_animator.SetInteger("AttackState", 0);
	}
}
