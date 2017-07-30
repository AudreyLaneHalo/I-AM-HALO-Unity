using UnityEngine;
using System.Collections;


namespace MalbersAnimations
{
    
    public class StepTrigger : MonoBehaviour
    {
        StepsManager _StepsManager;
        [HideInInspector]
        public AudioSource StepAudio;
        [Range(0,1)]
        public float volume = 1;
        bool hastrack;                      // Check if already  has a track... don't put another
        bool waitrack;                      // Check if is time to put a track; 
        public bool HasTrack
        {
            get { return hastrack; }
            set { hastrack = value; }
        }

        void Awake()
        {
            _StepsManager = GetComponentInParent<StepsManager>();

            StepAudio = GetComponent<AudioSource>();

            if (StepAudio == null)
            {
                StepAudio = gameObject.AddComponent<AudioSource>();
            }
            

            StepAudio.volume = volume;
        }


        void OnTriggerEnter(Collider other)
        {

            if (!waitrack && _StepsManager)             //
            {
                 StartCoroutine(WaitForStep(0.1f));     //Wait Half a Second before making another Step

                _StepsManager.EnterStep(this);
                hastrack = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            hastrack = false; // if the feet is on the air then can put a track
        }

        IEnumerator WaitForStep(float seconds)
        {
            waitrack =  true;
            yield return new WaitForSeconds(seconds);
            waitrack = false;
        }
    }
}