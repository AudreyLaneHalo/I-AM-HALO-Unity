fixed4 defaultRaymarch(float3 ro, float3 rd, float s, inout float3 raypos, out float objectID)
{
	bool found = false;
	objectID = 0.0;

	float2 d;
	float t = 0; // current distance traveled along ray
	float3 p = float3(0, 0, 0);

#if FADE_TO_SKYBOX
	const float skyboxAlpha = 0;
#else
	const float skyboxAlpha = 1;
#endif

#if FOG_ENABLED
	fixed4 ret = fixed4(FogColor, skyboxAlpha);
#else
	fixed4 ret = fixed4(0,0,0,0);
#endif

	int numSteps;
	d = trace(ro, rd, raypos, numSteps, found);
	t = d.x;
	p = raypos;

#if DEBUG_STEPS
	float3 c = float3(1,0,0) * (1-(t / (float)numSteps));
	return fixed4(c, 1);
#elif DEBUG_MATERIALS
	float3 c = float3(1,1,1) * (d.y / 20);
	return fixed4(c, 1);
#endif

	[branch]
	if (found)
	{
		// First, we sample the map() function around our hit point to find the normal.
		float3 n = calcNormal(p);

		// Then, we get the color of the world at that point, based on our material ids.
		float3 color = MaterialFunc(d.y, n, p, rd, objectID);
		float3 light = getLights(color, p, n);

		// The ambient color is applied.
		color *= _AmbientColor.xyz;

		// And lights are added.
		color += light;

		// If enabled, darken with ambient occlusion.
		#if AO_ENABLED
		color *= ambientOcclusion(p, n);
		#endif

		// If fog is enabled, lerp towards the fog color based on the distance.
		#if FOG_ENABLED
		color = lerp(color, FogColor, 1.0-exp2(-FogDensity * t * t));
		#endif

		// If fading to the skybox is enabled, reduce the alpha value of the output pizel.
		#if FADE_TO_SKYBOX
		float alpha = lerp(1.0, 0, 1.0 - (_DrawDistance - t) / FadeToSkyboxDistance);
		#else
		float alpha = 1.0;
		#endif

		ret = fixed4(color, alpha);
	}

	raypos = p;
	return ret;
}


FragOut frag (v2f i)
{
    UNITY_SETUP_INSTANCE_ID(i);

    #if RENDER_OBJECT
    float3 rayDir = GetCameraDirection(i.screenPos);
    #else
    float3 rayDir = normalize(i.interpolatedRay);
    #endif

  	float3 rayOrigin = _WorldSpaceCameraPos + _ProjectionParams.y * rayDir;
    
	float3 raypos = float3(0, 0, 0);
	float objectID = 0.0;
	
	FragOut o;
	o.col = raymarch(rayOrigin, rayDir, _DrawDistance, raypos, objectID);
	o.col1 = float4(objectID, 0, 0, 1);
  #if !SKIP_DEPTH_WRITE
	o.depth = compute_depth(mul(UNITY_MATRIX_VP, float4(raypos, 1.0)));
	
	#if !FOG_ENABLED
	clip(o.col.a < 0.001 ? -1.0 : 1.0);
	#endif
  #endif
	
	return o;
}
