using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuController : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] Button[] _selectionButtons;
	[SerializeField] Button[] _dragonButtons;
	[SerializeField] Button[] _enemyDragonButtons;
	[Header("GameObjects")]
	[SerializeField] GameObject[] _selectionWindows; // section
	[SerializeField] GameObject[] _selectionFocuses; // outline for slot
	[SerializeField] GameObject[] _chosenCDSprites; // current dragon
	[SerializeField] GameObject[] _chosenEDSprites; // enemy dragon
	[Header("Texts")]
	[SerializeField] TextMeshProUGUI[] _levelTexts;
	[SerializeField] TextMeshProUGUI[] _xpTexts;
	private int selectionIndex = 1;
	private int cdIndex = -1;
	private int edIndex = -1;
	private GameController _game;
	private InventorySystem _inventory;
	
	void Start()
	{
		SelectionInit();
		
		foreach (var selectionButton in _selectionButtons)
		{
			selectionButton.onClick.AddListener(ToSelection);
		}
		foreach (var dragonButton in _dragonButtons)
		{
			dragonButton.onClick.AddListener(SelectDragon);
		}
		foreach (var enemyDragonButton in _enemyDragonButtons)
		{
			enemyDragonButton.onClick.AddListener(SelectEnemyDragon);
		}
	}
	private void SelectionInit()
	{
		for (int i = 0; i < _selectionButtons.Length; i++)
		{
			_levelTexts[i].text = _inventory.CalculateLevel(i).ToString();
			_xpTexts[i].text = $"{_inventory.CalculateCurrentLevelXp(i)}/{_inventory._levelsXp[cdIndex]}";
			if (i != selectionIndex)
			{
				_selectionWindows[i].SetActive(false);
				_selectionFocuses[i].SetActive(false);
			}
			else
			{
				_selectionWindows[i].SetActive(true);
				_selectionFocuses[i].SetActive(true);
			}
		}
		for (int i = 0; i < _dragonButtons.Length; i++)
		{
			if (i != cdIndex)
			{
				_chosenCDSprites[i].SetActive(false);
			}
			else
			{
				_chosenCDSprites[i].SetActive(true);
			}
		}
	}
	private void ToSelection()
	{
		for (int i = 0; i < _selectionButtons.Length; i++)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == _selectionButtons[i])
			{
				_selectionWindows[selectionIndex].SetActive(false);
				_selectionFocuses[selectionIndex].SetActive(false);
				selectionIndex = i;
				_selectionWindows[i].SetActive(true);
				_selectionFocuses[i].SetActive(true);
				
			}
		}
	}
	private void SelectDragon()
	{
		for (int i = 0; i < _dragonButtons.Length; i++)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == _dragonButtons[i])
			{
				if (_inventory._dragonIndexes.Contains(i))
				{
					_chosenCDSprites[cdIndex].SetActive(false);
					cdIndex = i;
					_chosenCDSprites[i].SetActive(true);
					_game.SelectCD(cdIndex);
				}
			}
		}
	}
	private void SelectEnemyDragon()
	{
		for (int i = 0; i < _enemyDragonButtons.Length; i++)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == _enemyDragonButtons[i])
			{
				_chosenEDSprites[edIndex].SetActive(false);
				edIndex = i;
				_chosenCDSprites[i].SetActive(true);
				_game.SelectED(edIndex);
			}
		}
	}
}
