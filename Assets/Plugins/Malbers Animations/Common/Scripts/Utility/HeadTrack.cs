using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MalbersAnimations.Utilities
{
    [Serializable]
    public class BoneRotation
    {
        public Transform bone;                                      //The bone
        public Vector3 offset = new Vector3(0, -90, -90);           //The offset for the look At
        [Range(0, 1)]
        public float weight = 1;                                    //the Weight of the look at
    }
    public class HeadTrack : MonoBehaviour, IAnimatorListener       //This is for sending messages from the animator
    {
        public bool active = true;                                  //For Activating and Deactivating the HeadTrack

        [Header("Looks towards a Camera or a Target")]
        public bool UseCamera;                   //Use the camera ForwardDirection instead a Target
        public Transform Target;                        //Use a target to look At

        [Space]
        public float LimitAngle = 80f;                  //Max Angle to LookAt
        public float Smoothness = 5f;                   //Smoothness between Enabled and Disable

        [Space]
        public BoneRotation[] Bones;                    //Bone Chain   

        private Transform cam;                          //Reference for the camera
        protected float angle;                          //Angle created between the transform.Forward and the LookAt Point    
        protected float weightMiltiplier;                //this will is used with the smoothness to smoothly swap beteen enabled and disabled

        void Start()
        {
            if (Camera.main != null) cam = Camera.main.transform;   //Get the main camera
        }

        void LateUpdate()
        {
            SetWeight();            
            LookAtBoneSet();        //Rotate the bones
        }

        /// <summary>
        /// This will set the weight of the total Look At
        /// </summary>
        private void SetWeight()
        {
            int activeValue = 0;

            if (active && angle < LimitAngle) activeValue = 1; //If is active and the able is on the limits
            if (active && !UseCamera && Target == null) activeValue = 0;

            weightMiltiplier = Mathf.Lerp(weightMiltiplier, activeValue, Time.deltaTime * Smoothness);
        }

        /// <summary>
        /// Enable or Disable this script functionality
        /// </summary>
        public void EnableLookAt(bool value)
        {
            active = value;
        }

        /// <summary>
        /// Calulates the direction from the origin towards the camera or a specific target
        /// </summary>
        Vector3 CalculateDir(Transform origin)
        {
            Vector3 dir = cam.forward;

            RaycastHit hit = new RaycastHit();
            hit.distance = float.MaxValue;

            if (UseCamera)
            {
                RaycastHit[] hits;

                hits = Physics.RaycastAll(cam.position, cam.forward, 100);

                foreach (RaycastHit item in hits)
                {
                    if (item.transform.root == transform.root) continue; //Dont Hit anything in this hierarchy
                    if (Vector3.Distance(cam.position, item.point) < Vector3.Distance(cam.position, origin.position)) continue; //If I hit something behind me skip
                    if (hit.distance > item.distance) hit = item;
                }

                if (hit.distance != float.MaxValue)
                {
                    dir = (hit.point - origin.position).normalized;
                }
            }
            else
            {
                if (Target)
                {
                    dir = (Target.position - origin.position).normalized;
                }
                else
                {
                    dir = transform.forward;
                }
            }
            return dir;
        }

        /// <summary>
        /// Rotates the bones to the Look direction
        /// </summary>
        void LookAtBoneSet()
        {
            if (Bones.Length > 0)
            {
                foreach (var bones in Bones)
                {
                    Vector3 dir = CalculateDir(bones.bone);

                    angle = Vector3.Angle(transform.forward, dir); //Set the angle for the current bone

                    Quaternion next =
                        Quaternion.Slerp(bones.bone.rotation, Quaternion.LookRotation(dir) * Quaternion.Euler(bones.offset), bones.weight * weightMiltiplier); //Calculate the rotation
                    bones.bone.rotation = next;         //Apply the Rotation
                }
            }
        }

        /// <summary>
        /// This is used to listen the Animator asociated to this gameObject
        /// </summary>
        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }
    }
}
