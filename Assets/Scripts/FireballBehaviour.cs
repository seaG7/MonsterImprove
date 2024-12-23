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
		float _timer = 0;
		while (true)
		{
			transform.position = Vector3.MoveTowards(transform.position, targetPos, _speed * Time.deltaTime);
			yield return null;
			_timer += Time.deltaTime;
			if (_timer > 3)
				Destroy(gameObject);
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (_game._selectedTargets[0] == collision.transform.gameObject)
		{
			_game._destroyedTargetsAmount++;
			FindAnyObjectByType<MenuController>().UpdateTargetCountDisplay();
			_game._cdController.needToShoot = true;
			_game._targets.Remove(collision.transform.gameObject);
			_game._selectedTargets.RemoveAt(0);
			Destroy(collision.transform.gameObject);
			// анимка с получением опыта (айтем над моделькой player dragon)
			Destroy(gameObject);
		}
	}
}
