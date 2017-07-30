using UnityEngine;
using System.Collections;
using MalbersAnimations.HAP;

namespace MalbersAnimations.HAP
{
    public class Mountable : MonoBehaviour, IMount
    {


        protected Rider _rider;            //Rider's Animator to control both Sync animators from here
        protected Animal _animal;          //The animal Asociated to the mount
        protected bool mounted;            //If the Animal have been mounted  
        protected MountSide mount_side;    //From which side the animal have been mounted  

        protected Vector3 LocalStride_L, LocalStride_R;
        protected bool freeRightHand = true;
        protected bool freeLeftHand = true;

        #region Transform feet and mount on the Horse 

        public MountType mountType = MountType.Animal;

        public bool Active = true;


        public Transform ridersLink;            // Reference for the RidersLink Bone
        public Transform leftIK;                // Reference for the LeftFoot correct position on the mount
        public Transform rightIK;               // Reference for the RightFoot correct position on the mount
        public Transform leftKnee;              // Reference for the LeftKnee correct position on the mount
        public Transform rightKnee;             // Reference for the RightKnee correct position on the mount
        public bool straightSpine;              //Activate this only for other animals but the horse 
        public Vector3 spineOffset = new Vector3(-90,180,0);
        #endregion

        #region Properties
        public Transform MountPoint   { get { return ridersLink; } }    // Reference for the RidersLink Bone  
        public Transform FootLeftIK   { get { return leftIK;     } }    // Reference for the LeftFoot correct position on the mount
        public Transform FootRightIK  { get { return rightIK;    } }    // Reference for the RightFoot correct position on the mount
        public Transform KneeLeftIK   { get { return leftKnee;   } }    // Reference for the LeftKnee correct position on the mount
        public Transform KneeRightIK  { get { return rightKnee;  } }    // Reference for the RightKnee correct position on the mount

        public bool StraightSpine { get { return straightSpine; } }     // Reference for the RightKnee correct position on the mount

        public Quaternion SpineOffset
        {
            get
            {
                return Quaternion.Euler(spineOffset);
            }
        }     



        public virtual MountType Mount_Type
        {
            get { return mountType; }
        }


        public virtual Animal Animal
        {
            get
            {
                if (_animal == null)
                    _animal = GetComponent<Animal>();
                return _animal;
            }
        }

        /// <summary>
        /// Dismount only when the Animal is Still on place
        /// </summary>
        public virtual bool CanDismount
        {
            get { return Animal.Stand; }
        }

        public virtual bool Mounted
        {
            get { return mounted; }
            set { mounted = value; }
        }

        public virtual bool CanBeMounted
        {
            get { return Active; }
            set { Active = value; }
        }

        public Rider ActiveRider
        {
            get { return _rider; }
            set { _rider = value; }
        }
        #endregion


        public virtual void EnableControls(bool value)
        {
            if (Animal)
            {
                MalbersInput animalControls = Animal.GetComponent<MalbersInput>();

                if (animalControls)
                {
                    animalControls.enabled = value;
                }
                Animal.MovementAxis = Vector3.zero;
            }
        }

        public virtual void SyncAnimator(Animator anim)
        {
            if (Animal)
            {
                Animal.LinkingAnimator(anim);
            }
        }

        //UnityEditor
        [HideInInspector] public bool ShowLinks;
    }
}