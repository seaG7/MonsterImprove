using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button spawnFarmButton;
	[SerializeField] private Button exitButton;
	private GameController _game;
	private void Awake()
	{
		_game = FindAnyObjectByType<GameController>();
		resetRoomButton.onClick.AddListener(ResetRoom);
		spawnFarmButton.onClick.AddListener(_game.SpawnFarm);
		spawnFarmButton.onClick.AddListener(PlayClick);
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
	private void ResetRoom()
	{
		PlayClick();
		var arSession = FindAnyObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
		var success = (arSession.subsystem as UnityEngine.XR.OpenXR.Features.Meta.MetaOpenXRSessionSubsystem)?.TryRequestSceneCapture() ?? false;
		Debug.Log($"Запрос на захват сцены Meta OpenXR завершен с результатом: {success}");
	}
	
	private void PlayClick() 
	{
		_game._mainAS.clip = _game._clicks[Random.Range(0, 2)];
		_game._mainAS.Play();
	}
}
