using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using MalbersAnimations.Events;


namespace MalbersAnimations.Weapons
{
    public class MArrow : MonoBehaviour, IArrow
    {
        #region Variables
        [Space]
        [Tooltip("Damage this Arrow Causes")]
        public float damage  = 1;                                   //Damage this Arrow Causes       
        [Tooltip("Penetration to the hitted mesh")]
        public float Penetration = 0.2f;                            //Penetration to the next mesh
        [Tooltip("How long the arrow is stay alive after hit Something")]
        public float AgeAfterImpact = 10f;                          //How long the arrow is stay alive after hit Something
        [Tooltip("How long the arrow is alive flying (Used for removing arrows shooted off the map)")]
        public float AgeFlying = 30f;                               //How long the arrow is alive flying (Used for shooting arrows off the map)
        [Tooltip("Damage this Arrow Causes")]
        public float tailOffset = 1;                                //Offset from the Tip to the End of the arrow 

        public bool AffectRigidBodies = true;

        [Space]
        [Header("Events")]

        public RayCastHitEvent OnHitTarget;                  //Send the transform to the event

        protected LayerMask hitmask;                        //What to Hit
        protected float force;
        protected Rigidbody _rigidbody;
        protected Vector3 DeltaPos;
        protected bool isflying;

        [HideInInspector] public RaycastHit HitPoint;

        WaitForEndOfFrame WfeoF = new WaitForEndOfFrame();

        #endregion
        Rigidbody mRigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = GetComponent<Rigidbody>();
                }
                return _rigidbody;
            }
        }

        public LayerMask HitMask
        {
            get { return hitmask; }
            set { hitmask = value; }
        }

        public float TailOffset
        {
            get { return tailOffset; }
            set { tailOffset = value; }
        }

        public float Damage
        {
            get { return damage; }
            set { damage = value; }
        }


        public bool isFlying
        {
            get { return isflying; }
        }

        /// <summary>
        /// Shoots the Arrow in a given direciton
        /// </summary>
        /// <param name="force"></param>
        /// <param name="direction"></param>
        public virtual void ShootArrow(float force, Vector3 direction)
        {
            this.force = force;
            mRigidbody.constraints = RigidbodyConstraints.None;
            mRigidbody.isKinematic = false;
            mRigidbody.useGravity = true;
            mRigidbody.AddForce(direction * force);
           // mRigidbody.velocity = direction * force;
            isflying = true;
            StartCoroutine(FlyingArrow());
            StartCoroutine(FlyingAge());
        }

        /// <summary>
        /// To avoid that the arrow flyies for all eternity
        /// </summary>
        IEnumerator FlyingAge()
        {
            yield return new WaitForSeconds(AgeFlying);
            if (isflying)
            {
                isflying = false;
                Destroy(gameObject);
                StopAllCoroutines();
            }
        }
      

        /// <summary>
        /// Call This if some else Shoot the Arrow, if the arrow is already flying check for collision to invoke ONHIT
        /// </summary>
        public virtual void TestFlyingArrow()
        {
            StopAllCoroutines();
            StartCoroutine(TestFly());
            StartCoroutine(FlyingAge());
        }


        IEnumerator TestFly()
        {
            isflying = true;
            DeltaPos = transform.position;

            yield return null;

            RaycastHit hit = new RaycastHit();

            while (isflying)
            {
                float Prediction = Mathf.Abs((transform.position - DeltaPos).magnitude) + 0.3f; //Prediction to Hit something in the way

                if (Physics.Raycast(transform.position, mRigidbody.velocity.normalized, out hit, Prediction, hitmask))
                {
                    isflying = false;
                }
                else
                {
                    Debug.DrawRay(transform.position, mRigidbody.velocity.normalized * Prediction, Color.red);

                    DeltaPos = transform.position;
                }

                yield return null;
            }

            TestHit(hit);
        }

        IEnumerator FlyingArrow()
        {
            RaycastHit hit;

            yield return new WaitForEndOfFrame();
            while (isflying)
            {
                float Prediction = Mathf.Abs((transform.position - DeltaPos).magnitude) + 0.3f; //Prediction to Hit something in the way

                if (Physics.Raycast(transform.position, mRigidbody.velocity.normalized, out hit, Prediction, hitmask))
                {
                    OnHit(hit);
                    isflying = false;
                }
                else
                {
                    Debug.DrawRay(transform.position, mRigidbody.velocity.normalized * Prediction, Color.red);
                    if (mRigidbody.velocity.magnitude > 0)
                    {
                        transform.rotation = Quaternion.LookRotation(mRigidbody.velocity.normalized, transform.up);
                    }

                    yield return WfeoF;
                    DeltaPos = transform.position;
                    yield return null;
                }
            }
        }


        /// <summary>
        /// Will Send
        /// </summary>
        /// <param name="other"></param>
        public virtual void TestHit(RaycastHit other)
        {
            DamageValues DV = new DamageValues(-transform.forward, damage);

            if (other.transform)
            {
                other.transform.SendMessageUpwards("getDamaged", DV, SendMessageOptions.DontRequireReceiver);

                if (other.rigidbody && AffectRigidBodies)
                {
                    other.rigidbody.AddForceAtPosition(transform.forward * force, other.point);
                }
            }
            OnHitTarget.Invoke(other);
        }

        public virtual void OnHit(RaycastHit other)
        {
            DamageValues DV = new DamageValues(-transform.forward, damage);

            if (other.transform)
            {
                other.transform.SendMessageUpwards("getDamaged", DV, SendMessageOptions.DontRequireReceiver);

                if (other.rigidbody && AffectRigidBodies)
                {
                    other.rigidbody.AddForceAtPosition(transform.forward * force, other.point);
                }
            }

            transform.position = other.point + transform.forward * Penetration; //Put the arrow a bit deeper in the collider

            mRigidbody.isKinematic = true;
            mRigidbody.constraints = RigidbodyConstraints.FreezeAll;


                //FIX FOR SCALED OBJECTS
                Vector3 NewScale = other.transform.lossyScale;
                NewScale.x = 1f / Mathf.Max(NewScale.x, 0.0001f);
                NewScale.y = 1f / Mathf.Max(NewScale.y, 0.0001f);
                NewScale.z = 1f / Mathf.Max(NewScale.z, 0.0001f);

                GameObject Hlper = new GameObject();
                Hlper.name = name + "Link";

                Hlper.transform.parent = other.collider.transform;
                Hlper.transform.localScale = NewScale;
                Hlper.transform.position = other.point;
                //Hlper.transform.localPosition = other.transform.InverseTransformPoint(other.point);
                Hlper.transform.localRotation = Quaternion.identity;

                transform.parent = Hlper.transform;
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;


            Destroy(Hlper, AgeAfterImpact);
            Destroy(gameObject, AgeAfterImpact);

            OnHitTarget.Invoke(other);
        }
    }
}