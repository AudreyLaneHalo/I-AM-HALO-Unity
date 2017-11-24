using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(AudioSource) )]
public class MicrophoneClip : MonoBehaviour 
{
	bool debug = false;
	string deviceName = "Built-in Microphone"; //"Living Room Speaker";

	void Start () 
	{
		if (debug)
		{
			foreach (string device in Microphone.devices) 
			{
				Debug.Log("Audio device available: " + device);
			}
		}

		if (new List<string>( Microphone.devices ).Find( s => s == deviceName ) == null)
		{
			Debug.LogWarning( "Can't find " + deviceName + "!" );
		}

		AudioSource audio = GetComponent<AudioSource>();
		if (audio != null)
		{
			audio.clip = Microphone.Start( deviceName, true, 10, 44100 );
			audio.loop = true;
			audio.Play();
		}
	}
}
