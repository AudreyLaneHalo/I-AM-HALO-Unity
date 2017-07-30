using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations.HAP
{
    public class Rider3rdPerson : Rider 
    {
        #region IK VARIABLES    
        protected float L_IKFootWeight = 0f;        //IK Weight for Left Foot
        protected float R_IKFootWeight = 0f;        //IK Weight for Right Foot
        #endregion

        public UnityEvent
              OnStartMounting,
              OnEndMounting,
              OnStartDismounting,
              OnEndDismounting,
              OnAlreadyMounted;

        [HideInInspector]
        public int MountLayerHash;               //Mount Layer Hash


        /// Set all the References
        void Awake()
        {
            _transform = transform;
            if (!Anim) Anim = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponents<Collider>();
        }

        void Start()
        {
            IsOnHorse = Mounted = false;                                    //initialize in false

            if (Anim) MountLayerHash = Anim.GetLayerIndex("Mounted");    //Get The mount Layer ID

            if (StartMounted) AlreadyMounted();                             //Set All if Started Mounted is Active
        }

        /// <summary>
        ///Set all the correct atributes and variables to Start Mounted on the next frame
        /// </summary>
        public void AlreadyMounted()
        {
            StartCoroutine(AlreadyMountedC());
        }

        IEnumerator AlreadyMountedC()
        {
            yield return null;      //Wait for the next frame

            if (AnimalStored != null && StartMounted)
            {
                Montura = AnimalStored;
                Start_Mounting();
                End_Mounting();
                Anim.SetBool(Hash_Mount, mounted);    //Update Mount Parameter In the Animator

                if (Montura.Mount_Type == MountType.Carriage || Montura.Mount_Type == MountType.Wagon || Montura.Mount_Type == MountType.Cart)
                {
                    if (Anim) Anim.Play("Idle Carriage", MountLayerHash);               //Play Idle01 Animation Directly
                }
                toogleCall = true; //Turn Off the Call
            }

            OnAlreadyMounted.Invoke();
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This is call at the Beginning of the Mount Animations
        /// </summary>
        public override void Start_Mounting()
        {
            if (Anim) Anim.SetLayerWeight(MountLayerHash, 1);                             //Enable Mount Layer

            if (Anim) Anim.SetBool(Hash_Mount, Mounted);                                 //Update the Mount Parameter on the Animator

            if (!mountSideTransform) mountSideTransform = Montura.transform;            //If null add the horse as transform

            if (DisableComponents)
            {
                ToggleComponents(false);                                                //Disable all Monobehaviours breaking the Mount System
            }

            base.Start_Mounting();
            OnStartMounting.Invoke();                                                  //Invoke UnityEvent for  Start Mounting

            if (!Anim) End_Dismounting();
        }


        public override void End_Mounting()
        {
            base.End_Mounting();
            OnEndMounting.Invoke();
        }

        public override void Start_Dismounting()
        {
            base.Start_Dismounting();
            OnStartDismounting.Invoke();

        }
        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This is call at the end of the Dismounting Animations States on the animator
        /// </summary>
        public override void End_Dismounting()
        {
            if (Anim) Anim.SetLayerWeight(MountLayerHash, 0);                       //Reset the Layer Weight to 0 when end dismounting
            base.End_Dismounting();

            if (DisableComponents) ToggleComponents(true);                          //Enable all Monobehaviours breaking the Mount System
            OnEndDismounting.Invoke();                                              //Invoke UnityEvent when is off Animal
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Syncronize the Horse/Rider animations if Rider loose sync with the animal on the locomotion state
        /// </summary>
        private void Check_Syncronization_with_Horse()
        {
            if (!Anim) return;
            if (Montura.Mount_Type != MountType.Animal) return;

            if (Montura.Animal.Anim.GetCurrentAnimatorStateInfo(0).IsTag("Locomotion"))                             //Search for syncron the locomotion state on the animal
            {
                 RiderNormalizedTime = Anim.GetCurrentAnimatorStateInfo(MountLayerHash).normalizedTime;            //Get the normalized time from the Rider
                 HorseNormalizedTime = Montura.Animal.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime;           //Get the normalized time from the Horse

                syncronize = true;   

                if (Mathf.Abs(RiderNormalizedTime - HorseNormalizedTime) > 0.1f && Time.time-LastSyncTime>0.5f)     //Checking if the animal and the rider are unsync by 0.2
                {
                    Anim.CrossFade("Locomotion",0.1f, MountLayerHash, HorseNormalizedTime);                      //Normalized with blend
                    //_anim.Play("Locomotion", MountLayerHash, HorseNormalizedTime);                                //Normalized without blend
                    LastSyncTime = Time.time;
                }
            }
            else
            {
                syncronize = false;
            }
        }

        //Used this for Sync Animators
        private float RiderNormalizedTime;
        private float HorseNormalizedTime;
        private float LastSyncTime;
        private bool syncronize;

        void Update()
        {

            if (MountInput.GetInput)                                        //If the mount Key is pressed
            {
                if (CanMount)                                               //if are near an animal and we are not already on an animal
                {
                    Run_Mounting();                                         //Run mounting Animations
                }
                else if (CanDismount)                                       //if we are already mounted and the animal is not moving (Mounted && IsOnHorse && Montura.CanDismount)
                {
                    Run_Dismounting();                                      //Run Dismounting Animations
                }
                else if (!can_Mount && !Mounted && !IsOnHorse)              //if there is no horse near, call the animal in the slot
                {
                    CallAnimal();
                }
            }

            //───────────────────────────────────────────────────────────────────────────────────────────────────────
            if (isRiding && Montura != null)                                          //Run Stuff While Mounted
            {
                WhileIsMounted();
            }
        }


        protected virtual void WhileIsMounted()
        {
            if (Montura.StraightSpine)                                          //Used for other Animals but the horse
            {
                Montura.MountPoint.rotation =
                        Quaternion.LookRotation(Vector3.up, Montura.transform.forward) * Montura.SpineOffset;                //Keep a Straight Spine while Aiming
            }

            Check_Syncronization_with_Horse();                                  //Check the syncronization and fix it if is offset***

            if (isRiding && Montura != null && Anim)
            {
                Montura.SyncAnimator(Anim);                                     //Sync Animators while is in the horse
            }
        }

        protected virtual void Run_Mounting()
        {
            if (Montura == null) return;

            Montura.Mounted = Mounted = true;                               //Send to the animalMount that mounted is active

            if (Anim)
            {
                Anim.SetLayerWeight(MountLayerHash, 1);

                Anim.SetBool(Hash_Mount, Mounted);                            //Update Mount Parameter In the Animator
                Anim.SetInteger(Hash_MountSide, ((int)(Montura.Mount_Type) * 10) + (int)Mount_Side);             //Update Mount_Side Parameter In the Animator
            }

            if (Montura.Mount_Type != MountType.None)
            {
                string MountAnimation = "Mount " + Montura.Mount_Type.ToString() + " " + Mount_Side.ToString();
                if (Anim) Anim.Play(MountAnimation, MountLayerHash);
            }
            else
            {
                Start_Mounting();
                End_Mounting();
                if (Anim) Anim.Play("Idle01", MountLayerHash);               //Play Idle01 Animation Directly
            }

        }

        protected virtual void Run_Dismounting()
        {
            Montura.Mounted = Mounted = false;                      //Unmount the Animal

            if (Anim)
            {
                Anim.SetBool(Hash_Mount, Mounted);                     //Update Mount Parameter In the Animator
                Anim.SetInteger(Hash_MountSide, ((int)(Montura.Mount_Type) * 10) + (int)Mount_Side);  //Update MountSide Parameter In the Animator
            }
            if (Montura.Mount_Type == MountType.None)              //Use for Instant mount
            {
                if (Anim) Anim.Play("Null", MountLayerHash);
                Start_Dismounting();
                End_Dismounting();
                _transform.position = mountSideTransform.position + (mountSideTransform.forward * -0.2f);
            }
        }

        void OnAnimatorMove()
        {
            if (Anim.GetNextAnimatorStateInfo(MountLayerHash).IsTag("Unmounting"))
            {
                _transform.position = Anim.rootPosition;
                _transform.rotation = Anim.rootRotation;
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// IK Feet Adjustment while mounting
        /// </summary>
        void OnAnimatorIK()
        {
            if (Anim == null) return;           //If there's no animator skip
            if (Montura != null)
            {
                if (Montura.FootLeftIK == null || Montura.FootRightIK == null 
                 || Montura.KneeLeftIK == null || Montura.KneeRightIK == null) return;  //if is there missing an IK point do nothing

                //linking the weights to the animator
                if (Mounted || IsOnHorse)
                {
                    L_IKFootWeight = 1f;
                    R_IKFootWeight = 1f;

                    if (Anim.GetCurrentAnimatorStateInfo(MountLayerHash).IsTag("Mounting") || Anim.GetCurrentAnimatorStateInfo(MountLayerHash).IsTag("Unmounting"))
                    {
                        L_IKFootWeight = Anim.GetFloat("IKLeftFoot");
                        R_IKFootWeight = Anim.GetFloat("IKRightFoot");
                    }

                    //setting the weight
                    Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, L_IKFootWeight);
                    Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, R_IKFootWeight);

                    Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, L_IKFootWeight);
                    Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, R_IKFootWeight);

                    //Knees
                    Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, L_IKFootWeight);
                    Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, R_IKFootWeight);

                    //Set the IK Positions
                    Anim.SetIKPosition(AvatarIKGoal.LeftFoot, Montura.FootLeftIK.position);
                    Anim.SetIKPosition(AvatarIKGoal.RightFoot, Montura.FootRightIK.position);

                    //Knees
                    Anim.SetIKHintPosition(AvatarIKHint.LeftKnee, Montura.KneeLeftIK.position);    //Position
                    Anim.SetIKHintPosition(AvatarIKHint.RightKnee, Montura.KneeRightIK.position);  //Position

                    Anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, L_IKFootWeight);   //Weight
                    Anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, R_IKFootWeight);  //Weight

                    //setting the IK Rotations of the Feet
                    Anim.SetIKRotation(AvatarIKGoal.LeftFoot, Montura.FootLeftIK.rotation);
                    Anim.SetIKRotation(AvatarIKGoal.RightFoot, Montura.FootRightIK.rotation);
                }
                else
                {
                    Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
                    Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);

                    Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
                    Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
                }
            }
        }

        ///Inspector Entries
        [SerializeField] public bool Editor_Advanced;

        
        public bool debug;

        void OnDrawGizmos()
        {
            if (!debug) return;
            if (!Anim) return;
            if (syncronize)
            {
                Transform head = Anim.GetBoneTransform(HumanBodyBones.Head);

                if ((int)RiderNormalizedTime % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                    Gizmos.DrawSphere((head.position - transform.root.right * 0.2f), 0.05f);

                if ((int)HorseNormalizedTime % 2 == 0)
                {
                    Gizmos.color = Color.red;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                    Gizmos.DrawSphere((head.position + transform.root.right * 0.2f), 0.05f);

#if UNITY_EDITOR
                UnityEditor.Handles.color = Color.white;
                UnityEditor.Handles.Label(head.position + transform.up * 0.5f, "Sync Status");
#endif

            }



            if (Anim)
            {
                //Gizmos.color = Color.red;
                //Gizmos.DrawSphere(RiderAnimator.pivotPosition,0.05f);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawSphere(RiderAnimator.rootPosition, 0.05f);

                //if (RiderAnimator)
                //{
                //    Vector3 LeftFoot = RiderAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
                //    Vector3 RightFoot = RiderAnimator.GetBoneTransform(HumanBodyBones.RightFoot).position;

                //    //Vector3 laspos = new Vector3((LeftFoot.x + RightFoot.x) / 2, (LeftFoot.y + RightFoot.y) / 2, (LeftFoot.z + RightFoot.z) / 2);

                //    Vector3 laspos = (LeftFoot + RightFoot) / 2;
                //    Gizmos.color = Color.green;
                //    Gizmos.DrawSphere(laspos, 0.05f);
                //}
            }
        }
    }
}