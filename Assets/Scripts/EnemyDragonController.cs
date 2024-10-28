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
	private bool isNear = true;
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
				lookAt = _game._currentDragon.transform.position;
				lookAt.y = transform.position.y;
				transform.LookAt(lookAt);
				if (distance <= _attackRange && !isNear)
				{
					isNear = true;
					StartCoroutine(Attack());
				}
				else if (distance > _attackRange && isNear)
				{
					isNear = false;
					StartCoroutine(FlyToTarget());
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
			// any visualization of losing hp (effect)
			if (_hp <= 0)
			{
				_animator.SetBool("IsDie", true);
				StartCoroutine(_game.Kill(gameObject));
			}
		}
	}
	public IEnumerator Attack()
	{
		while (isNear)
		{
			Turn(_game._currentDragon.transform.position);
			_animator.SetInteger("AttackState", Random.Range(1,countOfAttacks));
			Debug.Log("enemy random attack switched");
			switch (_animator.GetInteger("AttackState"))
			{
				case 1:
					StartCoroutine(SpawnFireball());
					break;
				case 2:
					StartCoroutine(SpawnFireball());
					break;
				case 3:
					StartCoroutine(ComeCloser());
					break;
				case 4:
					StartCoroutine(ComeCloser());
					break;
			}
			
			yield return new WaitForSecondsRealtime(0.1f);
			_animator.SetInteger("AttackState", 0);
			Debug.Log("AttackState enemy = 0");
			yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.2f));
		}
		StartCoroutine(FlyToTarget());
	}
	public IEnumerator SpawnFireball()
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
		_animator.SetBool("IsFlying", true);
		_animator.SetBool("IsLanding", false);
		while (distance > _attackRange)
		{
			lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.LookAt(lookAt);
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return null;
		}
		isNear = true;
		_animator.SetBool("IsFlying", false);
		_animator.SetBool("IsLanding", true);
		StartCoroutine(DistanceCheck());
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
		_game.needToFight = true;
		_game._enemyDragon = gameObject;
		_game._currentDragon.GetComponent<DragonBehaviour>().Turn(transform.position);
		distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		StartCoroutine(DistanceCheck());
	}
	private IEnumerator ComeCloser()
	{
		Vector3 nextLookAt = transform.position;
		lookAt = _game._currentDragon.transform.position;
		lookAt.y = transform.position.y;
		transform.LookAt(lookAt);
		yield return new WaitForSeconds(0.5f);
		distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		while (distance + 0.1f > _attackRange)
		{
			transform.position = Vector3.MoveTowards(transform.position, lookAt, 0.3f * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		}
	}
}