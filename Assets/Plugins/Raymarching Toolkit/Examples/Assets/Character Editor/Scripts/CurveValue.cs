using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RaymarchingToolkit.Examples {
public class CurveValue : MonoBehaviour {
	public AnimationCurve m_Curve = AnimationCurve.Linear(0,0,1,1);
	[System.Serializable] public class FloatEvent : UnityEvent<float> { };
	public FloatEvent onValueChanged = new FloatEvent();

	public float m_MinValue = 0.0f;
	public float m_MaxValue = 1.0f;

	public void SetValue(float normalizedValue) {
		onValueChanged.Invoke(Mathf.Lerp(m_MinValue, m_MaxValue, m_Curve.Evaluate(normalizedValue)));
	}
}
}