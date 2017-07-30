using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{

    public class RigidConstraintsB : StateMachineBehaviour
    {
        public bool PosX, PosY = true, PosZ, RotX =true, RotY=true, RotZ=true;
        public bool OnEnter = true, OnExit;
        protected int Amount = 0;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (PosX) Amount += 2;
            if (PosY) Amount += 4;
            if (PosZ) Amount += 8;
            if (RotX) Amount += 16;
            if (RotY) Amount += 32;
            if (RotZ) Amount += 64;

            if (OnEnter)
            {
                animator.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)Amount;
            }
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (OnExit)
                animator.GetComponent<Rigidbody>().constraints = (RigidbodyConstraints)Amount;
        }
    }
}