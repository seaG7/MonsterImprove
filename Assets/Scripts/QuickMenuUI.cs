using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button statisticsButton;
	[SerializeField] private GameObject _statisticsFrame;
	[SerializeField] private Button exitButton;
	private GameController _game;
	
	private void Awake()
	{
		_game = FindAnyObjectByType<GameController>();
		resetRoomButton.onClick.AddListener(ResetRoom);
		statisticsButton.onClick.AddListener(StatisticsActive);
		statisticsButton.onClick.AddListener(PlayClick);
	  	exitButton.onClick.AddListener(Exit);
	}
	private void Exit()
	{
		PlayClick();
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		UnityEngine.Application.Quit();
	#endif
	}
	private void StatisticsActive() 
	{
		_statisticsFrame.gameObject.SetActive(!_statisticsFrame.activeSelf);
	}
	
	private void ResetRoom()
	{
		PlayClick();
		var arSession = FindAnyObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
		var success = (arSession.subsystem as UnityEngine.XR.OpenXR.Features.Meta.MetaOpenXRSessionSubsystem)?.TryRequestSceneCapture() ?? false;
		Debug.Log($"Запрос на захват сцены Meta OpenXR завершен с результатом: {success}");
	}
	
	public void PlayClick() 
	{
		_game._mainAS.clip = _game._click;
		_game._mainAS.Play();
	}
	
	public void HandleDropDown(int value) 
	{
		_game.id = value;
	}
}
