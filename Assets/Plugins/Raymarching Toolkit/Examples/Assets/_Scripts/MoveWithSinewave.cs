using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
public class MoveWithSinewave : MonoBehaviour {

	public Vector3 m_Distance = Vector3.one;
	public Vector3 m_Frequency = Vector3.one;
	
	Vector3 initialPos;

	void Start () {
		initialPos = transform.localPosition;
	}
	
	void Update () {
		transform.localPosition = initialPos + new Vector3(
			Mathf.Sin(Time.time * m_Frequency.x) * m_Distance.x,
			Mathf.Sin(Time.time * m_Frequency.y) * m_Distance.y,
			Mathf.Sin(Time.time * m_Frequency.z) * m_Distance.z
		);
	}
}
}