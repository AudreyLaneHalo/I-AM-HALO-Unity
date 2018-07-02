Shader "Hidden/Raymarcher Blit"
{
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Geometry-1" }
		Cull Off
		ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile __ CONVERT_TO_GAMMA
			#pragma multi_compile __ HIGHLIGHT_OBJECTS

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			UNITY_DECLARE_SCREENSPACE_TEXTURE(_RaymarchColor);
			UNITY_DECLARE_DEPTH_TEXTURE(_RaymarchDepth);
			
			#if HIGHLIGHT_OBJECTS
			UNITY_DECLARE_SCREENSPACE_TEXTURE(ObjectIDTexture);
			float objectToHighlight;
			fixed4 objectHighlightColor;
			#endif

			float2 TransformTriangleVertexToUV(float2 vertex)
			{
				float2 uv = (vertex + 1.0) * 0.5;
				return uv;
			}

			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = float4(v.vertex.xy, 0.0, 1.0);
				o.uv = TransformTriangleVertexToUV(v.vertex.xy);
				o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
				return o;
			}

			struct FragOut
			{
				fixed4 color : SV_Target;
				float depth : SV_Depth;
			};
			
			float4 LinearToSrgb(float4 color) {
              // Approximation http://chilliant.blogspot.com/2012/08/srgb-approximations-for-hlsl.html
              float3 linearColor = color.rgb;
              float3 S1 = sqrt(linearColor);
              float3 S2 = sqrt(S1);
              float3 S3 = sqrt(S2);
              color.rgb = 0.662002687 * S1 + 0.684122060 * S2 - 0.323583601 * S3 - 0.0225411470 * linearColor;
              return color;
            }

			FragOut frag(v2f i)
			{
				UNITY_SETUP_INSTANCE_ID(i);

				FragOut result;
				float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);
				fixed4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_RaymarchColor, uv);
				
				#if CONVERT_TO_GAMMA
				col = LinearToSrgb(col);
				#endif
				
				result.color = col;
				
				#if HIGHLIGHT_OBJECTS
				#define HIGHLIGHT_EPSILON 0.001
				float objectID = UNITY_SAMPLE_SCREENSPACE_TEXTURE(ObjectIDTexture, uv).r;
				if (abs(objectID - objectToHighlight) < HIGHLIGHT_EPSILON)
				{
				    result.color = lerp(result.color, fixed4(objectHighlightColor.rgb, 1.0), objectHighlightColor.a);
				}
				#endif
				
                result.depth = SAMPLE_DEPTH_TEXTURE(_RaymarchDepth, uv);
				return result;
			}
			ENDCG
		}
	}
}
