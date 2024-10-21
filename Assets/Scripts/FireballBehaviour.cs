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
			Vector3 lookPos = lookAt - transform.position;
			lookPos.y = 0;
	   		transform.rotation = Quaternion.LookRotation(lookPos);
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
			transform.position = Vector3.MoveTowards(transform.position, transform.forward, _speed * Time.deltaTime);
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
