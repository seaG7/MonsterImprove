using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernController : MonoBehaviour
{
	[SerializeField] private FarmController _farmController;
	[SerializeField] private GameObject[] _icons; // 0 - harvest, 1 - eat
	[SerializeField] private GameObject _animal;
	public bool anyStateActive = false;
	public bool isEat = false;
	public int lastState = -1;
	
	public void Start()
	{
		_farmController = gameObject.GetComponentInParent<FarmController>();
	}

	
	public GameObject GetAnimal() 
	{
		return _animal;
	}

	public void HarvestinTavern() 
	{
		if (isEat && anyStateActive)
		{
			_farmController._harvestAmount++; 
			_farmController._harvestAmountTMP.text = _farmController._harvestAmount.ToString() + "/" + _farmController._maxHarvestAmount.ToString();
		}
		
		else if (anyStateActive) 
		{
			GameController._coinAmount--;
			anyStateActive = false;
		}
		
		_icons[lastState].SetActive(false);
		_farmController._gameController.SaveCoins();
	}
	
	public IEnumerator LifeCycle() 
	{
		yield return new WaitForSeconds(Random.Range(15f, 45f));
		
		if (!anyStateActive) 
		{
			int randomState = Random.Range(0, 2);
			lastState = randomState;
			
			if (randomState == 0) 
			{
				isEat = false;
				anyStateActive = true;
			}
			
			else 
			{
				isEat = true;
				anyStateActive = true;
			}
		
			_icons[randomState].SetActive(true);
		}
	}
}
