using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
[ExecuteInEditMode(), AddComponentMenu("")]
public class CopyObjectTransform : MonoBehaviour {

	public Transform m_Target;
	public Vector3 m_PositionOffset = Vector3.zero;
	public Vector3 m_RotationOffset = Vector3.zero;


	void Start() {
		// if (Application.isPlaying) {
		// 	transform.SetParent(m_Target);
		// 	transform.localPosition = m_PositionOffset;
		// 	transform.localEulerAngles = m_RotationOffset;
		// }
	}
	void LateUpdate () {
		if (m_Target == null)
			return;
		// if (Application.isEditor && !Application.isPlaying)
		// {
			transform.position = m_Target.position;
			transform.Translate(m_PositionOffset, Space.Self);
			transform.rotation = m_Target.rotation;
			transform.Rotate(m_RotationOffset, Space.Self);
			transform.localScale = m_Target.localScale;
		// }
	}
}
}