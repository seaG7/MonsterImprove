using System.Collections;
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
			transform.position = Vector3.MoveTowards(transform.position, lookAt, _speed * Time.deltaTime);
			yield return null;
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Player")
		{
			Destroy(gameObject);
		}
	}
}
