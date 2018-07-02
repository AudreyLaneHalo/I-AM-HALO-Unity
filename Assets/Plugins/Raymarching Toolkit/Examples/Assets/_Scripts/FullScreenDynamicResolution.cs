using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace RaymarchingToolkit.Examples
{
[RequireComponent(typeof(Canvas)), RequireComponent(typeof(Camera)), RequireComponent(typeof(RawImage))]
[ExecuteInEditMode()]
public class FullScreenDynamicResolution : MonoBehaviour
{
	public Camera targetCamera;
	[Tooltip("Width of the resulting texture. It can also be a number between 0 and 1, as a factor of the current resolution (eg. 0.5 = 50% resolution)")]
	public float size = 0.5f;
	public FilterMode filterMode = FilterMode.Point;

	Camera cam;
	RawImage rawImage;
	RenderTexture renderTexture;
	RenderTexture originalRenderTexture;
	float _lastSize = -1f;

	Camera Target { get { return targetCamera ? targetCamera : Camera.main; }}
	int Width { get { return Mathf.RoundToInt(size > 1 ? size : (cam.pixelWidth * size)); } }
	int Height { get { return Mathf.RoundToInt(size > 1 ? (size / Target.aspect) : (cam.pixelHeight * size)); } }

	bool isVR = false;

	void OnEnable()
	{
		if (UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRDevice.isPresent)
		{
			isVR = true;
			return;
		}

		GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		rawImage = GetComponent<RawImage>();
		cam = GetComponent<Camera>();
		cam.clearFlags = CameraClearFlags.SolidColor;
		cam.cullingMask = 0;
		cam.depth = 99;
		cam.backgroundColor = Color.black;

		Update();
	}

	void OnDisable()
	{
		if (!isVR)
			RemoveRenderTexture();
	}

	void AddRenderTexture()
	{
		cam.enabled = true;
		renderTexture = new RenderTexture(
			Mathf.Max(1, Width), Mathf.Max(1, Height), 24,
			RenderTextureFormat.DefaultHDR,
			RenderTextureReadWrite.Default);
		renderTexture.useMipMap = false;
		renderTexture.name = "Dynamic Resolution RT";
		renderTexture.filterMode = filterMode;
		renderTexture.Create();

		rawImage.texture = renderTexture;
		rawImage.enabled = true;

		if (Target)
		{
			originalRenderTexture = Target.targetTexture;
			Target.targetTexture = renderTexture;
		}
	}

	void Update()
	{
		if (!Target || isVR)
			return;

		rawImage.rectTransform.sizeDelta = new Vector2(
			cam.pixelWidth, cam.pixelHeight);

		if (Mathf.Approximately(_lastSize, size))
			return;
		
		RemoveRenderTexture();
		AddRenderTexture();
		_lastSize = size;
	}

	void RemoveRenderTexture()
	{
		cam.enabled = false;
		rawImage.enabled = false;

		if (Target)
			Target.targetTexture = originalRenderTexture;

		if (renderTexture)
		{
			renderTexture.Release();
			renderTexture = null;
		}
	}
}
}