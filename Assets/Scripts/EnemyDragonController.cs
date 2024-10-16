using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDragonController : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	
	[Header("Enemy Dragon Stats")]
	[SerializeField] public float _hp;
	[SerializeField] public float _speed;
	[SerializeField] public float _strength;
	[SerializeField] public float _attackRange;
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Transform _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	private Vector3 lookAt;
	private float distance;
	private int countOfAttacks = 4;
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
		distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
	}
	void FixedUpdate()
	{
		if (_game._currentDragon != null)
		{
			// if (_hp <= 0)
			// {
			// 	_animator.SetBool("IsDie", true);
			// }
			
			lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.LookAt(lookAt);
			if (distance <= _attackRange)
			{
				if (!_game.needToFight)
					{
						_game.needToFight = true;
						_animator.SetBool("IsFlying", false);
						_animator.SetBool("IsLanding", true);
						StartCoroutine(Attack());
					}
			}
			else if (distance > _attackRange)
			{
				StartCoroutine(FlyToTarget());
				_game.needToFight = false;
			}
		}
	}
	
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Player")
		{
			_hp -= _game._currentDragon.GetComponent<DragonBehaviour>()._strength;
		}
	}
	public IEnumerator Attack()
	{
		while (_game.needToFight)
		{
			_animator.SetInteger("AttackState", Random.Range(1,countOfAttacks));
			Debug.Log("enemy random attack switched");
			if (_animator.GetInteger("AttackState") == 1)
			{
				StartCoroutine(FireballAttack());
			}
			yield return new WaitForSecondsRealtime(0.1f);
			_animator.SetInteger("AttackState", 0);
			Debug.Log("AttackState enemy = 0");
			yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.2f));
		}
		StartCoroutine(FlyToTarget());
	}
	public IEnumerator FireballAttack()
	{
		_spawnFirePos = transform.Find("FireballPos").GetComponent<Transform>();
		yield return new WaitForSecondsRealtime(0.2f);
		fireballs.Add(Instantiate(_fireball, _spawnFirePos));
		Debug.Log("enemy fireball spawned");
		yield return new WaitForSecondsRealtime(2f);
		if (fireballs.Count > 0)
		{
			Destroy(fireballs[0]);
			fireballs.RemoveAt(0);
		}
	}
	public IEnumerator FlyToTarget()
	{
		while (distance > _attackRange)
		{
			lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.LookAt(lookAt);
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return null;
		}
	}
}