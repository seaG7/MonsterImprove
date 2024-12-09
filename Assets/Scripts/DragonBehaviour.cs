using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class DragonBehaviour : MonoBehaviour
{
	private GameController _game;
	private InventorySystem _inventory;
	public Animator _animator;
	[Header("Your dragon Stats")]
	[SerializeField] public int _id;
	[SerializeField] public int _hp;
	[SerializeField] public float _speed;
	private bool _collisionDetected = false;
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Vector3 _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	
	[Header("Points")]
	[SerializeField] public Transform[] pointsOfTarget;
	[SerializeField] public Slider _hpSlider;
	[SerializeField] public GameObject[] _gestureIcons;
	
	[Header("Value Setting")]
	[SerializeField] private int _minigameBallXP;
	[SerializeField] private float _dragonBattleWinMultiplier;
	private bool _isMovingToHand = false;
	public Vector3 _moveRot;
	public Vector3 _startPos;
	public bool needToShoot = true;
	public bool needToTurn = false;
	public bool isAttacking = false;
	public Rigidbody _rb;
	void Start()
	{
		Init();
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.gameObject.tag == "Enemy") && !_collisionDetected)
		{
			if (isAttacking)
			{
				Instantiate(_game._fightEffects[Random.Range(1,_game._fightEffects.Length)], collision.contacts[0].point, Quaternion.identity);
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
		if (other.CompareTag("Enemy") && !_collisionDetected)
		{
			StartCoroutine(DealDamage());
		}
	}
	public IEnumerator SetHatchingFalse()
	{
		_animator.SetBool("IsLevelUp", true);
		_inventory.GainXp(_id, 5);
		yield return new WaitForSeconds(2f);
		_animator.SetBool("IsLevelUp", false);
		_animator.SetBool("IsInspect", true);
		yield return new WaitForSeconds(3f);
		_animator.SetBool("IsInspect", false);
	}
	private IEnumerator Inspect()
	{
		_animator.SetBool("IsInspect", true);
		yield return new WaitForSeconds(3f);
		_animator.SetBool("IsInspect", false);
	}
	public IEnumerator SpawnFireball()
	{
		_spawnFirePos = transform.Find("FireballPos").GetComponent<Transform>().position;
		yield return new WaitForSecondsRealtime(0.2f);
		fireballs.Add(Instantiate(_fireball, _spawnFirePos, Quaternion.identity));
		Debug.Log("player fireball spawned");
		yield return new WaitForSeconds(3f);
		if (fireballs.Count > 0)
		{
			Destroy(fireballs[0]);
		}
	}
	public IEnumerator Turn(Vector3 lookAt)
	{
		lookAt = lookAt - transform.position;
		Quaternion _targetRot = Quaternion.LookRotation(lookAt);
		_targetRot.x = transform.rotation.x;
		_targetRot.z = transform.rotation.z;
		while (transform.rotation != _targetRot)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, 5 * Time.deltaTime);
			yield return null;
		}
		needToTurn = true;
	}
	private void Init()
	{
		_inventory = FindAnyObjectByType<InventorySystem>();
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
		if (!_game.isSwitching)
		{
			while (!FindAnyObjectByType<PlacementManager>().isDragged) {}
			if (_inventory._xp[_id] == 0)
			{
				StartCoroutine(SetHatchingFalse());
			}
			else
			{
				StartCoroutine(Inspect());
			}
		}
		_hp = _inventory._hp[_id];
		Debug.Log(_hp);
		_game._cdIndex = _id;
		_game._cdController = GetComponent<DragonBehaviour>();
		StartCoroutine(Turn(FindAnyObjectByType<Camera>().transform.position));
		FindAnyObjectByType<MenuController>()._mainMenuButtons[2].gameObject.SetActive(true);
		_rb = GetComponent<Rigidbody>();
	}
	public void AnimGestureIcons()
	{
		_hpSlider.gameObject.SetActive(true);
		foreach (var icon in _gestureIcons)
		{
			icon.SetActive(true);
		}
		// Quaternion targetRotation = Quaternion.Euler(_gestureIcons[0].transform.rotation.eulerAngles.x, _gestureIcons[0].transform.rotation.eulerAngles.y, -20);
		// while ((_gestureIcons[0].transform.rotation.z > -20) && _game.needToFight)
		// {
		// 	transform.rotation = Quaternion.Slerp(_gestureIcons[0].transform.rotation, targetRotation, 3f);
		// 	transform.rotation = Quaternion.Slerp(_gestureIcons[1].transform.rotation, targetRotation, 3f);
		// 	yield return null;
		// }
		// while ((_gestureIcons[0].transform.rotation.z < -20) && _game.needToFight)
		// {
		// 	transform.rotation = Quaternion.Slerp(_gestureIcons[0].transform.rotation, targetRotation, 3f);
		// 	transform.rotation = Quaternion.Slerp(_gestureIcons[1].transform.rotation, targetRotation, 3f);
		// 	yield return null;
		// }
		// if (_game.needToFight)
		// 	StartCoroutine(AnimGestureIcons());
	}
	public void EndAnimGestureIcons()
	{
		foreach (var icon in _gestureIcons)
		{
			icon.SetActive(false);
		}
		transform.Find("Canvas").gameObject.SetActive(false);
		FindAnyObjectByType<EnemyDragonBehaviour>().transform.Find("Canvas").gameObject.SetActive(false);
	}
	public IEnumerator LevelUp()
	{
		_animator.SetBool("IsLevelUp", true);
		// instantiate level up effects (visual + sound)
		yield return new WaitForSeconds(1.5f);
		_animator.SetBool("IsLevelUp", false);
	}
	public void FlyIdleShoot()
	{
		StartCoroutine(Turn(_game._selectedTargets[0].transform.position));
		// StopCoroutine(FixShootingMinigame());
		StartCoroutine(SetAttackState(10));
		StartCoroutine(SpawnFireball());
	}
	private IEnumerator FixShootingMinigame()
	{
		yield return new WaitForSeconds(3f);
		if (!needToShoot && _game._selectedTargets.Count > 0)
		{
			FlyIdleShoot();
		}
	}
	public IEnumerator SetAttackState(int number)
	{
		if (_animator.GetInteger("AttackState") == 0)
		{
			_animator.SetInteger("AttackState", number);
			// if (number == 4)
			// 	StartCoroutine(SpawnFireball());
			if (number != 0)
			{
				yield return new WaitForSeconds(0.1f);
				_animator.SetInteger("AttackState", 0);
			}
			isAttacking = true;
		}
	}
	public IEnumerator MoveToHand()
	{
		if (!_isMovingToHand && Vector3.Distance(transform.position, GameObject.Find("Right Hand Interaction Visual").transform.position) > 0.3f)
		{
			_isMovingToHand = true;
			_animator.SetInteger("FlyState", 2);
			Vector3 _movePos = GameObject.Find("Right Hand Interaction Visual").transform.position;
			_movePos.y = transform.position.y;
			while (transform.position != _movePos)
			{
				transform.position = Vector3.MoveTowards(transform.position, _movePos, _speed * Time.deltaTime);
				yield return null;
			}
			_animator.SetInteger("FlyState", 1);
			_movePos = GameObject.Find("Right Hand Interaction Visual").transform.position;
			while (transform.position != _movePos)
			{
				transform.position = Vector3.MoveTowards(transform.position, _movePos, _speed * Time.deltaTime);
				yield return null;
			}
		}
		_isMovingToHand = false;
	}
	public IEnumerator DestroyTargets()
	{
		float _height = 1f;
		_animator.SetInteger("FlyState", 1);
		yield return new WaitForSeconds(0.7f);
		Vector3 _movePos = transform.position;
		_movePos.y += _height;
		while (transform.position != _movePos)
		{
			transform.position = Vector3.MoveTowards(transform.position, _movePos, _speed * Time.deltaTime);
			yield return null;
		}
		while (_game.isMiniGaming)
		{
			if (_game._selectedTargets.Count > 0 && needToShoot)
			{
				needToShoot = false;
				FlyIdleShoot();
				StartCoroutine(ShootDelay());
			}
			yield return null;
		}
		_movePos.y -= _height;
		while (transform.position != _movePos)
		{
			transform.position = Vector3.MoveTowards(transform.position, _movePos, _speed * Time.deltaTime);
			yield return null;
		}
		_animator.SetInteger("FlyState", 0);

		yield return new WaitForSeconds(0.5f);
		_inventory.GainXp(_id, 10);
		_game._targetsCount = 0;
	}
	public IEnumerator ShootDelay()
	{
		yield return new WaitForSeconds(0.7f);
		needToShoot = true;
	}
	public IEnumerator TurnInFight()
	{
		while (_game.needToFight && _hp > 0)
		{
			if (needToTurn)
			{
				needToTurn = false;
				StartCoroutine(Turn(_game._enemyDragon.transform.position));
			}
			yield return null;
		}
	}
	public IEnumerator DealDamage()
	{
		if (isAttacking)
		{
			_collisionDetected = true;
			isAttacking = false;
			FindAnyObjectByType<EnemyDragonBehaviour>()._hp -= _inventory._strength[_id];
			FindAnyObjectByType<EnemyDragonBehaviour>()._hpSlider.value = FindAnyObjectByType<EnemyDragonBehaviour>()._hp;
			Debug.Log($"ED got damage, hp = {FindAnyObjectByType<EnemyDragonBehaviour>()._hp}");
			if (FindAnyObjectByType<EnemyDragonBehaviour>()._hp <= 0)
			{
				FindAnyObjectByType<MenuController>().ResetChosenSprites(false);
				EndAnimGestureIcons();
				FindAnyObjectByType<MenuController>().edIndex = -1;
				FindAnyObjectByType<EnemyDragonBehaviour>()._animator.SetBool("IsDie", true);
				FindAnyObjectByType<InventorySystem>().GainXp(_id, FindAnyObjectByType<EnemyDragonBehaviour>()._xpByKill);
				StartCoroutine(_game.Kill(_game._enemyDragon));
				FindAnyObjectByType<MenuController>().ExitMode();
			}
			
			yield return new WaitForSeconds(0.6f);
			_collisionDetected = false;
		}
		StopCoroutine(FindAnyObjectByType<EnemyDragonBehaviour>().ComeCloser());
		StartCoroutine(FindAnyObjectByType<EnemyDragonBehaviour>().ComeCloser());
	}
	public IEnumerator ComeCloser()
	{
		float distance = Vector3.Distance(transform.position, _game._enemyDragon.transform.position);
		while (distance > FindAnyObjectByType<EnemyDragonBehaviour>()._attackRange && _game.needToFight)
		{
			transform.position = Vector3.MoveTowards(transform.position, _game._enemyDragon.transform.position, 0.05f * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._enemyDragon.transform.position);
			yield return null;
		}
	}
}
