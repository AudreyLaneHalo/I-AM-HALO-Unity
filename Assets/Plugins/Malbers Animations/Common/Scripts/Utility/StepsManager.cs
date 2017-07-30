using UnityEngine;

namespace MalbersAnimations
{
    /// <summary>
    /// This will manage the steps sounds and tracks for each animal, on each feet there's a Script StepTriger (Basic)
    /// </summary>
    public class StepsManager : MonoBehaviour
    {
        public ParticleSystem Tracks;
        public ParticleSystem Dust;

        public AudioClip[] clips;
        public float trackOffset = 0.0085f;

        //Is Called by any of the "StepTrigger" Script on a feet when they collide with the ground.
        public void EnterStep(StepTrigger foot)
        {
           
            if (!Tracks) return; //If there

            RaycastHit footRay;

            if (foot.StepAudio && clips.Length > 0) //If the track has an AudioSource Component and whe have some audio to play
            {
                foot.StepAudio.clip = clips[Random.Range(0, clips.Length)];  //Set the any of the Audio Clips from the list to the Feet's AudioSource Component
                foot.StepAudio.Play();  //Play the Audio

                //Put a track and particles
                if (!foot.HasTrack)  // If we can put a track 
                {
                    if (Physics.Raycast(foot.transform.position, -transform.up, out footRay, 1, GetComponent<Animal>().GroundLayer))
                    {
                        ParticleSystem.EmitParams ptrack = new ParticleSystem.EmitParams();
                        ptrack.rotation3D = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation).eulerAngles; //Get The Rotation
                        ptrack.position = new Vector3(foot.transform.position.x, footRay.point.y + trackOffset, foot.transform.position.z); //Get The Position


                        Tracks.Emit(ptrack, 1);

                        if (Dust)
                        {
                            Dust.transform.rotation = (Quaternion.FromToRotation(-foot.transform.forward, footRay.normal) * foot.transform.rotation);
                            Dust.transform.Rotate(-90, 0, 0);
                            Dust.Emit(1);
                        }

                    }
                }
            }
        }
    }
}
