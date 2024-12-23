using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class MenuController : MonoBehaviour
{
	[Header("Buttons")]
	[SerializeField] Button[] _selectionButtons;
	[SerializeField] Button[] _dragonButtons;
	[SerializeField] Button[] _enemyDragonButtons;
	[SerializeField] public Button[] _mainMenuButtons; // 0 - reset room 1 - exit 2 - minigame 3 - remove dragon
	[SerializeField] Button[] _exitModeButtons; // 0 - exit minigame 1 - exit battle
	[Header("GameObjects")]
	[SerializeField] GameObject _volumeChangerWindow;
	[SerializeField] GameObject _sectionIconsWindow;
	[SerializeField] GameObject[] _selectionWindows; // 0 - main menu 1 - dragons 2 - battle
	[SerializeField] GameObject[] _selectionFocuses; // outline for slot
	[SerializeField] GameObject _exitMiniGameWindow;
	[SerializeField] GameObject[] _chosenCDSprites; // current dragon chosen sprites
	[SerializeField] GameObject[] _chosenEDSprites; // enemy dragon chosen sprites
	[SerializeField] GameObject[] _statsObjects;
	[SerializeField] GameObject[] _lockedObjects;
	[SerializeField] GameObject[] _awardObjects;
	[SerializeField] GameObject[] _requiredObjects;
	[Header("Texts")]
	[SerializeField] TextMeshProUGUI[] _levelTexts;
	[SerializeField] TextMeshProUGUI[] _xpTexts;
	[SerializeField] TextMeshProUGUI[] _hpTexts;
	[SerializeField] TextMeshProUGUI[] _killsTexts;
	[SerializeField] TextMeshProUGUI[] _requiredTexts;
	[SerializeField] TextMeshProUGUI _destroyedTargetsAmountText; // used in FireballBehaviour OnCollisionEnter
	[SerializeField] TextMeshProUGUI _maxTargetsText;
	[Header("Sliders")]
	[SerializeField] Slider[] _cdXpSliders;
	[SerializeField] Slider _volumeSlider;
	[SerializeField] Slider _targetCountSlider;
	[Header("Audio")]
	[SerializeField] AudioSource _music;
	[SerializeField] public AudioSource _sounds;
	private int selectionIndex = 0;
	public int cdIndex = -1;
	public int edIndex = -1;
	private float _volume = 0.4f;
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
		_mainMenuButtons[2].onClick.AddListener(StartMiniGame);
		_mainMenuButtons[3].onClick.AddListener(RemoveDragon);
		foreach (var exitModeButton in _exitModeButtons)
			exitModeButton.onClick.AddListener(ExitMode);
		_volumeSlider.onValueChanged.AddListener(SetVolume);
		_volumeSlider.value = _volume;
		_music.volume = _volume;
		_sounds.volume = _volume;
	}
	private void SelectionInit()
	{
		_game = FindAnyObjectByType<GameController>();
		_inventory = FindAnyObjectByType<InventorySystem>();
		for (int i = 0; i < _selectionButtons.Length; i++)
		{
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
		_mainMenuButtons[2].gameObject.SetActive(false);
		UpdateDragonsDisplay();
		UpdateEnemyDragonsDisplay();
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
	private void ToSelectionByIndex(int index)
	{
		_selectionWindows[selectionIndex].SetActive(false);
		_selectionFocuses[selectionIndex].SetActive(false);
		selectionIndex = index;
		_selectionWindows[index].SetActive(true);
		_selectionFocuses[index].SetActive(true);
	}
	public void UpdateDragonsDisplay()
	{
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
			if (_inventory._dragonIndexes.Contains(i))
			{
				_statsObjects[i].SetActive(true);
				_lockedObjects[i].SetActive(false);
				_hpTexts[i].text = _inventory._hp[i].ToString();
			}
			else
			{
				_statsObjects[i].SetActive(false);
				_lockedObjects[i].SetActive(true);
			}
			_levelTexts[i].text = _inventory.CalculateLevel(i).ToString();
			if (_inventory.CalculateLevel(i) == 5)
				_xpTexts[i].text = "MAX";
			else
				_xpTexts[i].text = $"{_inventory.CalculateCurrentLevelXp(i)}/{_inventory.CalculateMaxLevelXp(i)}";
			_cdXpSliders[i].minValue = 0;
			_cdXpSliders[i].maxValue = _inventory.CalculateMaxLevelXp(i);
			_cdXpSliders[i].value = _inventory.CalculateCurrentLevelXp(i);
		}
	}
	public void UpdateEnemyDragonsDisplay()
	{
		for (int i = 0; i < _enemyDragonButtons.Length; i++)
		{
			if (i != edIndex)
			{
				_chosenEDSprites[i].SetActive(false);
			}
			else
			{
				_chosenEDSprites[i].SetActive(true);
			}
			_killsTexts[i].text = $"{_inventory._kills}/";
			if (int.TryParse(_requiredTexts[i].text, out int number))
			{
				if (_inventory._kills >= number)
				{
					_awardObjects[i].SetActive(true);
					_requiredObjects[i].SetActive(false);
				}
				else
				{
					_awardObjects[i].SetActive(false);
					_requiredObjects[i].SetActive(true);
				}
			}
		}
	}
	private void SelectDragon()
	{
		for (int i = 0; i < _dragonButtons.Length; i++)
		{
			if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == _dragonButtons[i])
			{
				if (_inventory._dragonIndexes.Contains(i) && cdIndex != i)
				{
					if (cdIndex > -1)
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
		if (cdIndex > -1)
		{
			for (int i = 0; i < _enemyDragonButtons.Length; i++)
			{
				if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() == _enemyDragonButtons[i] && _awardObjects[i].activeInHierarchy)
				{
					if (edIndex > -1)
						_chosenEDSprites[edIndex].SetActive(false);
					edIndex = i;
					_chosenEDSprites[i].SetActive(true);
					_game.SelectED(edIndex);
				}
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
	public void ExitMode()
	{
		for (int i = 0; i < _mainMenuButtons.Length - 1; i++)
		{
			_mainMenuButtons[i].gameObject.SetActive(true);
		}
		if (_game.isMiniGaming)
		{
			_game.isMiniGaming = false;
			foreach (var target in _game._targets)
			{
				Destroy(target);
			}
			_exitMiniGameWindow.SetActive(false);
			_mainMenuButtons[_mainMenuButtons.Length-1].gameObject.SetActive(true);
		}
		else
		{
			_game.needToFight = false;
			if (_game._currentDragon == null)
				_mainMenuButtons[_mainMenuButtons.Length-1].gameObject.SetActive(false);
			_exitModeButtons[0].gameObject.SetActive(false);
		}
		_mainMenuButtons[2].gameObject.SetActive(true);
		_sectionIconsWindow.SetActive(true);
		_volumeChangerWindow.SetActive(true);
	}
	public void StartMiniGame()
	{
		_game.isMiniGaming = true;
		if (_game._enemyDragon != null)
		{
			_game.needToFight = false;
			Destroy(_game._enemyDragon);
		}
		StartCoroutine(_game.MinigameFireball(4));

		_targetCountSlider.maxValue = _game._targetsCount;
		_game._destroyedTargetsAmount = 0;
		UpdateTargetCountDisplay();
		_maxTargetsText.text = _game._targetsCount.ToString();
		
		foreach (var button in _mainMenuButtons)
		{
			button.gameObject.SetActive(false);
		}
		for (int i = 1; i < _selectionWindows.Length; i++)
		{
			_selectionWindows[i].SetActive(false);
		}
		ToSelectionByIndex(0);
		_exitMiniGameWindow.SetActive(true);
		_sectionIconsWindow.SetActive(false);
		_volumeChangerWindow.SetActive(false);
	}
	public void StartBattle()
	{
		foreach (var button in _mainMenuButtons)
		{
			button.gameObject.SetActive(false);
		}
		for (int i = 1; i < _selectionWindows.Length; i++)
		{
			_selectionWindows[i].SetActive(false);
		}
		_exitModeButtons[0].gameObject.SetActive(true);
		_sectionIconsWindow.SetActive(false);
		_volumeChangerWindow.SetActive(false);
		StartCoroutine(_game.TurnCD(_game._enemyDragon.transform.position));
	}
	public void UpdateTargetCountDisplay()
	{
		_targetCountSlider.value = _game._destroyedTargetsAmount;
		_destroyedTargetsAmountText.text = _game._destroyedTargetsAmount.ToString();
	}
	private void SetVolume(float volume)
	{
		_volume = _volumeSlider.value = volume;
		_music.volume = _volume;
		_sounds.volume = _volume;
	}
	private void RemoveDragon()
	{
		Destroy(_game._currentDragon);
		cdIndex = -1;
		UpdateDragonsDisplay();
		_mainMenuButtons[3].gameObject.SetActive(false);
	}
}
