using UnityEngine.UI;
using UnityEngine;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button spawnEggButton;
	[SerializeField] private Button battleButton;
	[SerializeField] private Button exitButton;
	private GameController _game;
	private void Awake()
	{
		_game = FindAnyObjectByType<GameController>();
		resetRoomButton.onClick.AddListener(ResetRoom);
		spawnEggButton.onClick.AddListener(_game.SpawnHatchingEgg);
		battleButton.onClick.AddListener(_game.StartFight);
	  	exitButton.onClick.AddListener(Exit);
	}
	private void Exit()
	{
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		UnityEngine.Application.Quit();
	#endif
	}
	private void ResetRoom()
	{
		var arSession = FindAnyObjectByType<UnityEngine.XR.ARFoundation.ARSession>();
		var success = (arSession.subsystem as UnityEngine.XR.OpenXR.Features.Meta.MetaOpenXRSessionSubsystem)?.TryRequestSceneCapture() ?? false;
		Debug.Log($"Запрос на захват сцены Meta OpenXR завершен с результатом: {success}");
	}
	void Update()
	{
		
	}
}
