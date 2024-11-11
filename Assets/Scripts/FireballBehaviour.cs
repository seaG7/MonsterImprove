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
			_game._selectedTargets.RemoveAt(0);
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
		if (collision.transform.tag == "Player" || collision.transform.tag == "Enemy")
		{
			Destroy(gameObject);
		}
		if (collision.transform.tag == "Target")
		{
			_game._destroyedTargetsAmount++;
			Destroy(collision.transform.gameObject);
			// анимка с получением опыта (айтем над моделькой player dragon)
			Destroy(gameObject);
		}
	}
}
