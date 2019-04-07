using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BL.Splines;

public class FollowSpline : MonoBehaviour 
{
    public Spline spline;
    public float speed = 1f;
    public bool updateRotation;

    float currentT;

	void Update () 
    {
        if (spline == null)
        {
            return;
        }

        UpdatePosition( currentT );
        if (updateRotation)
        {
            UpdateRotation(currentT);
        }

        currentT += 0.01f * speed;
        if (currentT > 1)
        {
            currentT = 0;
        }
	}

    void UpdatePosition (float t)
    {
        transform.position = spline.GetPosition( t );
    }

    void UpdateRotation (float t)
    {
        Vector3 normal = spline.GetNormal( t );
        Vector3 tangent = spline.GetTangent( t );

        transform.rotation = Quaternion.LookRotation( tangent, normal );
    }
}
