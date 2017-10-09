using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientRotation : MonoBehaviour
{
    public float dRotation = 0.01f;
    public float meanTimeInterval = 0.5f;

    Quaternion startRotation;
    Quaternion goalRotation;
    float currentTimeInterval = 0;
    float lastTime = -1000f;

	void Update ()
    {
        if (Time.time - lastTime > currentTimeInterval)
        {
            SetGoalRotation();
        }

        transform.rotation = Quaternion.Slerp(startRotation, goalRotation, (Time.time - lastTime) / currentTimeInterval);
    }

    void SetGoalRotation ()
    {
        startRotation = transform.rotation;
        goalRotation = transform.rotation * Quaternion.Euler( Random.Range( -dRotation, dRotation ), Random.Range( -dRotation, dRotation ), Random.Range( -dRotation, dRotation ) );
        currentTimeInterval = Random.Range( 0.5f * meanTimeInterval, 1.5f * meanTimeInterval );
        lastTime = Time.time;
    }
}
