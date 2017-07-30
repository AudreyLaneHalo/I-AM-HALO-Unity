﻿using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    public class SoundBehavior : StateMachineBehaviour
    {
        public AudioClip[] sounds;

        public bool playOnEnter = true;
        public bool playOnTime;
        [Range(0, 1)]
        public float NormalizedTime = 0.5f;
        [Space]
        [Space]
        [Range(-0.5f, 3)]

        public float pitch = 1;
        [Range(0, 1)]
        public float volume = 1;

        AudioSource _audio;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _audio = animator.transform.root.GetComponent<AudioSource>();

            if (playOnEnter && _audio)
                PlaySound(animator);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playOnTime && _audio)
            {
                if (stateInfo.normalizedTime > NormalizedTime && !_audio.isPlaying)
                {
                    PlaySound(animator);
                }
            }
        }

        void PlaySound(Animator animator)
        {
            if (_audio)
            {
                if (sounds.Length > 0 && _audio.enabled)
                {
                    _audio.clip = sounds[Random.Range(0, sounds.Length)];
                    _audio.pitch = pitch;
                    _audio.volume = volume;
                    _audio.PlayOneShot(_audio.clip);
                }
            }
        }
    }
}