using UnityEngine;
using System;


namespace MalbersAnimations.HAP
{
    public class MountBehavior : StateMachineBehaviour
    {
        [Tooltip("The Riders Hip Y pos can't pass over the Mount Y pos")]
        public bool StayUnderY = true;

        public AnimationCurve MovetoMountPoint;

        protected Rider3rdPerson rider;
        protected Transform mountedside;
        protected Transform animT;
        protected Transform hip;

        const float toMountPoint = 0.2f; //Smooth time to put the Rider in the right position for mount


        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider = animator.GetComponent<Rider3rdPerson>();
            animT = animator.transform;
            hip = animator.GetBoneTransform(HumanBodyBones.Hips);

            foreach (AnimatorControllerParameter parameter in animator.parameters)                          //Set All Float values to their defaut
            {
                if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    if (parameter.name == "IKLeftFoot" || parameter.name == "IKRightFoot") break ;
                    animator.SetFloat(parameter.nameHash, parameter.defaultFloat);                          //here
                }
            }

            float CenterDiference = animator.transform.position.y - animator.pivotPosition.y;

            animT.position = new Vector3(animT.position.x, animT.position.y + CenterDiference, animT.position.z);

            mountedside = rider.MountSideTransform;

            rider.Start_Mounting();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rider.End_Mounting();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animT.position = animator.rootPosition;
            animT.rotation = animator.rootRotation;


            //Smootly move to the Mount Start Position && rotation
            if (stateInfo.normalizedTime < toMountPoint)
            {
                Vector3 NewPos = new Vector3(mountedside.position.x, animT.position.y, mountedside.position.z);
                animT.position = Vector3.Lerp(animT.position, NewPos, stateInfo.normalizedTime / toMountPoint);

                animT.rotation = Quaternion.Lerp(animT.rotation, Quaternion.LookRotation(mountedside.forward), stateInfo.normalizedTime / toMountPoint);
            }

               //Vector3 diferencia = hip.position - animator.rootPosition;


            animT.position = Vector3.Lerp(animT.position, rider.Montura.MountPoint.position/* - diferencia*/, MovetoMountPoint.Evaluate(stateInfo.normalizedTime));
            animT.rotation = Quaternion.Lerp(animT.rotation, rider.Montura.MountPoint.rotation, MovetoMountPoint.Evaluate(stateInfo.normalizedTime));

            //Smoothly Stay align in Y axis with the rider link position
            if (animT.position.y > rider.Montura.MountPoint.position.y && StayUnderY)
            {
                Vector3 NewPos = new Vector3(animT.position.x, rider.Montura.MountPoint.position.y, animT.position.z);
                animT.position = Vector3.Lerp(animT.position, NewPos, Time.deltaTime * 2);
            }
        }
    }
}

