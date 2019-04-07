using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.Splines
{
    public class CubicSplinePosition
    {
        public int pointIndex;
        public float sectionT;

        public CubicSplinePosition (int _pointIndex, float _sectionT)
        {
            pointIndex = _pointIndex;
            sectionT = _sectionT;
        }
    }

    public class NaturalCubicSpline : Spline 
    {
        // ---------------------------------------------- Calculation

        public override int n 
        {
            get
            {
                return points.Length;
            }
        }

        float[][] linearSystemSolution;

        ArbitraryMatrix coefficientMatrix
        {
            get 
            {
                ArbitraryMatrix a = new ArbitraryMatrix( n, n );
                for (int r = 1; r < n - 1; r++)
                {
                    a[r][r] = 4f;
                    a[r][r-1] = a[r][r+1] = 1f;
                }
                a[0][0] = a[n-1][n-1] = 1;
                return a;
            }
        }

        protected override void PreCalculateCurve ()
        {
            linearSystemSolution = new float[3][];
            for (int axis = 0; axis < 3; axis++)
            {
                float[] b = new float[n];
                for (int i = 1; i < n - 1; i++)
                {
                    b[i] = points[i + 1].position[axis] - 2f * points[i].position[axis] + points[i - 1].position[axis];
                }
                linearSystemSolution[axis] = coefficientMatrix.inverse * b;
            }
        }

        protected override float GetTForCalculation (int section, int segment)
        {
            return segment / (float)resolutionPerPoint;
        }

        protected override Vector3 CalculatePosition (int pointIndex, float sectionT)
        {
            Vector3 position = Vector3.zero;
            for (int axis = 0; axis < 3; axis++)
            {
                float ct = points[pointIndex + 1].position[axis] - linearSystemSolution[axis][pointIndex + 1];
                float c1t = points[pointIndex].position[axis] - linearSystemSolution[axis][pointIndex];
                position[axis] = linearSystemSolution[axis][pointIndex + 1] * Mathf.Pow( sectionT, 3f ) 
                    + linearSystemSolution[axis][pointIndex] * Mathf.Pow( 1f - sectionT, 3f ) + ct * sectionT + c1t * (1f - sectionT);
            }
            return position;
        }

        protected override Vector3 CalculateTangent (int pointIndex, float sectionT)
        {
            Vector3 tangent = Vector3.zero;
            for (int axis = 0; axis < 3; axis++)
            {
                float ct = points[pointIndex + 1].position[axis] - linearSystemSolution[axis][pointIndex + 1];
                float c1t = points[pointIndex].position[axis] - linearSystemSolution[axis][pointIndex];
                tangent[axis] = 3f * linearSystemSolution[axis][pointIndex + 1] * Mathf.Pow( sectionT, 2f ) 
                    - 3f * linearSystemSolution[axis][pointIndex] * Mathf.Pow( 1f - sectionT, 2f ) + ct - c1t;
            }
            return Vector3.Normalize( tangent );
        }
    }
}