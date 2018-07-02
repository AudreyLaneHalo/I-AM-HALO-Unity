using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RaymarchingToolkit;

namespace RaymarchingToolkit.Examples {
public class CharacterEditor : MonoBehaviour {

	[Header("Cameras")]
	public float m_CameraMoveSpeed = 0.25f;
	public Camera m_Camera;
	public Camera m_CameraBody;
	public Camera m_CameraHead;
	public Camera m_CameraButt;

	[Header("References")]
	[SerializeField] CharacterReferences m_References;

	[SerializeField] Raymarcher m_RaymarcherWithBackdrop;	
	[SerializeField] Raymarcher m_RaymarcherWithoutBackdrop;	
	[SerializeField] Image m_ScreenshotFlash;	
	[SerializeField] Canvas m_Canvas;	

	[System.Serializable]
	public struct CharacterReferences {
		public Transform characterParent;
		public RaymarchObject head;
		public RaymarchObject headAccesory;
		public RaymarchObject body;
		public RaymarchObject butt;
		public RaymarchModifier bodyModifier;
		public RaymarchObject eye;
		public RaymarchObject eyeSocket;
		public RaymarchObject eyeSubtraction;
		public RaymarchObject[] arms;
		public RaymarchObject[] legs;
	}
	struct CameraTarget { 
		public Vector3 pos; public Quaternion rot; public float fov;
		public static CameraTarget From(Camera cam){ return new CameraTarget{pos=cam.transform.position, rot=cam.transform.rotation, fov=cam.fieldOfView}; }}
	
	int m_HeadIndex;
	CameraTarget m_DesiredTarget;
	CameraTarget m_TargetBody;
	CameraTarget m_TargetHead;
	CameraTarget m_TargetButt;
	Coroutine cameraTurn;

	const string COLOR_PROPERTY = "color";

	void Start () {
		// Camera
		m_CameraBody.gameObject.SetActive(false);
		m_DesiredTarget = CameraTarget.From(m_CameraBody);
		m_Camera.transform.position = m_DesiredTarget.pos;
		m_Camera.transform.rotation = m_DesiredTarget.rot;
		m_TargetBody = CameraTarget.From(m_CameraBody);
		m_TargetHead = CameraTarget.From(m_CameraHead);
		m_TargetButt = CameraTarget.From(m_CameraButt);

		// Reset all controls
		foreach(var group in Resources.FindObjectsOfTypeAll<ToggleGroup>()) {
			var toggles = group.GetComponentsInChildren<Toggle>(true);
			Toggle first = null;
			if (toggles.Length > 0)
				first = toggles.Single(tg => tg.group == group && tg.isOn);
			group.SetAllTogglesOff();
			if (first) {
				first.isOn = false;
				first.isOn = true;
				first.onValueChanged.Invoke(true);
			}
		}
		foreach(var slider in Resources.FindObjectsOfTypeAll<Slider>()) {
			slider.onValueChanged.Invoke(slider.value);
		}
		foreach(var xy in Resources.FindObjectsOfTypeAll<XYControl>()) {
			xy.SetValue(Vector2.one * .5f);
		}
	}
	
	void Update () {
		m_Camera.transform.position = Vector3.Lerp(m_Camera.transform.position, m_DesiredTarget.pos, Time.deltaTime * m_CameraMoveSpeed);
		m_Camera.transform.rotation = Quaternion.Lerp(m_Camera.transform.rotation, m_DesiredTarget.rot, Time.deltaTime * m_CameraMoveSpeed);
		m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_DesiredTarget.fov, Time.deltaTime * m_CameraMoveSpeed);

		m_DesiredTarget.fov = Mathf.Clamp(m_DesiredTarget.fov - Input.mouseScrollDelta.y * Time.deltaTime * 5, 3.5f, 45f);
	}

	void TrySetColor(RaymarchObject obj, string propName, Color color) {
		var prop = obj.GetMaterialInput(propName);
		if (prop != null)
			prop.color = color;
	}

	IEnumerator ScreenshotSequence() {
		m_ScreenshotFlash.enabled = false;

		m_Canvas.enabled = false;
		var prevRT = m_Camera.targetTexture;
		var tex = new Texture2D(Screen.height, Screen.height, TextureFormat.RGB24, false);
		yield return new WaitForEndOfFrame();
		tex.ReadPixels(new Rect(0,0,Screen.height,Screen.height), 0, 0);
		tex.Apply();

		// split the process up--ReadPixels() and the GetPixels() call inside of the encoder are both pretty heavy
		yield return 0;
	
		byte[] bytes = tex.EncodeToPNG();
		
		string filename = string.Format("Snap_{0}.png", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
		string fullPath = string.Format("{0}/{1}", Application.dataPath, filename);
		try {
			System.IO.File.WriteAllBytes(fullPath , bytes);
		} catch (System.Exception e) {
			Debug.LogError("Error saving screenshot " + e.ToString());
		}

		m_Canvas.enabled = true;

		var c = Color.white;
		m_ScreenshotFlash.color = c;
		m_ScreenshotFlash.enabled = true;
		float t = 1f;
		while(t > 0) {
			t -= Time.deltaTime;
			c.a = t;
			m_ScreenshotFlash.color = c;
			yield return 0;
		}
		m_ScreenshotFlash.enabled = false;
	}

	#region Callbacks

	public void SetEyeSize(float value) {
		m_References.eye.GetObjectInput("radius").floatValue = value;
		m_References.eyeSocket.GetObjectInput("radius").floatValue = value + 0.01f;
		m_References.eyeSubtraction.GetObjectInput("radius").floatValue = (value + 0.02f) * 1.12f;
		UpdateEyePositions();
	}
	public void SetEyeHeight(float value) {
		var p = m_References.eye.transform.localPosition;
		p.x = value;
		m_References.eye.transform.localPosition = p;
		UpdateEyePositions();
	}
	public void SetEyeSeparation(float value) {
		var p = m_References.eye.transform.localPosition;
		p.z = value;
		m_References.eye.transform.localPosition = p;
		UpdateEyePositions();
	}
	public void SetEyeRotation(float value) {
		var r = m_References.eyeSubtraction.transform.localEulerAngles;
		r.y = value;
		m_References.eyeSubtraction.transform.localEulerAngles = r;
	}
	void UpdateEyePositions() {
		float radius = m_References.eye.GetObjectInput("radius").floatValue;
		var p = m_References.eye.transform.localPosition;
		m_References.eyeSocket.transform.localPosition = p;
		m_References.eyeSubtraction.transform.localPosition = p + Vector3.up * radius;
	}
	public void SetAccesoryPosition(Vector2 pos) {
		var p = m_References.headAccesory.transform.localPosition;
		p.x = pos.y;
		p.z = pos.x;
		m_References.headAccesory.transform.localPosition = p;
	}
	public void SetBodyShape(Vector2 value) {
		m_References.body.GetObjectInput("z").floatValue = value.x;
		m_References.bodyModifier.GetInput("intensity").floatValue = value.y;
	}
	public void SetButtSeparation(Vector2 pos) {
		var p = m_References.butt.transform.localPosition;
		p.x = pos.y;
		p.z = pos.x;
		m_References.butt.transform.localPosition = p;
	}
	public void SetButtDistance(float value) {
		var p = m_References.butt.transform.localPosition;
		p.y = value;
		m_References.butt.transform.localPosition = p;
	}
	public void SetButtSize(float value) {
		m_References.butt.GetObjectInput("radius").floatValue = value;
	}

	public void SetHeadColor(Color color) {
		TrySetColor(m_References.head, COLOR_PROPERTY, color);
		TrySetColor(m_References.headAccesory, COLOR_PROPERTY, color);
		TrySetColor(m_References.eyeSocket, COLOR_PROPERTY, color);
	}
	public void SetArmsColor(Color color) {
		foreach(var obj in m_References.arms)
			TrySetColor(obj, COLOR_PROPERTY, color);
	}
	public void SetLegsColor(Color color) {
		foreach(var obj in m_References.legs)
			TrySetColor(obj, COLOR_PROPERTY, color);
	}
	public void SetBodyColor(Color color) {
		TrySetColor(m_References.body, COLOR_PROPERTY, color);
		TrySetColor(m_References.butt, COLOR_PROPERTY, color);
	}

	public void LookAtBody() { m_DesiredTarget = m_TargetBody; TurnCameraTo(-10);}
	public void LookAtHead() { m_DesiredTarget = m_TargetHead; TurnCameraTo(0);}
	public void LookAtButt() { m_DesiredTarget = m_TargetButt; TurnCameraTo(143); }
	void TurnCameraTo(float angle = 0) {
		if (cameraTurn != null)
			StopCoroutine(cameraTurn);
		cameraTurn = StartCoroutine(CameraTurnSequence(angle));
	}
	IEnumerator CameraTurnSequence(float y = 0) {
		float t = 1f;
		var rot = m_References.characterParent.localEulerAngles;
		float r = rot.y;
		while(t > 0) {
			t -= Time.deltaTime * 1.5f;
			rot.y = Mathf.LerpAngle(r, y, Mathf.SmoothStep(0,1,1 - (t / 1f)));
			m_References.characterParent.localEulerAngles = rot;
			yield return 0;
		}
	}

	public void PlayAnimation(AnimationClip clip) {
		var anim = m_References.characterParent.GetComponent<Animation>();
		anim.clip = clip;
		anim.Play(PlayMode.StopAll);
	}

	public void ToggleBackdrop() {
		m_RaymarcherWithBackdrop.gameObject.SetActive(!m_RaymarcherWithBackdrop.gameObject.activeSelf);
		m_RaymarcherWithoutBackdrop.gameObject.SetActive(!m_RaymarcherWithoutBackdrop.gameObject.activeSelf);
	}

	public void OnDragCharacter(PointerEventData data) {
		var delta = m_Camera.ScreenToViewportPoint(data.delta) * 10000;
		m_References.characterParent.Rotate(0,-delta.x * Time.deltaTime, 0, Space.Self); // TODO: make resolution independent
	}

	public void TakeScreenshot() {
		StartCoroutine(ScreenshotSequence());
	}
	#endregion

}
}