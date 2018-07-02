using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RaymarchingToolkit.Examples {
[RequireComponent(typeof(Button))]
public class DragHandler : MonoBehaviour, IDragHandler {

	[System.Serializable]
	public class DragEvent : UnityEvent<PointerEventData> { }
	public DragEvent m_OnDrag;
	
	public void OnDrag(PointerEventData data) {
		m_OnDrag.Invoke(data);
	}
}
}