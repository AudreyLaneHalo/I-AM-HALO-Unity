using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.Splines
{
    public class BezierSpline : Spline
    {
        // ---------------------------------------------- Length

        public float GetPartialLength(float startT, float endT, int resolution)
        {
            float l = 0;
            float t = startT;
            float inc = (endT - startT) / resolution;
            for (int i = 0; i < resolution; i++)
            {
                l += GetSegmentLength(t, t + inc);
                t += inc;
            }
            return l;
        }

        float GetSegmentLength(float startT, float endT)
        {
            return Vector3.Distance(GetPosition(startT), GetPosition(endT));
        }

        // ---------------------------------------------- Calculation

        public override int n
        {
            get
            {
                return 4;
            }
        }

        protected override void PreCalculateCurve() { }

        protected override float GetTForCalculation(int section, int segment)
        {
            return (section * resolutionPerPoint + segment) / (float)(calculatedPoints.Length - 1);
        }

        protected override Vector3 CalculatePosition(int pointIndex, float t)
        {
            return new Vector3(
                CubicBezier(t, points[0].position.x, points[1].position.x, points[2].position.x, points[3].position.x),
                CubicBezier(t, points[0].position.y, points[1].position.y, points[2].position.y, points[3].position.y),
                CubicBezier(t, points[0].position.z, points[1].position.z, points[2].position.z, points[3].position.z)
            );
        }

        float CubicBezier(float t, float point0, float point1, float point2, float point3)
        {
            return CubicBezier_Point0(t, point0)
                + CubicBezier_Point1(t, point1)
                + CubicBezier_Point2(t, point2)
                + CubicBezier_Point3(t, point3);
        }

        float CubicBezier_Point0(float t, float point0)
        {
            return (1f - t) * (1f - t) * (1f - t) * point0;
        }

        float CubicBezier_Point1(float t, float point1)
        {
            return 3f * (1f - t) * (1f - t) * t * point1;
        }

        float CubicBezier_Point2(float t, float point2)
        {
            return 3f * (1f - t) * t * t * point2;
        }

        float CubicBezier_Point3(float t, float point3)
        {
            return t * t * t * point3;
        }

        protected override Vector3 CalculateTangent(int pointIndex, float t)
        {
            return Vector3.Normalize(new Vector3(
                CubicBezierTangent(t, points[0].position.x, points[1].position.x, points[2].position.x, points[3].position.x),
                CubicBezierTangent(t, points[0].position.y, points[1].position.y, points[2].position.y, points[3].position.y),
                CubicBezierTangent(t, points[0].position.z, points[1].position.z, points[2].position.z, points[3].position.z)
            ));
        }

        float CubicBezierTangent(float t, float point0, float point1, float point2, float point3)
        {
            return CubicBezierTangent_Point0_to_Point1(t, point0, point1)
                + CubicBezierTangent_Point1_to_Point2(t, point1, point2)
                + CubicBezierTangent_Point2_to_Point3(t, point2, point3);
        }

        float CubicBezierTangent_Point0_to_Point1(float t, float point0, float point1)
        {
            return 3f * (1f - t) * (1f - t) * (point1 - point0);
        }

        float CubicBezierTangent_Point1_to_Point2(float t, float point1, float point2)
        {
            return 6f * (1f - t) * t * (point2 - point1);
        }

        float CubicBezierTangent_Point2_to_Point3(float t, float point2, float point3)
        {
            return 3f * t * t * (point3 - point2);
        }

        public float GetTForClosestPoint(Vector3 point)
        {
            return FindClosest(point, 0f, 1f, 10);
        }

        float FindClosest(Vector3 point, float t1, float t2, int iterations)
        {
            float[] distance = new float[]{ Vector3.Distance( point, transform.TransformPoint( GetPosition( t1 ) ) ),
                Vector3.Distance( point, transform.TransformPoint( GetPosition( t2 ) ) ) };
            float middle = (t1 + t2) / 2f;
            if (distance[0] > distance[1])
            {
                if (iterations > 0)
                {
                    return FindClosest(point, middle, t2, iterations - 1);
                }
                return t2;
            }
            else
            {
                if (iterations > 0)
                {
                    return FindClosest(point, t1, middle, iterations - 1);
                }
                return t1;
            }
        }
    }
}