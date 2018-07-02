using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
public class MoveWithMouse : MonoBehaviour {
	public float maxForwardDistance = 0.5f;

	Vector3 initialPosition;
	Vector3 initialForward;

	void Start()
	{
		initialPosition = transform.localPosition;
		initialForward = transform.forward;
	}

	void Update () {
		var viewportPt = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		var factor = viewportPt.x;
		transform.localPosition = initialPosition + initialForward * factor * maxForwardDistance;
	}
}
}