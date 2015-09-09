using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour 
{
	AudioClip oxygenAlert, run, boost;
	AudioSource runAudio, alertAudio, boostAudio;


	// Use this for initialization
	void Start () 
	{
		//Load Audio Clips
		oxygenAlert = (AudioClip)Resources.Load("OxygenAlert");
		boost = (AudioClip)Resources.Load("Boost");

		//Create AudioSources
		alertAudio = gameObject.AddComponent<AudioSource>();
		boostAudio = gameObject.AddComponent<AudioSource>();

		//Set AudioSource settings
		alertAudio.playOnAwake = false;
		alertAudio.loop = true;
		alertAudio.volume = .25f;
		alertAudio.clip = oxygenAlert;

		boostAudio.playOnAwake = false;
		boostAudio.loop = true;
		boostAudio.volume = .1f;
		boostAudio.clip = boost;
	}

	public void OxygenAlert(bool play)
	{
		if(play)
		{
			if(!alertAudio.isPlaying)
				alertAudio.Play();
		}

		else
		{
			if(alertAudio.isPlaying)
				alertAudio.Stop();
		}
	}

	public void Boost(bool play)
	{
		if(play)
		{
			if(!boostAudio.isPlaying)
				boostAudio.Play();
		}
		
		
		else
		{
			if(boostAudio.isPlaying)
				boostAudio.Stop();
		}
	}
}
