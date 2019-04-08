using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BL.Splines;

public delegate void SplineEventDelegate();

public class FollowSpline : MonoBehaviour 
{
    public Spline spline;
    public float speed = 15f;
    public bool followRotationX;
    public bool followRotationY;
    public bool followRotationZ;
    public bool loopAutomatically;
    public SplineEventDelegate OnReachEndOfSpline;
    public SplineEventDelegate OnReachPercentageOfSpline;
    [HideInInspector] public float nextNotifyPercentage = 50f;

    Vector3 startRotationAngles;
    Quaternion finalRotation;
    float currentT;
    bool notifyOnPercentage;
    bool setNotifyNextFrame;

	void Update ()
	{
        if (loopAutomatically)
        {
            GoToNextSplinePosition(true);
        }
	}

    public void Setup (Vector3 _startRotationAngles, Quaternion _finalRotation, Spline _spline, SplineEventDelegate _OnReachEndOfSpline)
    {
        startRotationAngles = _startRotationAngles;
        finalRotation = _finalRotation;
        spline = _spline;
        OnReachEndOfSpline = _OnReachEndOfSpline;
    }

    public void NotifyOnPercent (float percent, SplineEventDelegate OnPercent)
    {
        nextNotifyPercentage = percent;
        OnReachPercentageOfSpline = OnPercent;
        setNotifyNextFrame = true;
    }

	public void GoToNextSplinePosition (bool loop) 
    {
        if (spline == null)
        {
            return;
        }

        UpdatePosition( currentT );
        if (followRotationX || followRotationY || followRotationZ)
        {
            UpdateRotation(currentT);
        }

        CheckReachPercent();

        currentT += 0.0001f * speed;
        if (currentT > 1f)
        {
            currentT = loop ? 0 : 1f;
            if (OnReachEndOfSpline != null)
            {
                OnReachEndOfSpline();
            }
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

        Vector3 rotation = Quaternion.LookRotation( tangent, normal ).eulerAngles;
        Quaternion splineRotation = Quaternion.Euler(followRotationX ? rotation.x : startRotationAngles.x, 
                                                     followRotationY ? rotation.y : startRotationAngles.y, 
                                                     followRotationZ ? rotation.z : startRotationAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, 
                                              Quaternion.Slerp(Quaternion.Slerp(splineRotation, finalRotation, Mathf.SmoothStep(0, 1f, 5f * Mathf.Max(0, t - 0.8f))),
                                                               Quaternion.Euler(startRotationAngles),
                                                               Mathf.SmoothStep(0, 1f, 5f * Mathf.Max(0, 0.2f - t))), 
                                              t);
    }

    void CheckReachPercent ()
    {
        if (setNotifyNextFrame)
        {
            notifyOnPercentage = true;
            setNotifyNextFrame = false;
        }

        if (currentT >= nextNotifyPercentage / 100f)
        {
            if (notifyOnPercentage)
            {
                if (OnReachPercentageOfSpline != null)
                {
                    OnReachPercentageOfSpline();
                }
                notifyOnPercentage = false;
            }
        }
    }
}
