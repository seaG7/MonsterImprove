using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FireballBehaviour : MonoBehaviour
{
	[SerializeField] public float _speed;
	private GameController _game;
	private Vector3 lookAt;
	void Start()
	{
		_game = FindAnyObjectByType<GameController>();
		if (_game.needToFight)
		{
			lookAt = _game._currentDragon.transform.position;
			lookAt.y = transform.position.y;
		}
		else
		{
			// направить файрбол туда, куда нажали
		}
		StartCoroutine(Fly());
	}
	void Update()
	{
		
	}
	public IEnumerator Fly()
	{
		while (true)
		{
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed);
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
			Destroy(_game._targets[0]);
			// анимка с получением опыта (айтем над моделькой player dragon)
			Destroy(gameObject);
		}
	}
}
