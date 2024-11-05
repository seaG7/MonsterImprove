using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
public class DragonBehaviour : MonoBehaviour
{
	private GameController _game;
	private InventorySystem _inventory;
	public Animator _animator;
	// private List<StaticHandGesture> _gestures = new List<StaticHandGesture>(4);
	// [SerializeField] private GameObject _gesturesObject;
	[Header("Your dragon Stats")]
	[SerializeField] private int _id;
	[SerializeField] public int _level;
	[SerializeField] public int _xp;
	private int _currentLevelXp;
	private int _maxLevelXp;
	[SerializeField] public int _hp;
	[SerializeField] public float _speed;
	[SerializeField] public int _strength;
	private List<int> _levelsXp = new List<int>(6) { 10, 20, 35, 50, 100 };
	
	[Header("Fireball")]
	[SerializeField] public GameObject _fireball;
	private Vector3 _spawnFirePos;
	private List<GameObject> fireballs = new List<GameObject>();
	
	[Header("Points")]
	[SerializeField] public Transform[] pointsOfTarget;
	
	[Header("Value Setting")]
	[SerializeField] private int _minigameBallXP;
	[SerializeField] private float _dragonBattleWinMultiplier;
	private bool _isMovingToHand = false;
	void Start()
	{
		// StartCoroutine(Init());
		_inventory = FindAnyObjectByType<InventorySystem>();
		_game = FindAnyObjectByType<GameController>();
		_game._currentDragon = gameObject;
		_game._cdController = GetComponent<DragonBehaviour>();
		_animator = GetComponent<Animator>();
		Turn(FindAnyObjectByType<Camera>().transform.position);
		StartCoroutine(SetHatchingFalse());
		
		_inventory.LoadStats(_id);
		_maxLevelXp = CalculateMaxLevelXp();
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy")
		{
			_hp -= _game._enemyDragon.GetComponent<EnemyDragonController>()._strength;
			// any visualization of losing hp (effect)
			if (_hp <= 0)
			{
				_animator.SetBool("IsDie", true);
				StartCoroutine(_game.Kill(gameObject));
			}
		}
	}
	public IEnumerator SetHatchingFalse()
	{
		_animator.SetBool("IsHatching", true);
		yield return new WaitForSecondsRealtime(2f);
		_animator.SetBool("IsHatching", false);
		_animator.SetBool("IsInspecting", true);
		yield return new WaitForSecondsRealtime(3f);
		_animator.SetBool("IsInspecting", false);
	}
	public void GainXP(int amount)
	{
		_xp += amount;
		if (_currentLevelXp >= _maxLevelXp)
		{
			LevelUp();
		}
		else
		{
			_currentLevelXp = CalculateCurrentLevelXp();
			// instantiate effect for gain xp;
		}
		_inventory.SaveStats(_id);
	}
	private void LevelUp()
	{
		// instantiate effect for level up;
		_currentLevelXp = CalculateCurrentLevelXp();
		_maxLevelXp = CalculateMaxLevelXp();
		_level++;
	}
	private int CalculateCurrentLevelXp()
	{
		int xp = _xp;
		foreach (int amount in _levelsXp)
		{
			if (xp >= amount)
				xp -= amount;
			else
				break;
		}
		if (xp > _levelsXp[_levelsXp.Count])
			return _levelsXp[_levelsXp.Count];
		return xp;
	}
	private int CalculateMaxLevelXp()
	{
		int xp = _xp;
		foreach (int amount in _levelsXp)
		{
			if (xp >= amount)
				xp -= amount;
			else
				return amount;
		}
		return _levelsXp[_levelsXp.Count];
	}
	public IEnumerator FireballAttack()
	{
		_spawnFirePos = transform.Find("FireballPos").GetComponent<Transform>().position;
		yield return new WaitForSecondsRealtime(0.2f);
		fireballs.Add(Instantiate(_fireball, _spawnFirePos, Quaternion.identity));
		Debug.Log("player fireball spawned");
		yield return new WaitForSeconds(2f);
		if (fireballs.Count > 0)
		{
			Destroy(fireballs[0]);
			fireballs.RemoveAt(0);
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
		while (FindAnyObjectByType<PlacementManager>().isDragged)
		{
			yield return null;
		}
		_game = FindAnyObjectByType<GameController>();
		_game._currentDragon = gameObject;
		_game._cdController = GetComponent<DragonBehaviour>();
		_game._cdAnimator = _animator = GetComponent<Animator>();
		Turn(FindAnyObjectByType<Camera>().transform.position);
		StartCoroutine(SetHatchingFalse());
	}
	public void FlyIdleShoot()
	{
		Turn(_game._selectedTargets[0].transform.position);
		StartCoroutine(SetAttackState(10));
		StartCoroutine(FireballAttack());
	}
	public IEnumerator SetAttackState(int _number)
	{
		if (_animator.GetInteger("AttackState") == 0)
		{
			_animator.SetInteger("AttackState", _number);
			if (_number == 4)
				StartCoroutine(FireballAttack());
			if (_number != 0)
			{
				yield return new WaitForSeconds(0.2f);
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
		while (_game.isMiniGaming)
		{
			if (_game._selectedTargets.Count > 0)
			{
				FlyIdleShoot();
				yield return new WaitForSeconds(3f);
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
	}
	private IEnumerator Move(Vector3 _movePos)
	{
		while (transform.position != _movePos)
		{
			transform.position = Vector3.MoveTowards(transform.position, _movePos, _speed * Time.deltaTime);
			yield return null;
		}
	}
}
