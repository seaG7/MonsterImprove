using UnityEngine.UI;
using UnityEngine;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button spawnEggButton;
	[SerializeField] private Button exitButton;
	private void Awake()
	{
		resetRoomButton.onClick.AddListener(ResetRoom);
		spawnEggButton.onClick.AddListener(SpawnEgg);
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
	private void SpawnEgg()
	{
		var egg = Resources.Load<GameObject>("Prefabs/Interactable Egg");    
		Instantiate(egg, transform.position - transform.forward * 0.25f, Quaternion.identity);
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
