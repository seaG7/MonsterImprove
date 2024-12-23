using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class EnemyDragonBehaviour : MonoBehaviour
{
	private GameController _game;
	private MenuController _menuController;
	public DragonBehaviour _cdController;
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
	public Transform _canvasTransform;
	void Start()
	{
		StartCoroutine(Init());
	}
	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.gameObject.tag == "Player") && !_collisionDetected)
		{
			Effect(collision);
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
			yield return new WaitForSeconds(0.2f);
			_animator.SetInteger("AttackState", 0);
			distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			yield return new WaitForSeconds(Random.Range(1f, 4f));
		}
	}
	private void Effect(Collision collision)
	{
		Instantiate(_game._fightEffects[Random.Range(1, _game._fightEffects.Length)], collision.contacts[0].point, Quaternion.identity);
		if (!FindAnyObjectByType<MenuController>()._sounds.isPlaying)
		{
			FindAnyObjectByType<MenuController>()._sounds.PlayOneShot(FindAnyObjectByType<MenuController>()._sounds.clip);
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
		_cdController = FindAnyObjectByType<DragonBehaviour>();
		_cdController._edController = GetComponent<EnemyDragonBehaviour>();
		EnableCanvas();
		_cdController.EnableCanvas();
		_game = FindAnyObjectByType<GameController>();
		_menuController = FindAnyObjectByType<MenuController>();
		_animator = GetComponent<Animator>();
		_game._enemyDragon = gameObject;
		_game._enemyStrength = _strength;
		
		_game.needToFight = true;
		_cdController.needToTurn = true;
		StartCoroutine(_cdController.TurnInFight());
		StartCoroutine(Turn(_game._currentDragon.transform.position));
		StartCoroutine(FlyToTarget());
	}
	private void EnableCanvas()
	{
		_canvasTransform = transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Canvas");
		_canvasTransform.gameObject.SetActive(true);
		_hpSlider.maxValue = _hp;
		_hpSlider.value = _hp;
		_hpSlider.minValue = 0;
	}
	private void DisableCanvas()
	{
		if (_canvasTransform == null)
			_canvasTransform = transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Canvas");
		_canvasTransform.gameObject.SetActive(false);
	}
	public IEnumerator DealDamage()
	{
		if (isAttacking)
		{
			_collisionDetected = true;
			isAttacking = false;
			_cdController._hp -= _strength;
			_cdController._hpSlider.value = _cdController._hp;
			Debug.Log($"СD got damage, hp = {_cdController._hp}");
			if (_cdController._hp <= 0)
			{
				_cdController.DisableCanvas();
				DisableCanvas();
				
				_cdController._animator.SetBool("IsDie", true);
				StartCoroutine(_game.Kill(_game._currentDragon));
				StartCoroutine(_game.Kill(_game._enemyDragon));
				
				_menuController.cdIndex = -1;
				_menuController.edIndex = -1;
				_menuController.UpdateDragonsDisplay();
				_menuController.UpdateEnemyDragonsDisplay();
				_menuController.ExitMode();
			}
			yield return new WaitForSeconds(0.6f);
			_collisionDetected = false;
		}
		if (_cdController != null)
		{
			StopCoroutine(_cdController.ComeCloser());
			StartCoroutine(_cdController.ComeCloser());
		}
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