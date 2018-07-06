using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BL.Vive
{
    public class DrawViveController : ViveController
    {
        public float segmentLength = 0.1f;
        public float lineWidth = 0.2f;

        DrawLine _drawer;
        DrawLine drawer
        {
            get
            {
                if (_drawer == null)
                {
                    _drawer = gameObject.AddComponent<DrawLine>();
                    _drawer.minimumPointDistance = segmentLength;
                    _drawer.lineWidth = lineWidth;
                }
                return _drawer;
            }
        }

        public override void OnTriggerDown()
        {
            Debug.Log( "draw" );
            drawer.canDraw = true;
        }

        public override void OnTriggerUp()
        {
            drawer.canDraw = false;
        }
    }
}