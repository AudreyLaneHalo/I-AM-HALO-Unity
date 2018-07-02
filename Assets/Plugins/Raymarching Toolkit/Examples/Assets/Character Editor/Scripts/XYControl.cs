using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RaymarchingToolkit.Examples {
[RequireComponent(typeof(Button))]
public class XYControl : MonoBehaviour, IDragHandler, IPointerDownHandler {

	public Button m_Knob;
	public RectTransform m_AxisX;
	public RectTransform m_AxisY;
	public Vector2 m_MinValue = Vector2.zero;
	public Vector2 m_MaxValue = Vector2.one;
	public AnimationCurve m_CurveX = AnimationCurve.Linear(0,0,1,1);
	public AnimationCurve m_CurveY = AnimationCurve.Linear(0,0,1,1);

	public float m_Padding = 0.1f;
	public bool m_Radial = false;

	[System.Serializable]
	public class XAxisEvent : UnityEvent<float> { }
	[System.Serializable]
	public class YAxisEvent : UnityEvent<float> { }
	[System.Serializable]
	public class XYAxisEvent : UnityEvent<Vector2> { }
	public XAxisEvent onXAxisChanged;
	public XAxisEvent onYAxisChanged;
	public XYAxisEvent onValueChanged = new XYAxisEvent();

	protected Vector2 m_NormalizedValue = Vector2.zero;
	protected Vector2 m_ResultValue = Vector2.zero;
	
	RectTransform _rt;
	RectTransform rectTransform { get {
		if (!_rt)
			_rt = GetComponent<RectTransform>();
		return _rt;
	}}

	public void OnDrag(PointerEventData data) {
		ProcessEvent(data);
	}
	public void OnPointerDown(PointerEventData data) {
		ProcessEvent(data);
	}

	void ProcessEvent(PointerEventData data) {
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		float w = corners[2].x - corners[0].x;
		float h = corners[2].y - corners[0].y;

		Vector2 lp;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, data.pressEventCamera, out lp);
		var v = new Vector2(
			( Mathf.Clamp(lp.x / w,-1,1) + 1 ) / 2, 
			( Mathf.Clamp(lp.y / h,-1,1) + 1 ) / 2
		);
		SetValue(v, true);
	}

	public virtual void SetValue(Vector2 normalizedValue, bool setCursor = true) {
		if (m_Radial) {
			float r = .5f;
			var c = Vector2.one * .5f;
			float d = Vector2.Distance(normalizedValue, c);
			if (d > r) {
				normalizedValue = normalizedValue - c;
				normalizedValue.Scale(Vector2.one * (r / d));
				normalizedValue = c + normalizedValue;
			}
		}
		normalizedValue.x = Mathf.Clamp01((normalizedValue.x - m_Padding) / (1-m_Padding));
		normalizedValue.y = Mathf.Clamp01((normalizedValue.y - m_Padding) / (1-m_Padding));
		m_NormalizedValue = normalizedValue;
		m_ResultValue.x = Mathf.Lerp(m_MinValue.x, m_MaxValue.x, m_CurveX.Evaluate(normalizedValue.x));
		m_ResultValue.y = Mathf.Lerp(m_MinValue.y, m_MaxValue.y, m_CurveY.Evaluate(normalizedValue.y));
		onXAxisChanged.Invoke(m_ResultValue.x);
		onYAxisChanged.Invoke(m_ResultValue.y);
		onValueChanged.Invoke(m_ResultValue);

		if (setCursor) {
			Vector3[] corners = new Vector3[4];
			rectTransform.GetWorldCorners(corners);
			float w = corners[2].x - corners[0].x;
			float h = corners[2].y - corners[0].y;
			float px = w * m_Padding;
			float py = h * m_Padding;
			Vector2 wp = new Vector2(
				Mathf.Lerp(corners[0].x + px, corners[2].x - px * 2, normalizedValue.x),
				Mathf.Lerp(corners[0].y + py, corners[2].y - py * 2, normalizedValue.y)
			);
			m_Knob.transform.position = new Vector3(wp.x,wp.y,m_Knob.transform.position.z);
			m_AxisX.transform.position = new Vector3(wp.x,m_AxisX.transform.position.y,m_AxisX.transform.position.z);
			m_AxisY.transform.position = new Vector3(m_AxisY.transform.position.x,wp.y,m_AxisY.transform.position.z);
		}
	}
}
}