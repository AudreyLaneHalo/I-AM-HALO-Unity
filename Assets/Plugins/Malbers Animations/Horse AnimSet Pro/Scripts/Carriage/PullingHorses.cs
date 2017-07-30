﻿using UnityEngine;
using System.Collections;
using MalbersAnimations.Events;

namespace MalbersAnimations.HAP
{
    public class PullingHorses : MonoBehaviour
    {

        [Header("Horses")]
        public Animal RightHorse;
        public Animal LeftHorse;

        public float TurnSpeed0 = 10f;
        public float TurnSpeed1 = 25f;
        public float TurnSpeed2 = 25f;
        public float TurnSpeed3 = 35f;

        [HideInInspector]
        public float CurrentTurnSpeed = 25f;

        protected Rigidbody _rigidbody;

        [HideInInspector] public Vector3 PullingDirection;          //Calculation for the Animator Velocity converted to RigidBody Velocityble
        [HideInInspector] public bool CurrentAngleSide;             //True if is in the Right Side ... False if is in the Left Side
        [HideInInspector] public bool CanRotateInPlace;

        public Transform RotationPivot;

        // Use this for initialization
        void Start()
        {
            if (!RightHorse) return;
            if (!LeftHorse) LeftHorse = RightHorse;

            _rigidbody = GetComponent<Rigidbody>();

            LeftHorse.transform.parent = transform;
            RightHorse.transform.parent = transform;

            LeftHorse.GetComponent<Rigidbody>().isKinematic = true;
            RightHorse.GetComponent<Rigidbody>().isKinematic = true;
            LeftHorse.Anim.applyRootMotion = false;
            RightHorse.Anim.applyRootMotion = false;


            switch (RightHorse.StartSpeed)
            {
                case Animal.Ground.walk:
                    CurrentTurnSpeed = TurnSpeed1;
                    break;
                case Animal.Ground.trot:
                    CurrentTurnSpeed = TurnSpeed2;
                    break;
                case Animal.Ground.run:
                    CurrentTurnSpeed = TurnSpeed3;
                    break;
                default:
                    break;
            }
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            if (!RightHorse) return;

            //rbody.velocity = RightHorse.Anim.velocity;
           // return;

            if (RightHorse.Speed1)
                CurrentTurnSpeed = TurnSpeed1;

            else if (RightHorse.Speed2)
                CurrentTurnSpeed = TurnSpeed2;

            else if (RightHorse.Speed3)
                CurrentTurnSpeed = TurnSpeed3;


           if ( LeftHorse.Anim.applyRootMotion)  LeftHorse.Anim.applyRootMotion = false;
           if (RightHorse.Anim.applyRootMotion) RightHorse.Anim.applyRootMotion = false;

            if (RightHorse.MovementAxis.z == 0)
            {
                RightHorse.MovementAxis = new Vector3(RightHorse.MovementAxis.x * 2, RightHorse.MovementAxis.y, RightHorse.MovementAxis.z);         //Put both horses to SideWalk
                LeftHorse.MovementAxis = new Vector3(LeftHorse.MovementAxis.x * 2, LeftHorse.MovementAxis.y, LeftHorse.MovementAxis.z);

                if (CanRotateInPlace)
                {
                    transform.RotateAround(RotationPivot.position, Vector3.up, RightHorse.MovementAxis.x * Time.deltaTime * TurnSpeed0);      //Rotation InPlace
                }
                else
                {
                    if ((CurrentAngleSide && RightHorse.MovementAxis.x<0) || (!CurrentAngleSide && RightHorse.MovementAxis.x> 0))       //Stop the horse Animation when it can rotate anylonger
                    {
                        RightHorse.MovementAxis = LeftHorse.MovementAxis = Vector3.zero;
                    }
                }

                
                PullingDirection = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 25);
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, Vector3.zero, Time.deltaTime * 5);
            }
            else
            {
                transform.RotateAround(RotationPivot.position, Vector3.up, RightHorse.MovementAxis.x * Time.deltaTime * CurrentTurnSpeed);          //Rotate around Speed

                PullingDirection = Vector3.Lerp(PullingDirection, transform.forward * RightHorse.Anim.velocity.magnitude * (RightHorse.MovementAxis.z >= 0 ? 1 : -1), Time.fixedDeltaTime * 5);                   //Calculate the current speed of the animator root motion

                _rigidbody.velocity = PullingDirection;
            }


            transform.position = new Vector3(transform.position.x, (RightHorse.transform.position.y + LeftHorse.transform.position.y) / 2, transform.position.z);       
        }
    }
}