using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDragonBehaviour : MonoBehaviour
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
	[Header("XP Rewards")]
	[SerializeField] private int _xpByKill;
	void Start()
	{
		StartCoroutine(Init());
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Player")
		{
			_hp -= FindAnyObjectByType<InventorySystem>()._strength[_game._cdIndex];
			// any visualization of losing hp (effect)
			if (_hp <= 0)
			{
				_game.needToFight = false;
				FindAnyObjectByType<MenuController>().edIndex = -1;
				FindAnyObjectByType<InventorySystem>().GainXp(_game._cdIndex, _xpByKill);
				_animator.SetBool("IsDie", true);
				StartCoroutine(_game.Kill(gameObject));
			}
		}
	}
	public IEnumerator Attack()
	{
		while (true)
		{
			StartCoroutine(Turn(_game._currentDragon.transform.position));
			_game._cdController.needToTurn = true;
			_animator.SetInteger("AttackState", Random.Range(1,countOfAttacks+1));
			if (_animator.GetInteger("AttackState") == 4)
			{
				StartCoroutine(SpawnFireball());
			}
			yield return new WaitForSeconds(0.2f);
			_animator.SetInteger("AttackState", 0);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return new WaitForSeconds(Random.Range(0.5f, 1.2f));
		}
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
		}
	}
	public IEnumerator FlyToTarget()
	{
		_animator.SetInteger("FlyState", 2);
		StartCoroutine(Turn(_game._currentDragon.transform.position));
		distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		while (distance > _attackRange)
		{
			lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return null;
		}
		_animator.SetInteger("FlyState", 0);
		StartCoroutine(Attack());
	}
	public IEnumerator Turn(Vector3 lookAt)
	{
		Vector3 lookPos = lookAt - transform.position;
		lookPos.y = 0;
		while (transform.rotation != Quaternion.LookRotation(lookPos))
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), 4 * Time.deltaTime);
			yield return null;
		}
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
		_game._cdController.needToTurn = true;
		StartCoroutine(Turn(_game._currentDragon.transform.position));
		StartCoroutine(_game._cdController.TurnInFight());
		StartCoroutine(FlyToTarget());
	}
}