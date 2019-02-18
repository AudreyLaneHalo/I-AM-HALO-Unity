using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    Vector3 incrementVector = new Vector3(0, 0.1f, 0);
    float pushSpeed = 0.005f;
    int directionRotate = 1;
    int directionPush = 1;
    float lastPushTime;
    float lastRotateTime;
    Vector3 localOffset;
    float pushTime = 3f;
    float rotateTime = 13f;

    Transform _observer;
    Transform observer
    {
        get
        {
            if (_observer == null)
            {
                _observer = transform.GetChild(0);
                localOffset = observer.forward;
            }
            return _observer;
        }
    }

	void Update ()
    {
		//if (Input.GetKey(KeyCode.Q))
        //{
        //    transform.rotation *= Quaternion.Euler(incrementVector);
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    transform.rotation *= Quaternion.Euler(-incrementVector);
        //}
        transform.rotation *= Quaternion.Euler(directionRotate * incrementVector);
        observer.localPosition += directionPush * pushSpeed * localOffset;

        if (Time.time - lastPushTime > pushTime)
        {
            directionPush *= -1;
            lastPushTime = Time.time;
            pushTime = Random.Range(2f, 4f);
        }

        if (Time.time - lastRotateTime > rotateTime)
        {
            directionRotate *= -1;
            lastRotateTime = Time.time;
            rotateTime = Random.Range(12f, 20f);
        }
    }
}
