using UnityEngine;
using System.Collections;
#if UNITY_5_5_OR_NEWER
using UnityEngine.AI;
#endif

namespace MalbersAnimations
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AnimalAIControl : MonoBehaviour
    {
        #region Components References
        protected NavMeshAgent agent;               //The NavMeshAgent
        protected Animal animal;                    //The Animal Script
        #endregion

        public Transform target;                    //The Target

        [Header("Distance for changing speed to")]

        public float Trot = 5f;                 
        public float Run = 7f;

        // Use this for initialization
        void Start()
        {
            StartAgent();
        }

        protected virtual void StartAgent()
        {
            animal = GetComponent<Animal>();
            agent = GetComponent<NavMeshAgent>();

            agent.updateRotation = false;
            agent.updatePosition = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (!agent.isOnNavMesh) return;
            if (target == null) return;

            UpdateAgent();
        }


        /// <summary>
        /// Updates the Agents using he animation root motion
        /// </summary>
        protected void UpdateAgent()
        {
            if (!agent.isOnNavMesh) return;

            agent.nextPosition = transform.position;

            if (target != null)
                agent.SetDestination(target.position);

            if (agent.remainingDistance > agent.stoppingDistance)
                animal.Move(agent.desiredVelocity.normalized);
            else
                animal.Move(Vector3.zero);

            VelocityChange();
            OffMeshBehaviour();
        }

        private void OffMeshBehaviour()
        {
            if (agent.isOnOffMeshLink)
            {
                OffMeshLinkData OffmeshLinkData = agent.currentOffMeshLinkData;


                if (OffmeshLinkData.linkType == OffMeshLinkType.LinkTypeManual)
                {

                    if (OffmeshLinkData.offMeshLink.area == 2)          //if the ofmeshh link is a Jump type
                    {
                        animal.SetJump();
                    }
                }
            }

            if ((!animal.IsFalling || !animal.isJumping()) && agent.isOnOffMeshLink)
            {
                agent.CompleteOffMeshLink();
            }
        }

        protected virtual void VelocityChange()
        {
            if (agent.remainingDistance > Run)
            {
                animal.Speed3 = true;
            }
            else if (agent.remainingDistance < Run)
            {
                animal.Speed2 = true;
            }
            else if (agent.remainingDistance < Trot)
            {
                animal.Speed1 = true;
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }
    }
}
