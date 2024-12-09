using System.Collections;
using UnityEngine;

public class FireballBehaviour : MonoBehaviour
{
	[SerializeField] public float _speed;
	private GameController _game;
	private Vector3 _targetPos;
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		if (_game.needToFight)
		{
			if (gameObject.CompareTag("Enemy"))
				_targetPos = _game._currentDragon.transform.position;
			else
				_targetPos = _game._enemyDragon.transform.position;
			_targetPos.y += 0.1f;
		}
		else if (_game.isMiniGaming)
		{
			_targetPos = _game._selectedTargets[0].transform.position;
			_targetPos.y += 0.1f;
		}
		else
		{
			_targetPos = _game._currentDragon.transform.forward;
		}
		StartCoroutine(Fly(_targetPos));
	}
	void Update()
	{
		
	}
	public IEnumerator Fly(Vector3 targetPos)
	{
		while (true)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
			yield return null;
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		// if (collision.transform.tag == "Player")
		// {
		// 	FindAnyObjectByType<EnemyDragonBehaviour>().isAttacking = true;
		// 	FindAnyObjectByType<DragonBehaviour>()._hp -= FindAnyObjectByType<EnemyDragonBehaviour>()._strength;
		// 	FindAnyObjectByType<DragonBehaviour>()._hpSlider.value = FindAnyObjectByType<DragonBehaviour>()._hp;
		// 	FindAnyObjectByType<EnemyDragonBehaviour>().DealDamage();
		// 	if (FindAnyObjectByType<DragonBehaviour>()._hp <= 0)
		// 	{
		// 		FindAnyObjectByType<MenuController>().ResetChosenSprites(true);
		// 		FindAnyObjectByType<DragonBehaviour>().EndAnimGestureIcons();
		// 		FindAnyObjectByType<MenuController>().cdIndex = -1;
		// 		FindAnyObjectByType<MenuController>().edIndex = -1;
		// 		FindAnyObjectByType<DragonBehaviour>()._animator.SetBool("IsDie", true);
		// 		StartCoroutine(_game.Kill(_game._currentDragon));
		// 		FindAnyObjectByType<MenuController>().ExitMode();
		// 	}
		// }
		// else if (collision.transform.tag == "Enemy")
		// {
		// 	FindAnyObjectByType<EnemyDragonBehaviour>()._hp -= FindAnyObjectByType<InventorySystem>()._strength[FindAnyObjectByType<DragonBehaviour>()._id];
		// 	FindAnyObjectByType<EnemyDragonBehaviour>()._hpSlider.value = FindAnyObjectByType<EnemyDragonBehaviour>()._hp;
		// 	if (FindAnyObjectByType<EnemyDragonBehaviour>()._hp <= 0)
		// 	{
		// 		FindAnyObjectByType<MenuController>().ResetChosenSprites(false);
		// 		FindAnyObjectByType<DragonBehaviour>().EndAnimGestureIcons();
		// 		FindAnyObjectByType<MenuController>().edIndex = -1;
		// 		FindAnyObjectByType<EnemyDragonBehaviour>()._animator.SetBool("IsDie", true);
		// 		FindAnyObjectByType<InventorySystem>().GainXp(FindAnyObjectByType<DragonBehaviour>()._id, FindAnyObjectByType<EnemyDragonBehaviour>()._xpByKill);
		// 		StartCoroutine(_game.Kill(_game._enemyDragon));
		// 		FindAnyObjectByType<MenuController>().ExitMode();
		// 	}
		// }
		if (collision.transform.tag == "Target")
		{
			_game._destroyedTargetsAmount++;
			FindAnyObjectByType<MenuController>().UpdateTargetCountDisplay();
			_game._selectedTargets.RemoveAt(0);
			_game._cdController.needToShoot = true;
			Destroy(collision.transform.gameObject);
			// анимка с получением опыта (айтем над моделькой player dragon)
			Destroy(gameObject);
		}
	}
}
