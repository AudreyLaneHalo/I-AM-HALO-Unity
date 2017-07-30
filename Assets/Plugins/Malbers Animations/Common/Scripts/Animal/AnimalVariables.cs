using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Events;

namespace MalbersAnimations
{
    public class DamageValues
    {
        public Vector3 Direction;
        public Vector3 Theircenter;
        public float Amount = 0;

        public DamageValues(Vector3 dir, float amount = 0)
        {
            Direction = dir;
            Amount = amount;
        }
    }
    public partial class Animal
    {

        public enum Ground
        {
            walk = 1, trot = 2, run = 3
        }

        #region Animal Components 
        protected Animator _anim;
        protected AnimatorStateInfo _currentState;
        protected Transform _transform;
        protected Rigidbody _rigidbody;
        protected Collider _collider;
        protected Transform Cam;
        #endregion

        #region Animator Parameters Variables

        protected Vector3 movementAxis;  // In this variable will store vertical (Z) and horizontal Y

        protected bool
            speed1,                //Walk (Set by Input)
            speed2,                //Trot (Set by Input)
            speed3,                //Run  (Set by Input)
            jump,                  //Jump (Set by Input)
            fly,                   //Fly  (Set by Input)
            shift,                 //Sprint or Speed Swap (Set by Input)
            down,                  //Crouch or Swim Underwater (Set by Input)
            dodge,                 //Dodge (Set by Input)
            damaged,               //GetHit (Set by OnTriggerEnter)
            fall, fallback,        //If is falling, (Automatic: Fall method)
            death,                 //Death (Set by Life<0)
            isInWater,             //if is Entering Water(Trigger by WaterEnter Script or if the RayWater Hit the WaterLayer)
            isInAir,               //if is Jumping or falling (Automatic: Fix Position Method)
            swim,                  //if is in Deep Water (Automatic: Swim method)
            attack1,attack2,       //Attacks (Set by Input)
            stun,                  //Stunned (Set by Input or callit by a property)
            action,                //Actions (Set by Input combined with Action ID)  
            stand = true,          //If Horizontal and Vertical are =0 (Automatic)
            isAttacking,           //Set to true whenever an Attack Animations is played (Set by Animator) 
            isTakingDamage,        //Prevent to take damage while this variable is true
            backray,               //Check if the Back feet are touching the ground 
            frontray;              //Check if the Front feet are touching the ground

        protected float
            speed,                 //Speed from the Vertical input multiplied by the speeds inputs(Walk Trot Run)
            direction,             //Direction from the Horizontal input multiplied by the Shift for turning 180    
            groundSpeed = 1f,      //Current Ground Speed (Walk=1, Trot=2 , Run =3) 
            slope,                 //Normalized Angle Slope from the MAX Angle Slope 
            idfloat,               //Float values for the animator
            _Height;               //Height from the ground to the hip 

        protected int
            idInt = 1,             //Integer values for the animator      
            actionID = -1,         //Every Actions has an ID this combined with the action bool will activate an action animation
            tired;                 //counter to go to SleepMode(Time OUT) 
        #endregion
        

        #region Inspector Entries
        
        public int  animalTypeID;

        public LayerMask GroundLayer;
        public Ground StartSpeed = Ground.walk;
        public float height = 1f;
        public float WalkSpeed = 1f;
        public float TrotSpeed = 1f;
        public float RunSpeed = 1f;
        public float TurnSpeed = 0f;

        public float movementS1 = 1, movementS2=2, movementS3=3; //IMPORTANT this are the values for the Animator Locomotion Blend Tree when the velocity is changed (Ex. Horse has 5 velocities)

        
        [Range(0f, 90f)]
        public float maxAngleSlope  =  45f;
        [Range(0, 100)]
        public int GotoSleep;
        public float SnapToGround = 20f;
        public float FallRayDistance = 0.15f;
        public bool swapSpeed;

        public float waterLine = 0f;
        public float swimSpeed = 1f;
        public float swimTurn = 0f;
        #endregion

        [SerializeField] public float life = 100;
        [SerializeField] public float defense = 0;
        [SerializeField] public float attackStrength = 10;

        public bool debug;

        //------------------------------------------------------------------------------
        #region Modify_the_Position_Variables

        protected RaycastHit hit_Hip, hit_Chest;
        protected Vector3 
            _fallVector, 
            hitDirection, 
            UpVector = Vector3.up;


        protected float
             turnAmount,
             forwardAmount,
             scaleFactor = 1,
             maxHeight;

        protected Pivots[] pivots;
        protected Pivots _Chest, _Hip;

        protected Vector3 SurfaceNormal;
        protected int SpeedCount;
        protected bool speed_Counter;
        #endregion


        #region Events
        public UnityEvent OnGetDamaged;
        public UnityEvent OnKilled;
        public UnityEvent OnJump;
        public UnityEvent OnFall;
        public UnityEvent OnAttack;
        #endregion

        #region Properties
        /// <summary>
        /// Current Animal Speed
        /// </summary>
        public float GroundSpeed
        {
            get { return groundSpeed; }
        }

        public float Speed
        {
            get { return speed; }
        }

        public float Direction
        {
            get { return direction; }
        }

        public bool SpeedHasChanged
        {
            get { return speed1 || speed2 || speed3; }
        }

        public bool IsFalling
        {
            get { return _currentState.IsTag("Fall"); }
        }

        public float MaxHeight
        {
            set { maxHeight = value; }
            get { return maxHeight; }
        }

        public int Tired
        {
            set { tired = value; }
            get { return tired; }
        }

        public bool IsInWater
        {
            get { return isInWater; }
        }
        public bool Speed1
        {
            get { return speed1; }
            set { speed1 = value; }
        }

        public bool Speed2
        {
            get { return speed2; }
            set { speed2 = value; }
        }

        public bool Speed3
        {
            get { return speed3; }
            set { speed3 = value; }
        }

        public bool Jump
        {
            get { return jump; }
            set { jump = value; }
        }
        public bool Shift
        {
            get { return shift; }
            set { shift = value; }
        }
        public bool Down
        {
            get { return down; }
            set { down = value; }
        }

        public bool Dodge
        {
            get { return dodge; }
            set { dodge = value; }
        }

        public bool Damaged
        {
            get { return damaged; }
            set { damaged = value; }
        }
        public bool Fly
        {
            get { return fly; }
            set { fly = value; }
        }

        public bool Death
        {
            get { return death; }
            set { death = value; }
        }

        public bool Attack1
        {
            get { return attack1; }
            set { attack1 = value; }
        }

        public bool Attack2
        {
            get { return attack2; }
            set { attack2 = value; }
        }

        public bool Stun
        {
            get { return stun; }
            set { stun = value; }
        }

        public bool Action
        {
            get { return action; }
            set { action = value; }
        }

        public int ActionID
        {
            get { return actionID; }
            set { actionID = value; }
        }

        public bool IsInAir
        {
            get { return isInAir; }
            set { isInAir = value; }
        }

        public bool Stand
        {
            get { return stand; }
        }

        public Vector3 HitDirection
        {
            get { return hitDirection; }
            set { hitDirection = value; }
        }

        public float ScaleFactor
        {
            get { return scaleFactor; }
        }

        public Pivots Pivot_Hip
        {
            get { return _Hip; }
        }
        public Pivots Pivot_Chest
        {
            get { return _Chest; }
        }

        public bool IsAttacking
        {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

        public Animator Anim
        {
            get { return _anim; }
        }

        public Vector3 Pivot_fall
        {
            get { return _fallVector; }
        }

        public RigidbodyConstraints Still_Constraints
        {
            get { return RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; }
        }

        public Vector3 MovementAxis
        {  
            get { return movementAxis; }
            set { movementAxis = value; }
        }
        #endregion

        //UnityEditor Variables
        [HideInInspector] public bool ShowEventsEditor;

    }
}
