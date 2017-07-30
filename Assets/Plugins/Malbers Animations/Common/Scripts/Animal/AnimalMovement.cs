﻿using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
   
    public partial class Animal
    {
        void Awake()
        {
            _anim = GetComponent<Animator>();
            _anim.SetInteger("Type", animalTypeID); //Adjust the layer for the curret animal Type this is of offseting the bones to another pose
        }

        void Start()
        {
            SetStart();
        }

        protected virtual void SetStart()
        {
            _transform = transform;                         //Set Reference for the transform
            _rigidbody = GetComponent<Rigidbody>(); 
            pivots = GetComponentsInChildren<Pivots>();     //Pivots are Strategically Transform objects use to cast rays used by the animal
            scaleFactor = _transform.localScale.y;          //TOTALLY SCALABE animal
            groundSpeed = (int)StartSpeed;
            SpeedCount = (int)StartSpeed - 1;
        }

       //Link all Parameters to the animator
       public virtual void LinkingAnimator(Animator anim_)
        {
            if (!death)
            {
                anim_.SetFloat(HashIDsAnimal.verticalHash, speed);
                anim_.SetFloat(HashIDsAnimal.horizontalHash, direction);
                anim_.SetFloat(HashIDsAnimal.slopeHash, slope);
                anim_.SetBool(HashIDsAnimal.shiftHash, shift);
                anim_.SetBool(HashIDsAnimal.standHash, stand);
                anim_.SetBool(HashIDsAnimal.jumpHash, jump);
                anim_.SetBool(HashIDsAnimal.attack1Hash, attack1);
                anim_.SetBool(HashIDsAnimal.damagedHash, damaged);
                anim_.SetBool(HashIDsAnimal.fallHash, fall);
                anim_.SetBool(HashIDsAnimal.stunnedHash, stun);
                anim_.SetBool(HashIDsAnimal.action, action);
                anim_.SetBool(HashIDsAnimal.stunnedHash, stun);
                anim_.SetBool(HashIDsAnimal.swimHash, swim);
                anim_.SetInteger(HashIDsAnimal.actionID, actionID);
                anim_.SetInteger(HashIDsAnimal.IDIntHash, idInt);//////
            }
            else  //Triggers the Death
            {
                if (!_currentState.IsTag("Death") && !_anim.GetNextAnimatorStateInfo(0).IsTag("Death"))
                {
                    anim_.SetTrigger(HashIDsAnimal.deathHash);
                    GetComponent<MalbersInput>().enabled = false;
                   
                }
            }
        }

        //Add more Rotations to the current Turn Animations ───────────────────────────────────────────────────────────────────────────────────
        protected virtual void AdditionalTurn()
        {
            float Turn;
            Turn = TurnSpeed;

            if (!_currentState.IsTag("Locomotion")) Turn = 0;

            if (swim) Turn = swimSpeed;

            if (movementAxis.z >= 0)
            {
                _transform.Rotate(_transform.up, Turn * 3 * movementAxis.x * Time.deltaTime);
            }
            else
            {
                _transform.Rotate(_transform.up, Turn * 3 * -movementAxis.x * Time.deltaTime);
            }
           
            if (isJumping() || fall && !fly && !swim && !stun)               //More Rotation when jumping and falling... in air rotation
            {
                if (movementAxis.z >= 0)
                    _transform.Rotate(_transform.up, 100 * movementAxis.x * Time.deltaTime);
                else
                    _transform.Rotate(_transform.up, 100 * -movementAxis.x * Time.deltaTime);
            }
        }

        //Add more Speed to the current Move animations───────────────────────────────────────────────────────────────────────────────────
        protected virtual void AdditionalSpeed()
        {
            float amount = 0;
            float axis = movementAxis.z;
            Vector3 direction = _transform.forward;

            if (groundSpeed == 1) amount = WalkSpeed;
            if (groundSpeed == 2) amount = TrotSpeed;
            if (groundSpeed == 3) amount = RunSpeed;

            if (!_currentState.IsTag("Locomotion")) amount = 0;

            if (swim) amount = swimSpeed;

            _transform.position = Vector3.Lerp(_transform.position, _transform.position + direction * amount * axis / 5f, Time.deltaTime);
        }

        //Terrain Logic
        protected virtual void FixPosition()
        {
            UpVector = -Physics.gravity;

            scaleFactor = _transform.localScale.y;              //Keep Updating the Scale Every Frame
            _Height = height * scaleFactor;                     //multiply the Height by the scale
            _Hip = pivots[0];
            _Chest = pivots[1];

            float distanceHip = 0, distanceChest = 0;           //For saving the distance for the hip and the chest

            backray = frontray = false;

            //Ray From Hip to the ground
            if (Physics.Raycast(_Hip.GetPivot, -_transform.up, out hit_Hip, scaleFactor * _Hip.multiplier, GroundLayer))
            {
                if (debug) Debug.DrawRay(hit_Hip.point, hit_Hip.normal * 0.2f, Color.blue);
                distanceHip = hit_Hip.distance;

                // if the hip ray has a Big angle ignore it
                if (hit_Hip.normal.y > 0.7)
                    backray = true;
            }
            else distanceHip = _Height;

            //Ray From Chest to the ground ***Use the pivot for calculate the ray... but the origin position to calculate the distance
            if (Physics.Raycast(_Chest.GetPivot, -_transform.up, out hit_Chest, scaleFactor * _Chest.multiplier, GroundLayer))
            {
                if (debug) Debug.DrawRay(hit_Chest.point, hit_Chest.normal * 0.2f, Color.red);

                distanceChest = Vector3.Distance(_Chest.transform.position, hit_Chest.point);
               
                if (hit_Chest.normal.y > 0.7)  // if the hip ray if in Big Angle ignore it
                    frontray = true;
            }
            else distanceChest = _Height;

            //───────────────────────────────────────────────Terrain Adjusment───────────────────────────────────────────────────────────────────────────────────

            //Calculate the Align vector of the terrain
            Vector3 direction = (hit_Chest.point - hit_Hip.point).normalized;
            Vector3 Side = Vector3.Cross(UpVector, direction).normalized;
            SurfaceNormal = Vector3.Cross(direction, Side).normalized;

            float AngleSlope = Vector3.Angle(SurfaceNormal, UpVector);                   //Calculate the Angle of the Terrain
            slope = AngleSlope / maxAngleSlope * ((_Chest.Y > _Hip.Y) ? 1 : -1);           //Normalize the AngleSlop by the MAX Angle Slope and make it positive(HighHill) or negative(DownHill)

            Quaternion finalRot = Quaternion.FromToRotation(_transform.up, SurfaceNormal) * _rigidbody.rotation;  //Orient To Terrain  

            if (isInAir || isJumping())                                 // If the character is falling, jumping or swimming smoothly aling with the Up Vector
            {
                if (slope < 0 || (fall && !isJumping()))                //Don't Align when is UpHill
                {
                    finalRot = Quaternion.FromToRotation(_transform.up, UpVector) * _rigidbody.rotation;
                    _transform.rotation = Quaternion.Lerp(_transform.rotation, finalRot, Time.deltaTime * 5f);
                }
            }
            else
            {
                if (!swim)
                {
                    _transform.rotation = Quaternion.Lerp(_transform.rotation, finalRot, Time.deltaTime * 10f);
                }
            }

            float distance = distanceHip;

            if (!backray) distance = distanceChest;         //if is landing on the front feets

            float realsnap = SnapToGround;                  //Change in the inspector the  adjusting speed for the terrain

            //───────────────────────────────────────────────Snap To Terrain- HIGHER───────────────────────────────────────────────────────────────────────────────────
            if (distance > _Height && !isInAir && !swim)
            {
                float diference = _Height - distance;

                if (_currentState.IsTag("Locomotion"))
                {
                    _transform.position = _transform.position + new Vector3(0, diference, 0); //Dont Lerp While is in locomotion
                }
                else
                    _transform.position = Vector3.Lerp(_transform.position, _transform.position + new Vector3(0, diference, 0), Time.deltaTime * realsnap);
            }
            //───────────────────────────────────────────────Snap To Terrain   LOWER───────────────────────────────────────────────────────────────────────────────────
            else if (distance < _Height)
            {
                isInAir = false;
                if (!fall && !isInAir)
                {
                    float diference = _Height - distance;
                    _transform.position = Vector3.Lerp(_transform.position, _transform.position + new Vector3(0, diference, 0), Time.deltaTime * realsnap);
                    if (!Death)
                    {
                        _rigidbody.constraints = Still_Constraints ;
                    }
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Fall Logic
        /// </summary>
        protected virtual void Falling()
        {
            RaycastHit hitpos;

            //Set the Fall Ray a bit farther from the front feet.
            _fallVector = _Chest.GetPivot + (_transform.forward.normalized * groundSpeed * FallRayDistance * ScaleFactor);

            if (debug) Debug.DrawRay(_fallVector, -transform.up * _Chest.multiplier * scaleFactor, Color.magenta);

            if (Physics.Raycast(_fallVector, -_transform.up, out hitpos, _Chest.multiplier * scaleFactor, GroundLayer))
            {
                fall = false;
                if (Vector3.Angle(hitpos.normal, UpVector) >maxAngleSlope && isJumping()) fall = true; //if the ray found ground but is to Slopy
            }
            else
            {
                fall = true;
            }
        }

        protected virtual void Swimming()
        {
            RaycastHit WaterHitCenter;
            Pivots waterPivot = pivots[2];

            float waterlevel = 0;

            //Front RayWater Cast
            if (Physics.Raycast(waterPivot.transform.position, -_transform.up, out WaterHitCenter, _Height * pivots[2].multiplier, LayerMask.GetMask("Water")))
            {
                waterlevel = WaterHitCenter.point.y; //get the water level when find water
                isInWater = true;
            }
            else
            {
                isInWater = false;
            }

            if (isInWater) //if we hit water
            {
                if (!swim && (Pivot_Chest.Y < waterlevel) || (fall && !isJumping(0.5f, true)))
                {
                    swim = true;
                     _rigidbody.constraints = Still_Constraints;
                }

                //Stop swimming when he is coming out of the water              //Just come out the water when hit the back ray
                else if (hit_Chest.distance < _Height && hit_Hip.distance < _Height + waterLine * 0.5f
                    && !isJumping() && !fall && backray && swim)
                {
                    swim = false;
                }
            }

            if (swim)
            {
                fall = false;
                isInAir = false;

                float angleWater = Vector3.Angle(_transform.up, WaterHitCenter.normal);

                Quaternion finalRot = Quaternion.FromToRotation(_transform.up, WaterHitCenter.normal) * _rigidbody.rotation;

                //Smoothy rotate until is Aling with the Water
                if (angleWater > 0.5f)
                    _transform.rotation = Quaternion.Lerp(_transform.rotation, finalRot, Time.deltaTime * 10);
                else
                    _transform.rotation = finalRot;

                //Smoothy Aling position with the Water
                Vector3 NewPos = new Vector3(_transform.position.x, waterlevel - _Height + waterLine, _transform.position.z);
                _transform.position = Vector3.Lerp(_transform.position, NewPos, Time.deltaTime * 10f);

                if (!isInWater)
                {
                    swim = false;
                }
            }
        }

        // Check for a behind Cliff so it will stop going backwards.
        protected virtual bool IsFallingBackwards(float ammount = 0.5f)
        {
            
            RaycastHit BackRay;
            Vector3 FallingVectorBack = pivots[0].transform.position + _transform.forward * -1 * ammount;

            if (debug) Debug.DrawRay(FallingVectorBack, -_transform.up * pivots[0].multiplier);                 //Draw a White Ray

            if (Physics.Raycast(FallingVectorBack, -_transform.up, out BackRay, scaleFactor * pivots[0].multiplier, GroundLayer))
            {
                return false;
            }
            else
            {
                if (!swim && movementAxis.z < 0) return true; //Check if the animal is moving backwards
            }
            return false;
        }

        // call to prevent Activation a button after a time
        protected IEnumerator SpeeedCount(float seconds)
        {
            if (!speed_Counter)
            {
                SpeedCount++;
                speed_Counter = true;
            }
            yield return new WaitForSeconds(seconds);
            speed_Counter = false;
        }

       
        /// <summary>
        /// Gets the movement from the Input Script or AI
        /// </summary>
        /// <param name="move">Direction VEctor</param>
        /// <param name="active">Active = true means that is taking the direction Move</param>
        public virtual void Move(Vector3 move, bool active = true)
        {
            if (active)
            {
                // convert the world relative moveInput vector into a local-relative
                // turn amount and forward amount required to head in the desired
                // direction.
                if (move.magnitude > 1f) move.Normalize();
                move = transform.InverseTransformDirection(move);

                move = Vector3.ProjectOnPlane(move, SurfaceNormal);
                turnAmount = Mathf.Atan2(move.x, move.z);
                forwardAmount = move.z;

                movementAxis = new Vector3(turnAmount, 0,Mathf.Abs(forwardAmount));

                if (!stand)
                    _transform.Rotate(UpVector, movementAxis.x * Time.deltaTime * 40f); //Add Extra rotation 
            }
            else
                movementAxis = move; //Do not convert to Direction Input Mode (Camera or AI)
        }

        protected void MovementSystem(float s1 = 1, float s2 = 2, float s3 = 3)
        {
            AdditionalTurn();                                       //Apply Additional Turn movement
            AdditionalSpeed();                                      //Apply Speed movement Turn movement

            if (swapSpeed)                                          //if is Active change the speed by shift input instead of 1 2 3
            {
                if (shift)      StartCoroutine(SpeeedCount(0.45f)); //This prevent the the shift input before 0.45f time has passed


                //Change the speed by shift input toggle
                if ((SpeedCount % 3) == 0)
                {
                    speed1 = true;
                    speed3 = speed2 = false;
                }
                else if ((SpeedCount % 3) == 1)
                {
                    speed2 = true;
                    speed1 = Speed3 = false;
                }
                else if ((SpeedCount % 3) == 2)
                {
                    speed3 = true;
                    speed1 = speed2 = false;
                }
            }

            int smooth = 2;

            //Change velocity on ground
            if (speed1) groundSpeed = s1;
            if (speed2) groundSpeed = s2;
            if (speed3) groundSpeed = s3;

            if (groundSpeed == 1) smooth = 8;       //fast change from idle to walk
            if (groundSpeed == 2) smooth = 4;       //medium change from idle to trot
            if (groundSpeed == 4) smooth = 2;       //slow change from idle to run

           
            float maxspeed = groundSpeed;           //Do not override the groundSpeed

            if (swim) maxspeed = 1;
            if (shift && !swapSpeed) maxspeed++;
            
            if (slope >= 0.5 && maxspeed > 1) maxspeed--;       //SlowDown When going UpHill
          
            if (slope >= 1)     //Prevent to go uphill
            {
                maxspeed = 0;
                smooth = 10;
            }
          
            if (movementAxis.z < 0)  //if is walking backwards check for a cliff
            {
                if (IsFallingBackwards() && !swim)
                {
                    maxspeed = 0;
                    smooth = 10;
                }
            }

            speed = Mathf.Lerp(speed, movementAxis.z * maxspeed, Time.deltaTime * smooth);  //smoothly transitions bettwen velocities
            direction = Mathf.Lerp(direction, movementAxis.x * (shift ? 2 : 1), Time.deltaTime * 8f);    //smoothly transitions bettwen directions
           
            if ((movementAxis.x != 0) || (Mathf.Abs(speed) > 0.2f))   //Check if the Character is Standing
                stand = false;
            else stand = true;

            if (jump || damaged || stun || fall || swim || fly || isInAir || tired >= GotoSleep)   stand = false; //Stand False when doing some action
               
            if (tired >= GotoSleep) tired = 0;          //Reset Time Out
        }

        void FixedUpdate()                   //All Raycast Stuff Here
        {
            _currentState = _anim.GetCurrentAnimatorStateInfo(0);
            FixPosition();
            Falling();
            Swimming();
        }

        void Update()
        {
            MovementSystem(movementS1,movementS2,movementS3);
        }

        void LateUpdate()
        {
            LinkingAnimator(_anim);//Set all Animator Parameters
        }


#if UNITY_EDITOR
        /// <summary>
        /// DebugOptions
        /// </summary>
        void OnDrawGizmos()
        {
            if (debug)
            {
                pivots = GetComponentsInChildren<Pivots>();
                Gizmos.color = Color.magenta;
                float sc = transform.localScale.y;

                if (backray)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit_Hip.point, 0.05f * sc);
                }
                if (frontray)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(hit_Chest.point, 0.05f * sc);
                }
            }
        }
#endif
    }
}