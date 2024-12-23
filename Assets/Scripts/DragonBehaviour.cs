using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
public class DragonBehaviour : MonoBehaviour
{
	private GameController _game;
	private InventorySystem _inventory;
	private MenuController _menuController;
	public EnemyDragonBehaviour _edController;
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
	[SerializeField] public Transform _pointEffect;
	[SerializeField] public Slider _hpSlider;
	public Transform _canvasTransform;
	
	[Header("Value Setting")]
	[SerializeField] private int _minigameBallXP;
	[SerializeField] private float _dragonBattleWinMultiplier;
	private bool _isMovingToHand = false;
	public Vector3 _moveRot;
	public Vector3 _startPos;
	public bool needToShoot = true;
	public bool needToTurn = false;
	public bool isAttacking = false;
	void Start()
	{
		StartCoroutine(Init());
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if ((collision.gameObject.tag == "Enemy") && !_collisionDetected)
		{
			Effect(collision);
			StartCoroutine(DealDamage());
		}
	}
	public IEnumerator SetHatchingFalse()
	{
		_animator.SetBool("IsLevelUp", true);
		_inventory.GainXp(_id, 10);
		yield return new WaitForSeconds(2f);
		_animator.SetBool("IsLevelUp", false);
		_animator.SetBool("IsInspect", true);
		yield return new WaitForSeconds(3f);
		_animator.SetBool("IsInspect", false);
	}
	private IEnumerator Inspect()
	{	
		if (_inventory._xp[_id] == 0)
		{
			_inventory.GainXp(_id, 10);
		}
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
		float elapsedTime = 0f;
		while (elapsedTime < 0.5f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, 5 * Time.deltaTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		needToTurn = true;
	}
	private IEnumerator Init()
	{
		_inventory = FindAnyObjectByType<InventorySystem>();
		_game = FindAnyObjectByType<GameController>();
		if (_game._cdIndex != -1 && !_game.isSwitching)
			Destroy(_game._currentDragon);
		_animator = GetComponent<Animator>();
		_game._cdController = GetComponent<DragonBehaviour>();
		if (!_game.isSwitching)
		{
			while (!FindAnyObjectByType<PlacementManager>().isDragged)
			{
				yield return null;
			}
			if (_inventory._xp[_id] == 0)
			{
				transform.Find("SelectionVisualization").gameObject.SetActive(false);
				_inventory.GainXp(_id, 10);
			}
			else
			{
				StartCoroutine(Inspect());
			}
		}
		if (_game.isSwitching)
		{
			transform.Find("SelectionVisualization").gameObject.SetActive(false);
			StartCoroutine(LevelUp());
			_game.isSwitching = false;
		}
		_menuController = FindAnyObjectByType<MenuController>();
		_hp = _inventory._hp[_id];
		_game._currentDragon = gameObject;
		_game._cdIndex = _id;
		StartCoroutine(Turn(FindAnyObjectByType<Camera>().transform.position));
		_menuController._mainMenuButtons[2].gameObject.SetActive(false);
	}
	public void EnableCanvas()
	{
		_canvasTransform = transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Canvas");
		_canvasTransform.gameObject.SetActive(true);
		_hpSlider.maxValue = _hp;
		_hpSlider.value = _hp;
		_hpSlider.minValue = 0;
	}
	public void DisableCanvas()
	{
		if (_canvasTransform == null)
			_canvasTransform = transform.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Canvas");
		_canvasTransform.gameObject.SetActive(false);
	}
	private void Effect(Collision collision)
	{
		if (isAttacking)
		{
			Instantiate(_game._fightEffects[Random.Range(1,_game._fightEffects.Length)], collision.contacts[0].point, Quaternion.identity);
			if (!FindAnyObjectByType<MenuController>()._sounds.isPlaying)
			{
					FindAnyObjectByType<MenuController>()._sounds.PlayOneShot(FindAnyObjectByType<MenuController>()._sounds.clip);
			}
		}
	}
	public IEnumerator LevelUp()
	{
		_animator.SetBool("IsLevelUp", true);
		yield return new WaitForSeconds(1.5f);
		Vector3 _effectPos = transform.position;
		_effectPos.y += 0.2f;
		Instantiate(_game._levelUpEffect, _effectPos, Quaternion.identity);
		_animator.SetBool("IsLevelUp", false);
		if (_inventory._xp[_id] == 10)
		{
			StartCoroutine(Inspect());
		}
	}
	public void FlyIdleShoot()
	{
		StartCoroutine(SetAttackState(10));
		StartCoroutine(SpawnFireball());
	}
	public IEnumerator SetAttackState(int number)
	{
		if (_animator.GetInteger("AttackState") == 0)
		{
			isAttacking = true;
			_animator.SetInteger("AttackState", number);
			if (number != 0)
			{
				yield return new WaitForSeconds(0.1f);
				_animator.SetInteger("AttackState", 0);
			}
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
		needToShoot = true;
		Debug.Log("Взлетел");
		while (_game.isMiniGaming)
		{
			if (_game._selectedTargets.Count > 0 && needToShoot)
			{
				Debug.Log("Нашёл цель и начал поворот");
				needToShoot = false;
				StartCoroutine(Turn(_game._selectedTargets[0].transform.position));
				yield return new WaitForSeconds(0.5f);
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

		yield return new WaitForSeconds(1f);
		_inventory.GainXp(_id, 10);
		_game._targetsCount = 0;
	}
	public IEnumerator ShootDelay()
	{
		yield return new WaitForSeconds(1.5f);
		needToShoot = true;
	}
	public IEnumerator DealDamage()
	{
		if (isAttacking)
		{
			if (_game._enemyDragon != null)
				StartCoroutine(Turn(_game._enemyDragon.transform.position));
			_collisionDetected = true;
			isAttacking = false;
			_edController._hp -= _inventory._strength[_id];
			_edController._hpSlider.value = _edController._hp;
			Debug.Log($"ED got damage, hp = {_edController._hp}");
			if (_edController._hp <= 0)
			{
				_edController._animator.SetBool("IsDie", true);
				
				DisableCanvas();
				_menuController.ExitMode();
				
				_menuController.edIndex = -1;
				_inventory._kills++;
				_hp = _inventory._hp[_id];
				_inventory.GainXp(_id, _edController._xpByKill);
				StopCoroutine(DealDamage());
			}
			
			yield return new WaitForSeconds(0.6f);
			_collisionDetected = false;
		}
		if (_menuController.edIndex != -1)
		{
			StopCoroutine(_edController.ComeCloser());
			StartCoroutine(_edController.ComeCloser());
		}
	}
	public IEnumerator ComeCloser()
	{
		float distance = Vector3.Distance(transform.position, _game._enemyDragon.transform.position);
		while (distance > _edController._attackRange && _game.needToFight)
		{
			transform.position = Vector3.MoveTowards(transform.position, _game._enemyDragon.transform.position, 0.05f * Time.deltaTime);
			distance = Vector3.Distance(transform.position, _game._enemyDragon.transform.position);
			yield return null;
		}
	}
	public void SpawnEffect(GameObject _effect)
	{
		Instantiate(_effect, _pointEffect);
	}
}
