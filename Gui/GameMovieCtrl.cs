using UnityEngine;
using System.Collections;

public class GameMovieCtrl : MonoBehaviour {
	public MovieTexture Movie;
	AudioSource AudioSourceObj;
	bool IsStopMovie;
	public static bool IsActivePlayer;
	static GameMovieCtrl _instance;
	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}
	
	// Use this for initialization
	void Start()
	{
		//Screen.SetResolution(1360, 768, true);
		Screen.SetResolution(1280, 720, true);

        _instance = this;
		IsActivePlayer = true;
		AudioSourceObj = transform.GetComponent<AudioSource>();
		PlayMovie();
	}
	
	void PlayMovie()
	{
		renderer.enabled = true;
		renderer.material.mainTexture = Movie;
		Movie.loop = true;
		Movie.Play();
		
		if (AudioSourceObj != null) {
			AudioSourceObj.clip = Movie.audioClip;
			AudioSourceObj.enabled = true;
			AudioSourceObj.Play();
		}
	}
	
	public void StopPlayMovie()
	{
		if (IsStopMovie) {
			return;
		}
		IsStopMovie = true;
		Movie.Stop();
		if (AudioSourceObj != null) {
			AudioSourceObj.enabled = false;
		}
		gameObject.SetActive(false);
	}
}