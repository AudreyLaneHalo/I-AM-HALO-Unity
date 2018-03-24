using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class Walk : MonoBehaviour 
{
	public Transform objectToMove;
	public float speed = 0.05f;

	float lastTime = -10f;

	void Update ()
	{
		UnityEngine.XR.XRSettings.enabled = false;
		Camera.main.transform.rotation = InputTracking.GetLocalRotation( XRNode.CenterEye );
		Camera.main.ResetAspect();

		if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				OnTriggerDown();
			}
			else if (Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				OnTriggerUp();
			}
			else
			{
				OnTriggerStay();
			}
		}
	}

	void OnTriggerDown ()
	{
		lastTime = Time.time;
	}

	void OnTriggerUp()
	{
		if (Time.time - lastTime < 0.25f)
		{
			objectToMove.position += 50f * speed * transform.forward;
		}
	}

	void OnTriggerStay ()
	{
		objectToMove.position += speed * transform.forward;
	}
}
