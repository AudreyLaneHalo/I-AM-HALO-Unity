using UnityEngine;
using System.Collections;
using MalbersAnimations.HAP;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace MalbersAnimations
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MountAI : AnimalAIControl, IMountAI
    {
        public bool canBeCalled;
        protected IMount animalMount;               //The Animal Mount Script
        protected bool isBeingCalled;

        public bool CanBeCalled
        {
            get { return canBeCalled; }
            set { canBeCalled = value; }
        }

        void Start()
        {
            StartAgent();
            animalMount = GetComponent<IMount>();
        }

        void Update()
        {
            if (animalMount.Mounted)            //If the Animal is mounted
            {
                agent.enabled = false;          //Disable the navmesh agent
                return;                     
            }
            UpdateAgent();
        }

        public void CallAnimal(Transform target, bool call)
        {
            if (!CanBeCalled) return;           //If the animal cannot be called ignore this
            if (!agent) return;                 //If there's no agent ignore this

            isBeingCalled = call;
            this.target = target;

            if (isBeingCalled)
            {
                agent.enabled = true;
#if UNITY_5_6_OR_NEWER
                agent.isStopped = false;
#else
                agent.Resume();
#endif
            }
            else
            {
                agent.enabled = true;
#if UNITY_5_6_OR_NEWER
                agent.isStopped = true;
#else
                agent.Stop();
#endif
            }
        }
    }
}
