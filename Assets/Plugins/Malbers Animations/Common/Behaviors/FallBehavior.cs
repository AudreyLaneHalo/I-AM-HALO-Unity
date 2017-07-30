using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    public class FallBehavior : StateMachineBehaviour
    {
        RaycastHit JumpRay; 

        [Tooltip("The Lower Fall animation will set to 1 if this distance the current distance to the ground")]
        public float LowerDistance;
        float animalFloat;
        Animal animal;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animal = animator.GetComponent<Animal>();
            animal.SetIntID(1);
            animal.IsInAir = true;
            animator.SetFloat("IDFloat", 1);
            //Resets MaxHeight
            animal.MaxHeight = 0;

            animator.applyRootMotion = false;
            animator.GetComponent<Rigidbody>().drag = 0;
            animator.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (Physics.Raycast(animator.transform.position, -animal.transform.up, out JumpRay, 100, animal.GroundLayer))
            {
                if (animal.MaxHeight < JumpRay.distance)
                {
                    //get the lower Distance 
                    animal.MaxHeight = JumpRay.distance;
                }
                //Fall Blend between fall animations ... Higher 1 one animation
                animalFloat =Mathf.Lerp(animalFloat, Mathf.Lerp(1, 0, (animal.MaxHeight - JumpRay.distance) / (animal.MaxHeight- LowerDistance)),Time.deltaTime*20f);
                animator.SetFloat("IDFloat", animalFloat);
            }
        }
    }
}