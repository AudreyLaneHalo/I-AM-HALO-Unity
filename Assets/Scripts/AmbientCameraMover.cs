using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientCameraMover : MonoBehaviour
{
    public float rotateSpeed = 5f;
    public float pushSpeed = 5f;
    public bool ambientlyRotate = true;

    float pushTime = 3f;
    float lastPushTime = -100f;
    float startPush;
    float goalPush;
    int pushDirection = 1;

    float rotateTime = 13f;
    float lastRotateTime = -100f;
    Quaternion startRotation;
    Quaternion goalRotation;

    Transform _observer;
    Transform observer
    {
        get
        {
            if (_observer == null)
            {
                _observer = transform.GetChild(0);
            }
            return _observer;
        }
    }

	void Update ()
    {
        if (ambientlyRotate)
        {
            AmbientlyRotate();
        }
    }

    void AmbientlyRotate ()
    {
        if (Time.time - lastPushTime > pushTime)
        {
            startPush = observer.localPosition.magnitude;
            goalPush = startPush + pushDirection * pushTime * 0.01f * pushSpeed * Random.value;
            pushDirection *= -1;
            lastPushTime = Time.time;
            pushTime = Random.Range(2f, 4f);
        }

        if (Time.time - lastRotateTime > rotateTime)
        {
            startRotation = transform.rotation;
            goalRotation = Quaternion.Euler(transform.eulerAngles +  
                                            new Vector3(Random.Range(-20f, 20f), rotateTime * rotateSpeed * Random.value, 0));
            lastRotateTime = Time.time;
            rotateTime = Random.Range(12f, 20f);
        }

        transform.rotation = Quaternion.Slerp(startRotation, goalRotation, Mathf.SmoothStep(0, 1f, (Time.time - lastRotateTime) / rotateTime));
        observer.localPosition = Vector3.Lerp(startPush * Vector3.back, goalPush * Vector3.back, Mathf.SmoothStep(0, 1f, (Time.time - lastPushTime) / pushTime));
    }
}
