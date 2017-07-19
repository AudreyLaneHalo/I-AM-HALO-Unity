using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveInputManager : MonoBehaviour 
{
	public bool pinching;
	public float pinchDelta;
	public float pinchMultiplier = 1f;

	float lastDistanceBetweenControllers;

	List<ZoomViveController> controllers = new List<ZoomViveController>();

	public void StartPinch (ZoomViveController controller)
	{
		if (!controllers.Contains( controller ))
		{
			controllers.Add( controller );
		}

		ZoomViveController otherController = controllers.Find( c => c != controller );
		if (otherController != null && otherController.triggerDown)
		{
			lastDistanceBetweenControllers = Vector3.Distance( controller.transform.position, otherController.transform.position );
			pinching = true;
		}
	}

	public void StopPinch ()
	{
		pinching = false;
	}

	void Update ()
	{
		controllers.RemoveAll( c => c == null );

		if (pinching && controllers.Count == 2)
		{
			float distanceBetweenControllers = Vector3.Distance( controllers[0].transform.position, controllers[1].transform.position );
			pinchDelta = distanceBetweenControllers - lastDistanceBetweenControllers;
			lastDistanceBetweenControllers = distanceBetweenControllers;

			InputManager.Instance.Zoom( pinchMultiplier * pinchDelta );
		}
	}
}
