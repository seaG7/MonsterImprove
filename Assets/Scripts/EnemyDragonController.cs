using UnityEngine;

public class EnemyDragonController : MonoBehaviour
{
	private GameController _game;
	public Animator _animator;
	[SerializeField] public float _hp;
	[SerializeField] public float _speed = 4f;
	[SerializeField] public float _strength;
	[SerializeField] public float _lookAtSpeed = 5f;
	[SerializeField] public float _attackRange = 1.5f;
	private float _desiredDistance = 1.5f;
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
				
			}
			
			Vector3 lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
			transform.LookAt(lookAt);
			float distance = Vector3.Distance(gameObject.transform.position, _game._currentDragon.transform.position);
			if (distance < _attackRange)
			{
				if (!_game.needToFight)
					_animator.SetBool("IsFlying", false);
				_game.needToFight = true;
			}
			else
			{
				if (_game.needToFight)
					_animator.SetBool("IsFlying", true);
				_game.needToFight = false;
				
				Vector3 direction = (_game._currentDragon.transform.position - transform.position).normalized;
				Quaternion targetRotation = Quaternion.LookRotation(direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _lookAtSpeed * Time.deltaTime);

				Vector3 targetPosition = _game._currentDragon.transform.position - direction * _desiredDistance;
				transform.position = Vector3.Lerp(transform.position, targetPosition, _speed * Time.deltaTime);
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
