using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BL.Vive
{
    public class WalkViveController : ViveController
    {
        public Transform objectToMove;
        public float speed = 0.05f;

        float lastTime = -10f;

        public override void OnTriggerDown ()
        {
            lastTime = Time.time;
        }

        public override void OnTriggerUp()
        {
            if (Time.time - lastTime < 0.25f)
            {
                objectToMove.position += 50f * speed * transform.forward;
            }
        }

        public override void OnTriggerStay ()
        {
            objectToMove.position += speed * transform.forward;
        }
    }
}