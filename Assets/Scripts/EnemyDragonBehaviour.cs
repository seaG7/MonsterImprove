using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyDragonBehaviour : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	
	[Header("Enemy Dragon Stats")]
	[SerializeField] public int _hp;
	[SerializeField] public float _speed;
	[SerializeField] public int _strength;
	[SerializeField] public float _attackRange;
	private bool _collisionDetected = false;
	public bool isAttacking = false;
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Vector3 _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	private Vector3 lookAt;
	private float distance;
	private int countOfAttacks = 4;
	[Header("XP Rewards")]
	[SerializeField] public int _xpByKill;
	[SerializeField] public Slider _hpSlider;
	public Rigidbody _rb;
	void Start()
	{
		StartCoroutine(Init());
	}
	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.gameObject.tag == "Player") && !_collisionDetected)
		{
			if (isAttacking)
			{
				Instantiate(_game._fightEffects[Random.Range(1, _game._fightEffects.Length)], collision.contacts[0].point, Quaternion.identity);
				if (!FindAnyObjectByType<MenuController>()._sounds.isPlaying)
				{
					FindAnyObjectByType<MenuController>()._sounds.PlayOneShot(FindAnyObjectByType<MenuController>()._sounds.clip);
				}
			}
			StartCoroutine(DealDamage());
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !_collisionDetected)
		{
			StartCoroutine(DealDamage());
		}
	}
	public IEnumerator Attack()
	{
		int lastNumber = 0;
		while (_game.needToFight)
		{
			StartCoroutine(Turn(_game._currentDragon.transform.position));
			FindAnyObjectByType<DragonBehaviour>().needToTurn = true;
			int number = lastNumber;
			while (lastNumber == number)
				number = Random.Range(1,countOfAttacks+1);
			_animator.SetInteger("AttackState", number);
			lastNumber = number;
			isAttacking = true;
			// if (_animator.GetInteger("AttackState") == 4)
			// {
			// 	StartCoroutine(SpawnFireball());
			// }
			yield return new WaitForSeconds(0.2f);
			_animator.SetInteger("AttackState", 0);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return new WaitForSeconds(Random.Range(1f, 4f));
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
		FindAnyObjectByType<DragonBehaviour>()._hpSlider.maxValue = FindAnyObjectByType<InventorySystem>()._hp[FindAnyObjectByType<DragonBehaviour>()._id];
		_hpSlider.maxValue = _hp;
		transform.Find("Canvas").gameObject.SetActive(false);
		while (!FindAnyObjectByType<PlacementManager>().isDragged)
		{
			yield return null;
		}
		FindAnyObjectByType<DragonBehaviour>()._hpSlider.value = FindAnyObjectByType<InventorySystem>()._hp[FindAnyObjectByType<DragonBehaviour>()._id];
		_hpSlider.value = _hp;
		transform.Find("Canvas").gameObject.SetActive(true);
		FindAnyObjectByType<DragonBehaviour>().transform.Find("Canvas").gameObject.SetActive(true);
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
		_game.needToFight = true;
		_game._enemyDragon = gameObject;
		FindAnyObjectByType<DragonBehaviour>().needToTurn = true;
		FindAnyObjectByType<DragonBehaviour>().AnimGestureIcons();
		_game._enemyStrength = _strength;
		StartCoroutine(Turn(_game._currentDragon.transform.position));
		StartCoroutine(FindAnyObjectByType<DragonBehaviour>().TurnInFight());
		StartCoroutine(FlyToTarget());
		_rb = GetComponent<Rigidbody>();
	}
	public IEnumerator DealDamage()
	{
		if (isAttacking)
		{
			_collisionDetected = true;
			isAttacking = false;
			FindAnyObjectByType<DragonBehaviour>()._hp -= _strength;
			FindAnyObjectByType<DragonBehaviour>()._hpSlider.value = FindAnyObjectByType<DragonBehaviour>()._hp;
			Debug.Log($"СD got damage, hp = {FindAnyObjectByType<DragonBehaviour>()._hp}");
			if (FindAnyObjectByType<DragonBehaviour>()._hp <= 0)
			{
				FindAnyObjectByType<MenuController>().ResetChosenSprites(true);
				FindAnyObjectByType<DragonBehaviour>().EndAnimGestureIcons();
				FindAnyObjectByType<MenuController>().cdIndex = -1;
				FindAnyObjectByType<MenuController>().edIndex = -1;
				FindAnyObjectByType<DragonBehaviour>()._animator.SetBool("IsDie", true);
				StartCoroutine(_game.Kill(_game._currentDragon));
				StartCoroutine(_game.Kill(_game._enemyDragon));
				FindAnyObjectByType<MenuController>().ExitMode();
			}
			yield return new WaitForSeconds(0.6f);
			_collisionDetected = false;
		}
		StopCoroutine(FindAnyObjectByType<DragonBehaviour>().ComeCloser());
		StartCoroutine(FindAnyObjectByType<DragonBehaviour>().ComeCloser());
	}
	public IEnumerator ComeCloser()
	{
		float distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
		while (distance > _attackRange && _game.needToFight)
		{
			transform.position = Vector3.MoveTowards(transform.position, _game._currentDragon.transform.position, 0.3f * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return null;
		}
	}
}