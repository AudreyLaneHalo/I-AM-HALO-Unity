using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDController : MonoBehaviour 
{
	public float speed = 0.1f;

	void Update () 
	{
		Vector3 dPosition = Vector3.zero;
		if (Input.GetKey( KeyCode.W ))
		{
			dPosition += Vector3.forward;
		}
		if (Input.GetKey( KeyCode.A ))
		{
			dPosition += Vector3.left;
		}
		if (Input.GetKey( KeyCode.S ))
		{
			dPosition += Vector3.back;
		}
		if (Input.GetKey( KeyCode.D ))
		{
			dPosition += Vector3.right;
		}
		if (Input.GetKey( KeyCode.UpArrow ))
		{
			dPosition += Vector3.up;
		}
		if (Input.GetKey( KeyCode.DownArrow ))
		{
			dPosition += Vector3.down;
		}
		transform.position += speed * dPosition.normalized;
	}
}
