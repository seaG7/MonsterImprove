using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DragonBehaviour : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	// private List<StaticHandGesture> _gestures = new List<StaticHandGesture>(4);
	// [SerializeField] private GameObject _gesturesObject;
	[Header("Your dragon Stats")]
	[SerializeField] public int _level;
	[SerializeField] public int _xp;
	[SerializeField] public float _hp;
	[SerializeField] public float _speed;
	[SerializeField] public float _strength;
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Transform _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
		if (_game.isHatching)
			StartCoroutine(SetHatchingFalse());
		// _gestures = _gesturesObject.transform.GetComponentsInChildren<StaticHandGesture>().ToList();
		// Debug.Log(_gestures);
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy")
		{
			_hp -= _game._enemyDragon.GetComponent<EnemyDragonController>()._strength;
		}
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
			StartCoroutine(FireballAttack());
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
	public IEnumerator FireballAttack()
	{
		_spawnFirePos = transform.Find("FireballPos").GetComponent<Transform>();
		yield return new WaitForSecondsRealtime(0.2f);
		fireballs.Add(Instantiate(_fireball, _spawnFirePos));
		Debug.Log("player fireball spawned");
		yield return new WaitForSecondsRealtime(2f);
		if (fireballs.Count > 0)
		{
			Destroy(fireballs[0]);
			fireballs.RemoveAt(0);
		}
	}
}
