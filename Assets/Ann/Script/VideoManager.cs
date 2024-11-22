using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
	private VideoPlayer _VP;
	
	void Start()
	{
		_VP = GetComponent<VideoPlayer>();
	}
	
	public void TurnVideo() 
	{
		if (!_VP.isPlaying) 
		{
			_VP.Play();
		}
		
		else 
		{
			_VP.Pause();
		}
	}
}
