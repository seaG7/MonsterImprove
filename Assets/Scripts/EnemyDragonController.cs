using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDragonController : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	
	[Header("Enemy Dragon Stats")]
	[SerializeField] public int _hp;
	[SerializeField] public float _speed;
	[SerializeField] public int _strength;
	[SerializeField] public float _attackRange;
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Vector3 _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	private Vector3 lookAt;
	private float distance;
	private int countOfAttacks = 4;
	void Start()
	{
		StartCoroutine(Init());
	}
	void FixedUpdate()
	{
		
	}
	
	void Update()
	{
		
	}
	public IEnumerator DistanceCheck()
	{
		while (true)
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
			yield return null;
		}
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
			Turn(_game._currentDragon.transform.position);
			_animator.SetInteger("AttackState", Random.Range(1,countOfAttacks));
			Debug.Log("enemy random attack switched");
			if (_animator.GetInteger("AttackState") == 1)
			{
				StartCoroutine(FireballAttack());
			}
			Turn(_game._currentDragon.transform.position);
			yield return new WaitForSecondsRealtime(0.1f);
			_animator.SetInteger("AttackState", 0);
			Debug.Log("AttackState enemy = 0");
			yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.2f));
		}
		StartCoroutine(FlyToTarget());
	}
	public IEnumerator FireballAttack()
	{
		_spawnFirePos = transform.Find("FireballPos").GetComponent<Transform>().position;
		yield return new WaitForSecondsRealtime(0.2f);
		fireballs.Add(Instantiate(_fireball, _spawnFirePos, Quaternion.identity));
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
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed );
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return null;
		}
	}
	public void Turn(Vector3 lookAt)
	{
		Vector3 lookPos = lookAt - transform.position;
		lookPos.y = 0;
	   	transform.rotation = Quaternion.LookRotation(lookPos);
	}
	private IEnumerator Init()
	{
		while (!FindAnyObjectByType<PlacementManager>().isDragged)
		{
			yield return null;
		}
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
		_game._enemyDragon = gameObject;
		_game._currentDragon.GetComponent<DragonBehaviour>().Turn(transform.position);
		distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		StartCoroutine(DistanceCheck());
	}
}