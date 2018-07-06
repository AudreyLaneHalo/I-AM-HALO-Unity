using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrimitiveGenerator{
	[AddComponentMenu("Primitive Generators/Sphere")]
	public class SphereGenerator : PrimitiveGeneratorBase {

		public float radius = 0.5f;
		[Header("Resolution")]
		public int latitudeSegments = 8;
		public int longitudeSegments = 8;
		[Header("Texture Mapping")]
		public float UVscaleX = 1;
		public float UVscaleY = 1;

		private const string _name = "GeneratedSphere";

		public override void CreateData ()
		{
			base.CreateData();
			int i = 0;
			for (int m = 0; m < latitudeSegments; m++) {
				for (int n = 0; n < longitudeSegments; n++) {
					
					float u0 = ((float)m / latitudeSegments) * UVscaleY;
					float u1 = ((float)(m+1) / latitudeSegments) *UVscaleY;
					float v0 = ((float)n / longitudeSegments) * UVscaleX;
					float v1 = ((float)(n+1) / longitudeSegments) * UVscaleX;

					float x00 = Mathf.Sin(Mathf.PI * m / latitudeSegments) * Mathf.Cos(2 * Mathf.PI * n / longitudeSegments);
					float y00 = Mathf.Sin(Mathf.PI * m / latitudeSegments) * Mathf.Sin(2 * Mathf.PI * n / longitudeSegments);
					float z00 = Mathf.Cos(Mathf.PI * m / latitudeSegments);

					float x10 = Mathf.Sin(Mathf.PI * (m + 1) / latitudeSegments) * Mathf.Cos(2 * Mathf.PI * n / longitudeSegments);
					float y10 = Mathf.Sin(Mathf.PI * (m + 1) / latitudeSegments) * Mathf.Sin(2 * Mathf.PI * n / longitudeSegments);
					float z10 = Mathf.Cos(Mathf.PI * (m + 1) / latitudeSegments);

					float x01 = Mathf.Sin(Mathf.PI * m / latitudeSegments) * Mathf.Cos(2 * Mathf.PI * (n+1) / longitudeSegments);
					float y01 = Mathf.Sin(Mathf.PI * m / latitudeSegments) * Mathf.Sin(2 * Mathf.PI * (n+1) / longitudeSegments);
					float z01 = Mathf.Cos(Mathf.PI * m / latitudeSegments);

					float x11 = Mathf.Sin(Mathf.PI * (m + 1) / latitudeSegments) * Mathf.Cos(2 * Mathf.PI * (n+1) / longitudeSegments);
					float y11 = Mathf.Sin(Mathf.PI * (m + 1) / latitudeSegments) * Mathf.Sin(2 * Mathf.PI * (n+1) / longitudeSegments);
					float z11 = Mathf.Cos(Mathf.PI * (m + 1) / latitudeSegments);

					x00 *= radius; x10 *= radius; x01 *= radius; x11 *= radius;
					y00 *= radius; y10 *= radius; y01 *= radius; y11 *= radius;
					z00 *= radius; z10 *= radius; z01 *= radius; z11 *= radius;

					meshData.vertex.Add(new Vector3(x10, z10, y10)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u1, v0)); 
					meshData.vertex.Add(new Vector3(x00, z00, y00)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u0, v0)); 
					meshData.vertex.Add(new Vector3(x11, z11, y11)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u1, v1)); 

					meshData.vertex.Add(new Vector3(x00, z00, y00)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u0, v0));
					meshData.vertex.Add(new Vector3(x01, z01, y01)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u0, v1));
					meshData.vertex.Add(new Vector3(x11, z11, y11)); meshData.index.Add(i); i++; meshData.uv0.Add(new Vector2(u1, v1));

					if (SmoothShading) {
						meshData.normal.Add(new Vector3(x10, z10, y10).normalized);
						meshData.normal.Add(new Vector3(x00, z00, y00).normalized);
						meshData.normal.Add(new Vector3(x11, z11, y11).normalized);

						meshData.normal.Add(new Vector3(x00, z00, y00).normalized);
						meshData.normal.Add(new Vector3(x01, z01, y01).normalized);
						meshData.normal.Add(new Vector3(x11, z11, y11).normalized);
					} else { 
						Vector3 faceNormal = (new Vector3(x10, z10, y10) + new Vector3(x00, z00, y00) + new Vector3(x11, z11, y11)).normalized;
						meshData.normal.Add(faceNormal);
						meshData.normal.Add(faceNormal);
						meshData.normal.Add(faceNormal);
						meshData.normal.Add(faceNormal);
						meshData.normal.Add(faceNormal);
						meshData.normal.Add(faceNormal);
					}
				}
			}
		}
		public override bool ValidateParameters ()
		{
			return base.ValidateParameters ();
		}
	}
}