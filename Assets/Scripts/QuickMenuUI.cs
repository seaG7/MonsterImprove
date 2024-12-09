using UnityEngine.UI;
using UnityEngine;

public class QuickMenuUI : MonoBehaviour
{
	[SerializeField] private Button resetRoomButton;
	[SerializeField] private Button exitButton;
	[SerializeField] private Button createcube;
	[SerializeField] private GameObject cube;
	private void Awake()
	{
		resetRoomButton.onClick.AddListener(ResetRoom);
	  	exitButton.onClick.AddListener(Exit);
		createcube.onClick.AddListener(Create);
	}
	public void Create()
	{
		GameObject cube1 = Instantiate(cube,new Vector3(FindAnyObjectByType<QuickMenuUI>().transform.position.x, FindAnyObjectByType<QuickMenuUI>().transform.position.y-0.4f, FindAnyObjectByType<QuickMenuUI>().transform.position.z),Quaternion.identity);
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
