using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    /// <summary>
    /// All Callbacks in here
    /// </summary>
    public partial class Animal 
    {

        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }


        ///Call Back from an Activation Zone
        public virtual void ActionEmotion(int ID)
        {
            actionID = ID;
        }

        //Enable/ Disable Actions
        public virtual void EnableAction(bool value)
        {
            Action = value;
        }


        /// Find the direction hit vector and send it to the Damage Behavior with DamageValues
        public virtual void getDamaged(DamageValues DV)
        {
            if (isTakingDamage) return;                     //If is already taking damage skip...

         //   if (DV.Mycenter == DV.Theircenter) return;    //If Im hitting MySelf skip...

            life = life - (DV.Amount - defense);            //Remove some life

            if (life > 0)                                   //If I have some life left play the damage Animation
            {
                damaged = true;                             //Activate the damage so it can be seted on the Animator
                StartCoroutine(isTakingDamageTime(0.2f));   //Prevent to take other hit in 0.2f

                hitDirection = DV.Direction;
            }
            else
            {
                OnDeath();
            }
        }
     

        /// Find the direction hit vector and send it to the Damage Behavior without DamageValues
        public virtual void getDamaged(Vector3 Mycenter, Vector3 Theircenter, float Amount = 0)
        {
            if (isTakingDamage) return;                     //If is already taking damage skip...

            if (Mycenter == Theircenter) return;            //If Im hitting MySelf skip...

            life = life - (Amount - defense);               //Remove some life

            if (life > 0)                                   //If I have some life left play the damage Animation
            {
                damaged = true;                             //Activate the damage so it can be seted on the Animator
                StartCoroutine(isTakingDamageTime(0.2f));   //Prevent to take other hit in 0.2f
                hitDirection = (-Mycenter + Theircenter).normalized; //Gets the Hit Direction Vector
            }
            else
            {
                Debug.Log(death);
                OnDeath();
            }
        }

        //Coroutine to avoid been hit and play damage animation twice
        IEnumerator isTakingDamageTime(float time)
        {
            isTakingDamage = true;
            yield return new WaitForSeconds(time);
            isTakingDamage = false;
        }

        public virtual void OnDeath()
        {
            if (!death)
            {
                death = true;
                _anim.SetTrigger(HashIDsAnimal.deathHash);  //Activate Death Animation
                //IMount IsAMount = GetComponent<IMount>();

                //if (IsAMount !=null) IsAMount.CanBeMounted = false;
               
            }
        }

        public void Attacking(bool attack)
        {
            isAttacking = attack;
        }

        public void SetIntID(int value)
        {
            idInt = value;
        }

        public void SetFloatID(float value)
        {
            idfloat = value;
        }
        /// <summary>
        /// Set a Random number to ID Int , that work great for randomly Play More animations
        /// </summary>
        public virtual void SetIntIDRandom(int range)
        {
            idInt = Random.Range(1, range + 1);
        }

        /// <summary>
        /// This will check is the Animal is in any Jump State
        /// </summary>
        /// <param name="normalizedTime">The normalized time of the Jump Animation</param>
        /// <param name="half">True to check if is the First Half, False to check the Second Half</param>
        /// <returns></returns>
        public virtual bool isJumping(float normalizedTime, bool half)
        {
            if (half)  //if is jumping the first half
            {

                if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
                {
                    if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= normalizedTime)
                        return true;
                }

                if (_anim.GetNextAnimatorStateInfo(0).IsTag("Jump"))  //if is transitioning to jump
                {
                    if (_anim.GetNextAnimatorStateInfo(0).normalizedTime <= normalizedTime)
                        return true;
                }
            }
            else //if is jumping the second half
            {
                if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
                {
                    if (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedTime)
                        return true;
                }

                if (_anim.GetNextAnimatorStateInfo(0).IsTag("Jump"))  //if is transitioning to jump
                {
                    if (_anim.GetNextAnimatorStateInfo(0).normalizedTime > normalizedTime)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This will check is the Animal is in any Jump State
        /// </summary>
        public virtual bool isJumping()
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
            {
                return true;
            }
            if (_anim.GetNextAnimatorStateInfo(0).IsTag("Jump"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Toogle the RigidBody Constraints
        /// </summary>
        public virtual void StillConstraints(bool active)
        {
            if (active)                                     //Activate Constraints
                _rigidbody.constraints = Still_Constraints;

            else                                            //DeActivate Constraints
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public virtual void EnableColliders(bool active)  
        {
            Collider[] cap = GetComponentsInChildren<Collider>();
            foreach (Collider item in cap) item.enabled = active;
        }

        public virtual void IsinAir(bool active)
        {
            isInAir = active;
            StillConstraints(!active);
        }

        public virtual void EnableJump()
        {

        }
    }
}
