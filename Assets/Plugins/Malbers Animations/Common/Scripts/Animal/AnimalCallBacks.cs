using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    /// All Callbacks in here
    public partial class Animal
    {

        public virtual void OnAnimatorBehaviourMessage(string message, object value)
        {
            this.InvokeWithParams(message, value);
        }

        //Call Back from an Activation Zone

        /// <summary>
        /// Set a specifi
        /// </summary>
        /// <param name="ID"></param>
        public virtual void ActionEmotion(int ID)
        {
            actionID = ID;
        }

        /// <summary>
        /// Enable/ Disable Actions
        /// </summary>
        public virtual void EnableAction(bool value)
        {
            Action = value;
        }


        /// <summary>
        /// Find the direction hit vector and send it to the Damage Behavior with DamageValues
        /// </summary>
        public virtual void getDamaged(DamageValues DV)
        {
            if (isTakingDamage) return;                             //If is already taking damage skip...
            OnGetDamaged.Invoke();
            life = life - (DV.Amount - defense);                    //Remove some life

            actionID = -1;                                          //If it was doing an action Stop!;

            if (life > 0)                                           //If I have some life left play the damage Animation
            {
                damaged = true;                                     //Activate the damage so it can be seted on the Animator
                StartCoroutine(isTakingDamageTime(damageDelay));    //Prevent to take other hit after a time.

                hitDirection = DV.Direction;
            }
            else
            {
                Death = true;
            }
        }

        /// Find the direction hit vector and send it to the Damage Behavior without DamageValues
        public virtual void getDamaged(Vector3 Mycenter, Vector3 Theircenter, float Amount = 0)
        {
            DamageValues DV = new DamageValues(Mycenter - Theircenter, Amount);
            getDamaged(DV);
        }

        //Coroutine to avoid been hit and play damage animation twice
        IEnumerator isTakingDamageTime(float time)
        {
            isTakingDamage = true;
            yield return new WaitForSeconds(time);
            isTakingDamage = false;
        }

        /// <summary>
        /// Get if the animal is currently attacking. This called form the animator when an attack animation started
        /// </summary>
        /// <param name="attack"></param>
        public void Attacking(bool attack)
        {
            isAttacking = attack;
            if (attack) OnAttack.Invoke();
        }


        /// <summary>
        /// Activate a random Attack
        /// </summary>
        public virtual void SetAttack()
        {
            attack1 = true;
            OnAttack.Invoke();
        }

        /// <summary>
        /// Activate an Attack by his Animation State Name
        /// </summary>
        public virtual void SetAttack(string animation)
        {
            attack1 = true;
            if (!isAttacking) Anim.Play(animation);
            OnAttack.Invoke();
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

        /// <summary>
        /// Set the animal if is in air. 
        /// True: it will deactivate the Rigidbody constraints. 
        /// False: will freeze all rotations and Y position on the rigidbody.
        /// </summary>
        /// <param name="active"></param>
        public virtual void IsinAir(bool active)
        {
            isInAir = active;
            StillConstraints(!active);
        }

        /// <summary>
        /// Activate the Jump and deactivate it 2 frames later
        /// </summary>
        public virtual void SetJump()
        {
            StartCoroutine(ToogleJump());
        }

        /// <summary>
        /// Set an Action using their Action ID (Find the IDs on the Animator Actions Transitions)
        /// </summary>
        /// <param name="ID"></param>
        public virtual void SetAction(int ID)
        {
            action = true;
            StartCoroutine(ToogleAction());
            actionID = ID;
        }

        /// <summary>
        /// Set an Action using their Action ID (Find the IDs on the Animator Actions Transitions)
        /// </summary>
        /// <param name="actionName">Name of the Animation State</param>
        public virtual void SetAction(string actionName)
        {
            Anim.CrossFade(actionName, 0.1f);
        }

        /// <summary>
        /// Set the Stun to true for a time
        /// </summary>
        public virtual void SetStun(float time)
        {
            StartCoroutine(toogleStun(time));
        }

        internal IEnumerator ToogleJump()
        {
            Jump = true;
            yield return null;
            yield return null;
            Jump = false;
        }

        internal IEnumerator ToogleAction()
        {
            yield return null;
            yield return null;
            action = false;
        }

        internal IEnumerator toogleStun(float time)
        {
            Stun = true;
            yield return new WaitForSeconds(time);
            stun = false;
        }
    }
}
