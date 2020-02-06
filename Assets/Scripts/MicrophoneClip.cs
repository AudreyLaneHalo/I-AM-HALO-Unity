using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(AudioSource) )]
public class MicrophoneClip : MonoBehaviour 
{
	public bool listDevices = false;
	public string preferredDeviceName = "Built-in Microphone";
    string deviceName;
    
	void Start () 
	{
        List<string> devices = new List<string>( Microphone.devices );
        
		if (listDevices)
		{
			foreach (string device in devices) 
			{
				Debug.Log("Audio device available: " + device);
			}
		}
        
        deviceName = preferredDeviceName;
		if (devices.Find( s => s == deviceName ) == null)
		{
			Debug.LogWarning( "Can't find " + deviceName + "!" );
            if (devices.Count > 0)
            {
                deviceName = devices[0];
            }
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