using UnityEngine;

public class EnemyDragonController : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	[SerializeField] public float _hp;
	[SerializeField] public float _speed;
	[SerializeField] public float _strength;
	[SerializeField] public float _attackRange = 1f;
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		_animator = GetComponent<Animator>();
	}
	void FixedUpdate()
	{
		if (_game._currentDragon != null)
		{
			if (_hp <= 0)
			{
				_animator.SetBool("IsDie", true);
			}
			
			Vector3 lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.LookAt(lookAt);
			float distance = Vector3.Distance(transform.position, _game._currentDragon.transform.position);
			if (distance <= _attackRange)
			{
				if (!_game.needToFight)
					{
						_animator.SetBool("IsFlying", false);
						_animator.SetBool("IsLanding", true);
						_animator.SetInteger("AttackState", 1);
					}
				_game.needToFight = true;
			}
			else if (distance + 0.1f > _attackRange)
			{
				_animator.SetInteger("AttackState", 0);
				_animator.SetBool("IsFlying", true);
				_game.needToFight = false;
				
				transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed * Time.deltaTime);
			}
		}
	}
	void Update()
	{
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		_hp -= _game._currentDragon.GetComponent<DragonBehaviour>()._strength;
	}
}
