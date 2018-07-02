using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
[RequireComponent(typeof(Raymarcher)), ExecuteInEditMode()]
public class FractalTemplateProperties : MonoBehaviour {

	public float m_Glow = 0;
	public Color m_ColorizedGlow = Color.black;

	public GradientTexture m_SkyGradient = new GradientTexture(new GradientColorKey[]{
		new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(Color.black, 1),
	});
	public float m_SkyGradientNoise = 0.06f;

	[StayPositive] public int m_Reflections = 0;
	[StayPositive] public int m_ReflectionSteps = 100;
	[StayPositive] public Color m_ReflectionColor = Color.white;

	Raymarcher m_Raymarcher;

	const string REFLECTIONS_KEYWORD = "ENABLE_REFLECTIONS";

	void OnEnable () {
		m_Raymarcher = GetComponent<Raymarcher>();	
	}
	
	void Update () {
		var material = m_Raymarcher.GetRaymarchMaterial();
		
		bool useReflections = m_Reflections > 0;
		bool kw = material.IsKeywordEnabled(REFLECTIONS_KEYWORD);
		if (kw && !useReflections)
			material.DisableKeyword(REFLECTIONS_KEYWORD);
		else if (!kw && useReflections)
			material.EnableKeyword(REFLECTIONS_KEYWORD);
		material.SetInt("_Reflections", m_Reflections);
		material.SetInt("_ReflectionSteps", m_ReflectionSteps);
		material.SetColor("_ReflectionColor", m_ReflectionColor);

		material.SetFloat("_Glow", m_Glow);
		material.SetColor("_GlowColor", m_ColorizedGlow);

		material.SetTexture("_SkyGradient", m_SkyGradient.GetTexture());
		material.SetFloat("_SkyGradientNoise", m_SkyGradientNoise);
		
	}
}
}