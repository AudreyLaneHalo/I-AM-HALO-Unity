using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MalbersAnimations.Utilities
{
    public static class MalbersTools
    {
        /// <summary>
        /// Calculate a Direction from an origin to a target
        /// </summary>
        /// <param name="origin">The start point to calculate the direction</param>
        /// <param name="Target">The Target</param>
        /// <returns></returns>
        public static Vector3 DirectionTarget(Transform origin, Transform Target, bool normalized = true)
        {
            if (normalized)
                return (Target.position - origin.position).normalized;

            return (Target.position - origin.position);
        }


        /// <summary>
        /// Gets the horizontal angle between two vectors. The calculation
        /// removes any y components before calculating the angle.
        /// </summary>
        /// <returns>The signed horizontal angle (in degrees).</returns>
        /// <param name="From">Angle representing the starting vector</param>
        /// <param name="To">Angle representing the resulting vector</param>
        public static float HorizontalAngle(Vector3 From, Vector3 To, Vector3 Up)
        {
            float lAngle = Mathf.Atan2(Vector3.Dot(Up, Vector3.Cross(From, To)), Vector3.Dot(From, To));
            lAngle *= Mathf.Rad2Deg;

            if (Mathf.Abs(lAngle) < 0.0001f) { lAngle = 0f; }

            return lAngle;
        }

        /// <summary>
        /// Calculate the direction from the center of the Screen
        /// </summary>
        /// <param name="origin">The start point to calculate the direction</param>
        ///  <param name="hitmask">Just use this layers</param>
        public static Vector3 DirectionFromCamera(Transform origin, float x, float y, out RaycastHit hit ,LayerMask hitmask)
        {
            Camera cam = Camera.main;

             hit = new RaycastHit();

            Ray ray = cam.ScreenPointToRay(new Vector2(x * cam.pixelWidth, y * cam.pixelHeight));
            Vector3 dir = ray.direction;

            hit.distance = float.MaxValue;

            RaycastHit[] hits;

            hits = Physics.RaycastAll(ray, 100, hitmask);

            foreach (RaycastHit item in hits)
            {
                if (item.transform.root == origin.transform.root) continue; //Dont Hit anything in this hierarchy
                if (Vector3.Distance(cam.transform.position, item.point) < Vector3.Distance(cam.transform.position, origin.position)) continue; //If I hit something behind me skip
                if (hit.distance > item.distance) hit = item;
            }

            if (hit.distance != float.MaxValue)
            {
                dir = (hit.point - origin.position).normalized;
            }

            return dir;
        }

        /// <summary>
        /// Calculate the direction from the ScreenPoint of the Screen and also saves the RaycastHit Info
        /// </summary>
        /// <param name="origin">The start point to calculate the direction</param>
        ///  <param name="hitmask">Just use this layers</param>
        public static Vector3 DirectionFromCamera(Transform origin, Vector3 ScreenPoint, out RaycastHit hit, LayerMask hitmask)
        {
            Camera cam = Camera.main;

            Ray ray = cam.ScreenPointToRay(ScreenPoint);
            Vector3 dir = ray.direction;

            hit = new RaycastHit();
            hit.distance = float.MaxValue;
            hit.point = ray.GetPoint(100);
            RaycastHit[] hits;

            hits = Physics.RaycastAll(ray, 100, hitmask);

            foreach (RaycastHit item in hits)
            {
                if (item.transform.root == origin.transform.root) continue;                                     //Dont Hit anything in this hierarchy
                if (Vector3.Distance(cam.transform.position, item.point) < Vector3.Distance(cam.transform.position, origin.position)) continue; //If I hit something behind me skip
                if (hit.distance > item.distance) hit = item;
            }

            if (hit.distance != float.MaxValue)
            {
                dir = (hit.point - origin.position).normalized;
            }  
            return dir;
        }

        /// <summary>
        /// Calculate the direction from the center of the Screen
        /// </summary>
        /// <param name="origin">The start point to calculate the direction</param>
        public static Vector3 DirectionFromCamera(Transform origin)
        {
            RaycastHit p;
            return DirectionFromCamera(origin, 0.5f * Screen.width, 0.5f * Screen.height,out p, -1);
        }


        public static Vector3 DirectionFromCamera(Transform origin, Vector3 ScreenCenter)
        {
            RaycastHit p;
            return DirectionFromCamera(origin, ScreenCenter, out p, -1);
        }


        public static Vector3 DirectionFromCameraNoRayCast(Vector3 ScreenCenter)
        {
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(ScreenCenter);

            return ray.direction;
        }
    }
}