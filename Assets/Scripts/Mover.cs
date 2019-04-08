using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
    public delegate void CompletionDelegate();

    public class Mover : MonoBehaviour
    {
        public bool moving;

        Vector3 startPosition;
        Vector3 goalPosition;
        float t;
        float speed;
        CompletionDelegate callback;

        public void MoveToOverDuration(Vector3 _goalPosition, float _duration, CompletionDelegate _callback)
        {
            callback = _callback;
            speed = 1f / _duration;

            MoveTo(_goalPosition);
        }

        public void MoveToWithSpeed(Vector3 _goalPosition, float _speed, CompletionDelegate _callback)
        {
            callback = _callback;
            float distance = Vector3.Distance(transform.position, _goalPosition);
            speed = 1f / (distance / _speed);

            MoveTo(_goalPosition);
        }

        void MoveTo(Vector3 _goalPosition)
        {
            startPosition = transform.position;
            goalPosition = _goalPosition;
            t = 0;
            moving = true;
        }

        void Update()
        {
            if (moving)
            {
                t += speed * Time.deltaTime;
                if (t >= 1f)
                {
                    t = 1f;
                    moving = false;
                    callback();
                }

                transform.position = Vector3.Lerp(startPosition, goalPosition, t);
            }
        }
    }
}