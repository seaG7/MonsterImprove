using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.Burst.Intrinsics;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button spawnButton;
	[SerializeField] private Button exitButton;
	private GameController _game;
	private void Awake()
	{
		_game = FindAnyObjectByType<GameController>();
		resetRoomButton.onClick.AddListener(ResetRoom);
		spawnButton.onClick.AddListener(_game.SpawnCamera);
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
	
	public void MenuBug() 
	{
		transform.position = new Vector3(transform.position.x, _game._playerPosition.action.ReadValue<Vector3>().y - 0.5f, transform.position.z);
	}
}
