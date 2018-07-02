/*
                                            _
   _ __ __ _ _   _ _ __ ___   __ _ _ __ ___| |__ (_)_ __   __ _ 
  | '__/ _` | | | | '_ ` _ \ /  ` | '__/ __| '_ \| | '_ \ /  ` |
  | | | (_| | |_| | | | | | |     | | | (__| | | | | | | |     |
  |_|  \__,_|\__, |_| |_| |_|\__,_|_|  \___|_| |_|_|_| |_|\__, |
             |___/                                        |___/ 
   _              _ _    _ _   
  | |_ ___   ___ | | | _(_) |_ 
  | __/   \ /   \| | |/ / | __|
  | ||     |     | |   <| | |_   for Unity
   \__\___/ \___/|_|_|\_\_|\__|
                              

  This shader was automatically generated from
  [[SOURCE_FILE]]
  
  for Raymarcher named '[[RAYMARCHER_NAME]]' in scene '[[SCENE_NAME]]'.

*/

Shader "Hidden/Fractal Template"
{

SubShader
{

Tags {
	"RenderType" = "Opaque"
	"Queue" = "Geometry-1"
	"DisableBatching" = "True"
	"IgnoreProjector" = "True"
}

Cull Off
ZWrite On

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc" // @noinlineinclude

#pragma multi_compile __ ENABLE_REFLECTIONS

uniform sampler2D _SkyGradient;
uniform float _SkyGradientNoise;
uniform float _Glow;
uniform float4 _GlowColor;
uniform int _Reflections;
uniform int _ReflectionSteps;
uniform float4 _ReflectionColor;

//[[GLOBALS]]

//[[LIGHTUNIFORMS]]

#include "../../../Assets/Shaders/Raymarching.cginc"
#include "../../../Assets/Shaders/CustomIncludes.cginc"

//[[UNIFORMS_AND_FUNCTIONS]]

float2 map(float3 p) {
	float2 result = float2(1.0, 0.0);
	//[[OBJECTS]]
	return result;
}

fixed4 sampleGradient(sampler2D tex, float value) {
	return tex2Dlod(tex, float4(value,0,0,0));
}

float3 getLights(in float3 color, in float3 pos, in float3 normal) {
	LightInput input;
	input.pos = pos;
	input.color = color;
	input.normal = normal;

	float3 lightValue = float3(0, 0, 0);
	//[[LIGHTS]]
	return lightValue;
}

float4 getColor(float3 p, float2 d, float3 rayDir, int numSteps, float3 bgColor)
{
	float t = d.x;

	// First, we sample the map() function around our hit point to find the normal.
	float3 n = calcNormal(p);

	// Then, we get the color of the world at that point, based on our material ids.
	float objectID = 0;
	float3 color = MaterialFunc(d.y, n, p, rayDir, objectID);
	float3 light = getLights(color, p, n);

	// The ambient color is applied.
	color *= _AmbientColor.xyz;

	// And lights are added.
	color += light;

	// Steps-based glow
	float s = numSteps / _Steps;
	s = clamp(s,0,1);
	color *= 1 + _Glow * s;
	color += _GlowColor * s;
	// color += _GlowColor * dot(1-s,2);

	// If enabled, darken with ambient occlusion.
	#if AO_ENABLED
	color *= ambientOcclusion(p, n);
	#endif

	// If fog is enabled, lerp towards the fog color based on the distance.
	#if FOG_ENABLED
	color = lerp(color, bgColor, 1.0-exp2(-FogDensity * t * t));
	#endif

	// If fading to the skybox is enabled, reduce the alpha value of the output pizel.
	#if FADE_TO_SKYBOX
	float alpha = lerp(1.0, 0, 1.0 - (_DrawDistance - t) / FadeToSkyboxDistance);
	#else
	float alpha = 1.0;
	#endif

	return fixed4(color, alpha);
}

fixed4 fractalRaymarch(float3 ro, float3 rd, float s, inout float3 raypos, out float objectID) {
	objectID = 0;
	bool found = false;

	float2 d;
	float t = 0; // current distance traveled along ray
	float3 p = float3(0, 0, 0);

	// Sample the sky gradient
	float3 gradientray = rd;
	if (_SkyGradientNoise != 0)
		gradientray += rand(rd.xy) * 0.03 * _SkyGradientNoise;
	// get the angle of the gradient to fake a dome
	// TODO: make this value spherical
	float g = gradientray.y;
	g = 0.5 + g * 0.5;
	fixed4 ret = sampleGradient(_SkyGradient, g);

	#if FADE_TO_SKYBOX
	const float skyboxAlpha = 0;
	#else
	const float skyboxAlpha = 1;
	#endif

	int numSteps;
	d = trace(ro, rd, raypos, numSteps, found);
	t = d.x;
	p = raypos;

	#if DEBUG_STEPS
	// float3 c = float3(1,0,0) * fmod(t, 0.1);
	float3 c = float3(1,0,0) * (1-(t / (float)numSteps));
	return fixed4(c, 1);
	#elif DEBUG_MATERIALS
	float3 c = float3(1,1,1) * (d.y / 20);
	return fixed4(c, 1);
	#endif


	[branch]
	if (found) 
	{
		#if ENABLE_REFLECTIONS
		ret = getColor(p, d, rd, numSteps, ret.rgb);
		float3 refp = p;
		float3 refraypos = raypos;
		float2 refd = d;
		for(int i = 0; i < _Reflections; ++i)
		{
			refraypos = calcNormal(refp);
			found = false;
			numSteps = 0;
			// TODO: pass _ReflectionStpes here
			refd = trace(refp, refraypos, refp, numSteps, found);
			if (!found)
				break;
			float4 refcol = getColor(refp, refd, rd, numSteps, ret.rgb);
			refcol = float4(refcol.rgb * _ReflectionColor.rgb, refcol.a);
			// refcol.rgb *=  _ReflectionColor.rgb;
			ret = lerp(ret, refcol, 0.5 * _ReflectionColor.a);
		}
		#else
		ret = getColor(p, d, rd, numSteps, ret.rgb);
		#endif
	}

	raypos = p;
	return ret;
}

#define raymarch fractalRaymarch

#include "../../../Assets/Shaders/RaymarchingEntryPoints.cginc"

ENDCG

}
}
}
