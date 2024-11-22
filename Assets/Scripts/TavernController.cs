using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernController : MonoBehaviour
{
	[SerializeField] private FarmController _farmController;
	[SerializeField] private GameObject[] _icons; // 0 - harvest, 1 - eat
	[SerializeField] private GameObject _animal;
	[SerializeField] private AudioSource _harvestAS;
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
			GameController._collected++;
			
			_farmController._harvestAmountTMP.text = _farmController._harvestAmount.ToString() + "/" + _farmController._maxHarvestAmount.ToString();
			_farmController._harvestAmountBackTMP.text = _farmController._harvestAmount.ToString() + "/" + _farmController._maxHarvestAmount.ToString();
			
			_farmController._gameController._collectedTMP.text = GameController._collected.ToString();
			_farmController._gameController._collectedBackTMP.text = GameController._collected.ToString();
			anyStateActive = false;
			_harvestAS.Play();
		}
		
		else if (anyStateActive) 
		{
			GameController._coinAmount--;
			
			_farmController._gameController._coinAmountTMP.text = GameController._coinAmount.ToString();
			_farmController._gameController._coinAmountBackTMP.text = GameController._coinAmount.ToString();
			
			anyStateActive = false;
			_harvestAS.Play();
		}
		
		_icons[lastState].SetActive(false);
		
		GameController.SaveData();
		
		
		StartCoroutine(LifeCycle());
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
				isEat = true;
				anyStateActive = true;
			}
			
			else 
			{
				isEat = false;
				anyStateActive = true;
			}
		
			_icons[randomState].SetActive(true);
			
			GameController.SaveData();
			
			_farmController._gameController._collectedTMP.text = GameController._collected.ToString();
			_farmController._gameController._collectedBackTMP.text = GameController._collected.ToString();
		}
	}
}
