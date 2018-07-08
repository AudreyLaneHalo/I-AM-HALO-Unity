using UnityEngine;

namespace BL.Vive
{
    public enum PlaceAndScaleControllerState
    {
        Idle,
        Holding,
        Scaling
    }

    public class PlaceAndScaleController : ViveController
    {
        public PlaceAndScaleControllerState state;
        public PlaceAndScaleController otherController;

        public Selectable selectedObj
        {
            get
            {
                return GetComponentInChildren<Selectable>();
            }
        }

        Selectable hoveredObj;
        float startControllerDistance;
        Vector3 startScale;
        Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 maxScale = new Vector3(25f, 25f, 25f);

        void OnTriggerEnter (Collider other)
        {
            Selectable obj = other.GetComponent<Selectable>();
            if (obj != null)
            {
                hoveredObj = obj;
            }
        }

        void OnTriggerExit(Collider other)
        {
            Selectable obj = other.GetComponent<Selectable>();
            if (obj != null && hoveredObj == obj)
            {
                hoveredObj = null;
            }
        }

        public override void OnGripDown()
        {
            if (state == PlaceAndScaleControllerState.Idle)
            {
                if (otherController.state == PlaceAndScaleControllerState.Holding)
                {
                    StartScaling();
                }
                else
                {
                    PickupObj();
                }
            }
        }

        public override void OnGripStay()
        {
            if (state == PlaceAndScaleControllerState.Scaling && otherController.state == PlaceAndScaleControllerState.Holding)
            {
                UpdateScale();
            }
        }

        public override void OnGripUp()
        {
            if (state == PlaceAndScaleControllerState.Scaling)
            {
                StopScaling();
            }
            else if (state == PlaceAndScaleControllerState.Holding)
            {
                ReleaseObj();
            }
        }

        public void SwitchToPrimary()
        {
            if (state == PlaceAndScaleControllerState.Scaling)
            {
                StopScaling();
                PickupObj();
            }
        }

        Vector3 ClampScale(Vector3 scale)
        {
            if (scale.magnitude > maxScale.magnitude)
            {
                return maxScale;
            }
            else if (scale.magnitude < minScale.magnitude)
            {
                return minScale;
            }
            else
            {
                return scale;
            }
        }

        void PickupObj()
        {
            if (hoveredObj != null)
            {
                hoveredObj.transform.SetParent(transform);
                state = PlaceAndScaleControllerState.Holding;
            }
        }

        void ReleaseObj()
        {
            Selectable obj = selectedObj;
            if (obj != null)
            {
                obj.transform.SetParent(null);
            }
            state = PlaceAndScaleControllerState.Idle;
            otherController.SwitchToPrimary();
        }

        void StartScaling()
        {
            Selectable obj = otherController.selectedObj;
            if (obj != null)
            {
                startControllerDistance = Vector3.Distance(transform.position, otherController.transform.position);
                startScale = obj.transform.localScale;
                state = PlaceAndScaleControllerState.Scaling;
            }
        }

        void UpdateScale()
        {
            Selectable obj = otherController.selectedObj;
            if (obj != null)
            {
                float d = Vector3.Distance(transform.position, otherController.transform.position);
                obj.transform.localScale = ClampScale((d / startControllerDistance) * startScale);
            }
        }

        void StopScaling()
        {
            state = PlaceAndScaleControllerState.Idle;
        }
    }
}