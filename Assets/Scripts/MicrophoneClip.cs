using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using NatMic.Recorders;

[RequireComponent( typeof(AudioSource) )]
public class MicrophoneClip : MonoBehaviour 
{
    //ClipRecorder clipRecorder;
    //bool recording;

    //AudioSource _audioSource;
    //AudioSource audioSource
    //{
    //    get
    //    {
    //        if (_audioSource == null)
    //        {
    //            _audioSource = GetComponent<AudioSource>();
    //            Debug.Log(_audioSource == null);
    //        }
    //        return _audioSource;
    //    }
    //}

    //void Start ()
    //{
    //    Debug.Log("start");
    //    clipRecorder = new ClipRecorder(Play);
    //    recording = true;
    //}

    //void Update ()
    //{
    //    if (Time.time > 1 && recording)
    //    {
    //        Debug.Log("play " + clipRecorder);
    //        clipRecorder.Dispose();
    //        recording = false;
    //    }
    //    //clipRecorder = new ClipRecorder(Play);
    //}

    //void Play (AudioClip _clip)
    //{
    //    Debug.Log("started playing " + _clip.samples);
    //    audioSource.clip = _clip;
    //    audioSource.Play();
    //}
}
