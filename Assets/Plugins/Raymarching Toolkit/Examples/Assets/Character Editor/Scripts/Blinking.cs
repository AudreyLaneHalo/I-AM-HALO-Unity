using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaymarchingToolkit;

namespace RaymarchingToolkit.Examples
{
	
public class Blinking : MonoBehaviour
{
	public RaymarchObject[] m_EyeSockets;
	public AnimationCurve m_BlinkCurve = AnimationCurve.Linear(0,0,1,1);

	const float BLINK_DURATION = 2f;
	const float BLINK_DELAY = 3;
	
	void Update ()
	{
		var delay = Mathf.Abs(Mathf.Sin(Mathf.Sin(Time.time) + Mathf.Sin(Time.time * 0.4f))) * BLINK_DELAY;
		foreach (var obj in m_EyeSockets) {
			var s = obj.transform.localScale;
			var t = Time.time + System.Array.IndexOf(m_EyeSockets, obj) * 0.1f;
			s.x = m_BlinkCurve.Evaluate(((t % BLINK_DURATION) / BLINK_DURATION) * delay);
			obj.transform.localScale = s;
		}
	}
}
	
}