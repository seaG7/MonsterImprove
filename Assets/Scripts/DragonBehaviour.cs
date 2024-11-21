using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DragonBehaviour : MonoBehaviour
{
	private GameController _game;
	private InventorySystem _inventory;
	public Animator _animator;
	[Header("Your dragon Stats")]
	[SerializeField] private int _id;
	[SerializeField] private int _hp;
	[SerializeField] public float _speed;
	
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
	public Vector3 _moveRot;
	public Vector3 _startPos;
	void Start()
	{
		Init();
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Enemy")
		{
			_hp -= _inventory._strength[_id];
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
		_animator.SetBool("IsLevelUp", true);
		_inventory.GainXp(_id, 5);
		yield return new WaitForSecondsRealtime(2f);
		_animator.SetBool("IsLevelUp", false);
		_animator.SetBool("IsInspect", true);
		yield return new WaitForSecondsRealtime(3f);
		_animator.SetBool("IsInspect", false);
	}
	private IEnumerator Inspect()
	{
		_animator.SetBool("IsInspect", true);
		yield return new WaitForSecondsRealtime(3f);
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
			fireballs.RemoveAt(0);
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
			transform.rotation = Quaternion.Slerp(transform.rotation, _targetRot, 4 * Time.deltaTime);
			yield return null;
		}
	}
	private void Init()
	{
		_inventory = FindAnyObjectByType<InventorySystem>();
		_game = FindAnyObjectByType<GameController>();
		if (!_game.isSwitching)
		{
			while (!FindAnyObjectByType<PlacementManager>().isDragged) {}
			_game._cdIndex = _id;
			if (_inventory._xp[_id] != 0)
			{
				StartCoroutine(SetHatchingFalse());
			}
			else
			{
				StartCoroutine(Inspect());
			}
		}
		_game._cdController = GetComponent<DragonBehaviour>();
		_animator = GetComponent<Animator>();
		StartCoroutine(Turn(FindAnyObjectByType<Camera>().transform.position));
	}
	public IEnumerator LevelUp()
	{
        _animator.SetBool("IsLevelUp", true);
		// instantiate level up effects (visual + sound)
        yield return new WaitForSecondsRealtime(2f);
        _animator.SetBool("IsLevelUp", false);
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
			_animator.SetInteger("AttackState", number);
			if (number == 4)
				StartCoroutine(SpawnFireball());
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
		while (_game.isMiniGaming)
		{
			if (_game._selectedTargets.Count > 0)
			{
				StartCoroutine(Turn(_game._enemyDragon.transform.position));
				yield return new WaitForSeconds(1f);
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

		yield return new WaitForSeconds(0.5f);
		_inventory.GainXp(_id, _game._targetsCount);
		_game._targetsCount = 0;
	}
}
