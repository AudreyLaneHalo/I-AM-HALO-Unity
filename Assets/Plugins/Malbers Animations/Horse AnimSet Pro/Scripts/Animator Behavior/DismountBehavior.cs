using UnityEngine;

using System.Collections;

namespace MalbersAnimations.HAP
{
    public class DismountBehavior : StateMachineBehaviour
    {
        [Tooltip("Use to Align the RootMotion with Hip Position, instead with the last FeetPositions")]
        public bool UseHip;

        Rider3rdPerson rider;
        Vector3 MountPosition; //Feets Next Postition
        AnimatorTransitionInfo transition;
        Transform animT;
        Transform hip;
        Vector3 laspos;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider = animator.GetComponent<Rider3rdPerson>();

            Vector3 LeftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            Vector3 RightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

            laspos = (LeftFoot + RightFoot) / 2;
            MountPosition = animator.rootPosition;

            animT = animator.transform;
            rider.Start_Dismounting();

            hip = animator.GetBoneTransform(HumanBodyBones.Hips);   //Get the Hip Bone
        }


        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider.End_Dismounting();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            transition = animator.GetAnimatorTransitionInfo(layerIndex);
            animT.position = animator.rootPosition;
            animT.rotation = animator.rootRotation;

            //Smoothly move the center of mass to the desired position in the first Transition
            if (animator.IsInTransition(layerIndex) && stateInfo.normalizedTime < 0.5f)
            {
                //animator.MatchTarget(rider.Montura.MountPoint.position, Quaternion.FromToRotation(animT.up, Vector3.up) * animT.rotation, AvatarTarget.Root,)

                if (UseHip)
                {
                    Vector3 diferencia = hip.position - rider.Montura.MountPoint.position;
                    animT.position = animT.position - diferencia;
                }
                else
                {
                    animT.position = Vector3.Lerp(MountPosition, laspos, transition.normalizedTime);
                }
                animT.rotation = Quaternion.Lerp(animT.rotation, Quaternion.FromToRotation(animT.up, Vector3.up) * animT.rotation, transition.normalizedTime);
            }
        

            //Dont go under the floor
            if (rider.MountSideTransform)
            {
                if (animT.position.y < rider.MountSideTransform.position.y)
                {
                    animT.position = new Vector3(animT.position.x, rider.MountSideTransform.position.y, animT.position.z);
                }
            }

            //Smoothly  Deactivate Mount Layer
            if (stateInfo.normalizedTime > 0.8f)
            {
                animT.rotation = Quaternion.Lerp(animT.rotation, Quaternion.FromToRotation(animT.up, Vector3.up) * animT.rotation, transition.normalizedTime);
                animT.position = Vector3.Lerp(animT.position, new Vector3(animT.position.x, rider.MountSideTransform.position.y, animT.position.z), Time.deltaTime * 5f);
            }

            animator.rootPosition = animT.position;
        }
    }
}