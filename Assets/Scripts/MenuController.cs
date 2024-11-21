using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using TMPro.EditorUtilities;

public class MenuController : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] Button[] _selectionButtons;
	[SerializeField] Button[] _dragonButtons;
	[SerializeField] Button[] _enemyDragonButtons;
	[SerializeField] Button[] _mainMenuButtons; // 0 - reset room 1 - minigame 2 - exit
	[SerializeField] Button[] _exitModeButtons; // 0 - exit minigame 1 - exit battle
	[Header("GameObjects")]
	[SerializeField] GameObject[] _selectionWindows; // 0 - main menu 1 - dragons 2 - battle
	[SerializeField] GameObject[] _selectionFocuses; // outline for slot
	[SerializeField] GameObject[] _chosenCDSprites; // current dragon chosen sprites
	[SerializeField] GameObject[] _chosenEDSprites; // enemy dragon chosen sprites
	[Header("Texts")]
	[SerializeField] TextMeshProUGUI[] _levelTexts;
	[SerializeField] TextMeshProUGUI[] _xpTexts;
	[SerializeField] TextMeshProUGUI _destroyedTargetsAmountText; // used in FireballBehaviour OnCollisionEnter
	[SerializeField] TextMeshProUGUI _maxTargetsText;
	[Header("Sliders")]
	[SerializeField] Slider _volumeSlider;
	[SerializeField] Slider _targetCountSlider;
	private int selectionIndex = 1;
	private int cdIndex = -1;
	private int edIndex = -1;
	private float _volume;
	private GameController _game;
	private InventorySystem _inventory;
	
	void Start()
	{
		SelectionInit();
		
		foreach (var selectionButton in _selectionButtons)
			selectionButton.onClick.AddListener(ToSelection);
		foreach (var dragonButton in _dragonButtons)
			dragonButton.onClick.AddListener(SelectDragon);
		foreach (var enemyDragonButton in _enemyDragonButtons)
			enemyDragonButton.onClick.AddListener(SelectEnemyDragon);
		_mainMenuButtons[0].onClick.AddListener(ResetRoom);
		_mainMenuButtons[1].onClick.AddListener(Exit);
		_mainMenuButtons[2].onClick.AddListener(StartMode);
		foreach (var exitModeButton in _exitModeButtons)
			exitModeButton.onClick.AddListener(ExitMode);
	}
	private void SelectionInit()
	{
		for (int i = 0; i < _selectionButtons.Length; i++)
		{
			_levelTexts[i].text = _inventory.CalculateLevel(i).ToString();
			if (_inventory.CalculateLevel(i) == 5)
			{
				_xpTexts[i].text = "MAX";
			}
			else
			{
				_xpTexts[i].text = $"{_inventory.CalculateCurrentLevelXp(i)}/{_inventory._levelsXp[cdIndex]}";
			}
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
		_volumeSlider.onValueChanged.AddListener(SetVolume);
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
	private void ResetRoom()
	{
		var arSession = FindAnyObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
		var success = (arSession.subsystem as UnityEngine.XR.OpenXR.Features.Meta.MetaOpenXRSessionSubsystem)?.TryRequestSceneCapture() ?? false;
		Debug.Log($"Запрос на захват сцены Meta OpenXR завершен с результатом: {success}");
	}
	private void Exit()
	{
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		UnityEngine.Application.Quit();
	#endif
	}
	private void ExitMode()
	{
		if (_game.needToFight)
		{
			_game.needToFight = false;
			Destroy(_game._enemyDragon);
		}
		else if (_game.isMiniGaming)
		{
			_game.isMiniGaming = false;
			foreach (var target in _game._targets)
			{
				Destroy(target);
			}
		}
		foreach (var button in _mainMenuButtons)
		{
			button.gameObject.SetActive(true);
		}
		foreach (var button in _selectionWindows)
		{
			button.gameObject.SetActive(true);
		}
	}
	public void StartMode()
	{
		if (_game.isMiniGaming)
		{
			_exitModeButtons[0].gameObject.SetActive(true);
			if (_game._enemyDragon != null)
			{
				_game.needToFight = false;
				Destroy(_game._enemyDragon);
			}
			StartCoroutine(_game.MinigameFireball(Random.Range(5, 11)));

			_targetCountSlider.maxValue = _game._targetsCount;
			_maxTargetsText.text = _game._targetsCount.ToString();

		}
		else if (_game.needToFight)
		{
			_exitModeButtons[1].gameObject.SetActive(true);
		}
		foreach (var button in _mainMenuButtons)
		{
			button.gameObject.SetActive(false);
		}
		foreach (var button in _selectionWindows)
		{
			button.gameObject.SetActive(false);
		}
	}
	public void UpdateTargetCountDisplay()
	{
		_targetCountSlider.value = _game._destroyedTargetsAmount;
		_destroyedTargetsAmountText.text = _game._destroyedTargetsAmount.ToString();
	}
	private void SetVolume(float volume)
	{
		_volume = _volumeSlider.value = volume;
	}
}
