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
	void Start()
	{
		// StartCoroutine(Init());
		_inventory = FindAnyObjectByType<InventorySystem>();
		_game = FindAnyObjectByType<GameController>();
		_game._currentDragon = gameObject;
		_game._dragonController = GetComponent<DragonBehaviour>();
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
	private void GainXP(int amount)
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
		yield return new WaitForSecondsRealtime(2f);
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
		_game._dragonController = GetComponent<DragonBehaviour>();
		_animator = GetComponent<Animator>();
		Turn(FindAnyObjectByType<Camera>().transform.position);
		StartCoroutine(SetHatchingFalse());
	}
	public IEnumerator ShootBall()
	{
		if (_game.isMiniGaming)
		{
			_animator.SetInteger("AttackState", 1);
			StartCoroutine(FireballAttack());
			yield return new WaitForSeconds(0.3f);
			_animator.SetInteger("AttackState", 0);
		}
	}
	public void FirstAttack()
	{
		if (_game.needToFight)
		{
			Turn(_game._enemyDragon.transform.position);
			_animator.SetInteger("AttackState", 1);
			Debug.Log("first attack activated by the gesture");
			StartCoroutine(FireballAttack());
		}
	}
	public void SecondAttack()
	{
		if (_game.needToFight)
		{
			Turn(_game._enemyDragon.transform.position);
			_animator.SetInteger("AttackState", 2);
			Debug.Log("second attack activated by the gesture");
			StartCoroutine(FireballAttack());
		}
	}
	public void ThirdAttack()
	{
		if (_game.needToFight)
		{
			Turn(_game._enemyDragon.transform.position);
			_animator.SetInteger("AttackState", 3);
			StartCoroutine(ComeCloser());
			Debug.Log("third attack activated by the gesture");
		}
	}
	public void FourthAttack()
	{
		if (_game.needToFight)
		{
			_animator.SetInteger("AttackState", 4);
			StartCoroutine(ComeCloser());
			Debug.Log("fourth attack activated by the gesture");
		}
	}
	public void StopAttack()
	{
		_animator.SetInteger("AttackState", 0);
	}
	private IEnumerator ComeCloser()
	{
		Vector3 nextLookAt = transform.position;
		Vector3 lookAt = _game._enemyDragon.transform.position;
		lookAt.y = transform.position.y;
		transform.LookAt(lookAt);
		transform.position = Vector3.MoveTowards(transform.position, lookAt, 0.3f * Time.deltaTime);
		yield return new WaitForSeconds(1f);
	}
}
