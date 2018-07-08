using UnityEngine;

namespace BL.Vive
{
    [RequireComponent( typeof(DrawLine) )]
    public class DrawViveController : ViveController
    {
        DrawLine _drawer;
        DrawLine drawer
        {
            get
            {
                if (_drawer == null)
                {
                    _drawer = gameObject.GetComponent<DrawLine>();
                }
                return _drawer;
            }
        }

        public override void OnTriggerDown()
        {
            drawer.canDraw = true;
        }

        public override void OnTriggerUp()
        {
            drawer.canDraw = false;
        }
    }
}