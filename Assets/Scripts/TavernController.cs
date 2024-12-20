using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TavernController : MonoBehaviour
{
	[SerializeField] private FarmController _farmController;
	[SerializeField] private GameObject[] _icons; // 0 - harvest, 1 - eat
	[SerializeField] private GameObject _animal;
	[SerializeField] private AudioSource _harvestAS;
	[SerializeField] private FoodGeneratorController _foodGeneratorC;
	[SerializeField] private GameObject _canvas;
	public bool anyStateActive = false;
	public bool isEat = false;
	public int lastState = -1;
	
	public void Start()
	{
		_farmController = gameObject.GetComponentInParent<FarmController>();

		FoodGeneratorController.ActionGeneratorSpawned += GetGeneratorSpawned;
	}
	
	public void OnDisable()
	{
		FoodGeneratorController.ActionGeneratorSpawned -= GetGeneratorSpawned;
	}
	
	public void GetGeneratorSpawned() 
	{
		_foodGeneratorC = FindAnyObjectByType<FoodGeneratorController>();
	}

    private void Update()
    {
		_canvas.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>());
    }

    public GameObject GetAnimal() 
	{
		return _animal;
	}

	public void HarvestinTavern() 
	{
		if (isEat && anyStateActive)
		{
			if (_foodGeneratorC != null && _foodGeneratorC._foodAmount > 0) 
			{
				_foodGeneratorC._foodAmount--;
			
				_foodGeneratorC._foodAmountTMP.text = _foodGeneratorC._foodAmount.ToString() + "/" + _foodGeneratorC._maxFoodAmount.ToString();
				_foodGeneratorC._foodAmountBackTMP.text = _foodGeneratorC._foodAmount.ToString() + "/" + _foodGeneratorC._maxFoodAmount.ToString();
			
				anyStateActive = false;
				_icons[lastState].SetActive(false);
				_harvestAS.clip = _farmController._gameController._eat;
				_harvestAS.Play();
				StartCoroutine(LifeCycle());
			}
			
			else 
			{
				StartCoroutine(_farmController._gameController._showNotification(1));
				_harvestAS.clip = _farmController._gameController._error;
				_harvestAS.Play();
			}
			
			GameController.SaveData();
		}
		
		else if (anyStateActive) 
		{
			_farmController._harvestAmount++; 
			GameController._collected++;
			
			GameController.SaveData();
			
			_farmController._harvestAmountTMP.text = _farmController._harvestAmount.ToString() + "/" + _farmController._maxHarvestAmount.ToString();
			_farmController._harvestAmountBackTMP.text = _farmController._harvestAmount.ToString() + "/" + _farmController._maxHarvestAmount.ToString();
			
			_farmController._gameController._collectedTMP.text = GameController._collected.ToString();
			_farmController._gameController._collectedBackTMP.text = GameController._collected.ToString();
			anyStateActive = false;

			_harvestAS.clip = _farmController._gameController._harvestMilk;
			_harvestAS.Play();
			
			_icons[lastState].SetActive(false);
			
			StartCoroutine(LifeCycle());
		}
		
		GameController.SaveData();
	}
	
	public IEnumerator LifeCycle() 
	{
		yield return new WaitForSeconds(Random.Range(15f, 45f));
		
		if (!anyStateActive) 
		{
			int randomState = Random.Range(0, 2);
			lastState = randomState;
			
			if (randomState == 1) 
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
