using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
    public class Rotator : MonoBehaviour 
    {
        public bool rotating;

        Quaternion startRotation;
        Quaternion goalRotation;
        float t;
        float speed;

        public void RotateToOverDuration (Quaternion _goalRotation, float _duration)
        {
            speed = 1f / _duration;

            RotateTo( _goalRotation );
        }

        public void RotateToWithSpeed (Quaternion _goalRotation, float _speed)
        {
            float angle = Mathf.PI * Quaternion.Angle(transform.rotation, _goalRotation) / 180f;
            speed = 1f / (angle / _speed);

            RotateTo( _goalRotation );
        }

        void RotateTo (Quaternion _goalRotation)
        {
            startRotation = transform.rotation;
            goalRotation = _goalRotation;
            t = 0;
            rotating = true;
        }

        void Update () 
        {
            if (rotating)
            {
                t += speed * Time.deltaTime;
                if (t >= 1f)
                {
                    t = 1f;
                    rotating = false;
                }

                transform.rotation = Quaternion.Slerp( transform.rotation, goalRotation, t );
            }
        }

        public void SnapToGoal ()
        {
            rotating = false;
            transform.rotation = goalRotation;
        }
    }
}