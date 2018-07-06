using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrimitiveGenerator{
	[AddComponentMenu("Primitive Generators/Icosahedron")]
	public class IcosahedronGenerator : PrimitiveGeneratorBase {

		public float radius = 0.5f;
		[Header("Resolution (max 5)")]
		public int subdivisions = 1;
		[Header("Texture Mapping")]
		public float UVScaleX = 1;
		public float UVScaleY = 1;

		private const string _name = "GeneratedSphere";

		public override void CreateData ()
		{
			base.CreateData ();

			float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

			Vector3[] v = new Vector3[12]{
				new Vector3(-1f, t, 0).normalized * radius,
				new Vector3(1f, t, 0).normalized * radius,
				new Vector3(-1f, -t, 0).normalized * radius,
				new Vector3(1f, -t, 0).normalized * radius,

				new Vector3(0, -1f, t).normalized * radius,
				new Vector3(0, 1f, t).normalized * radius,
				new Vector3(0, -1f, -t).normalized * radius,
				new Vector3(0, 1f, -t).normalized * radius,

				new Vector3( t, 0, -1f).normalized * radius,
				new Vector3( t, 0, 1f).normalized * radius,
				new Vector3(-t, 0, -1f).normalized * radius,
				new Vector3(-t, 0, 1f).normalized * radius
			};

			meshData.vertex.Add(v[0]); meshData.vertex.Add(v[11]); meshData.vertex.Add(v[5]);
			meshData.vertex.Add(v[0]); meshData.vertex.Add(v[5]); meshData.vertex.Add(v[1]);
			meshData.vertex.Add(v[0]); meshData.vertex.Add(v[1]); meshData.vertex.Add(v[7]);
			meshData.vertex.Add(v[0]); meshData.vertex.Add(v[7]); meshData.vertex.Add(v[10]);
			meshData.vertex.Add(v[0]); meshData.vertex.Add(v[10]); meshData.vertex.Add(v[11]);

			meshData.vertex.Add(v[1]); meshData.vertex.Add(v[5]); meshData.vertex.Add(v[9]);
			meshData.vertex.Add(v[5]); meshData.vertex.Add(v[11]); meshData.vertex.Add(v[4]);
			meshData.vertex.Add(v[11]); meshData.vertex.Add(v[10]); meshData.vertex.Add(v[2]);
			meshData.vertex.Add(v[10]); meshData.vertex.Add(v[7]); meshData.vertex.Add(v[6]);
			meshData.vertex.Add(v[7]); meshData.vertex.Add(v[1]); meshData.vertex.Add(v[8]);

			meshData.vertex.Add(v[3]); meshData.vertex.Add(v[9]); meshData.vertex.Add(v[4]);
			meshData.vertex.Add(v[3]); meshData.vertex.Add(v[4]); meshData.vertex.Add(v[2]);
			meshData.vertex.Add(v[3]); meshData.vertex.Add(v[2]); meshData.vertex.Add(v[6]);
			meshData.vertex.Add(v[3]); meshData.vertex.Add(v[6]); meshData.vertex.Add(v[8]);
			meshData.vertex.Add(v[3]); meshData.vertex.Add(v[8]); meshData.vertex.Add(v[9]);

			meshData.vertex.Add(v[4]); meshData.vertex.Add(v[9]); meshData.vertex.Add(v[5]);
			meshData.vertex.Add(v[2]); meshData.vertex.Add(v[4]); meshData.vertex.Add(v[11]);
			meshData.vertex.Add(v[6]); meshData.vertex.Add(v[2]); meshData.vertex.Add(v[10]);
			meshData.vertex.Add(v[8]); meshData.vertex.Add(v[6]); meshData.vertex.Add(v[7]);
			meshData.vertex.Add(v[9]); meshData.vertex.Add(v[8]); meshData.vertex.Add(v[1]);

			if (subdivisions > 0) {
				for (int i = 0; i < subdivisions; i++) {
					int count = meshData.vertex.Count;
					List<Vector3> newVerts = new List<Vector3>();
					for (int j = 0; j < count; j += 3) {
						// triangle points
						Vector3 va = meshData.vertex[j];
						Vector3 vb = meshData.vertex[j+1];
						Vector3 vc = meshData.vertex[j+2];

						// create middle points
						Vector3 vab = Vector3.Lerp(va, vb, 0.5f).normalized * radius;
						Vector3 vbc = Vector3.Lerp(vb, vc, 0.5f).normalized * radius;
						Vector3 vca = Vector3.Lerp(vc, va, 0.5f).normalized * radius;

						newVerts.Add(va); newVerts.Add(vab); newVerts.Add(vca);
						newVerts.Add(vab); newVerts.Add(vb); newVerts.Add(vbc);
						newVerts.Add(vca); newVerts.Add(vbc); newVerts.Add(vc);
						newVerts.Add(vab); newVerts.Add(vbc); newVerts.Add(vca);
					}
					meshData.vertex = newVerts;
				}
			}
			
			for (int k = 0; k < meshData.vertex.Count; k += 3) { 
				meshData.index.Add(k); meshData.index.Add(k+1); meshData.index.Add(k+2);
				meshData.uv0.Add(new Vector2(0,0)); meshData.uv0.Add(new Vector2(0, 0)); meshData.uv0.Add(new Vector2(0, 0));
				if (SmoothShading) {
					meshData.normal.Add(meshData.vertex[k].normalized);
					meshData.normal.Add(meshData.vertex[k+1].normalized);
					meshData.normal.Add(meshData.vertex[k+2].normalized);
				} else{
					Vector3 faceNormal = (meshData.vertex[k + 1] + meshData.vertex[k] + meshData.vertex[k + 2]).normalized;
					meshData.normal.Add(faceNormal);
					meshData.normal.Add(faceNormal);
					meshData.normal.Add(faceNormal);
				}
			}
		}
		public override bool ValidateParameters ()
		{
			if (subdivisions > 5) { subdivisions = 5; }
			if (subdivisions < 0) { subdivisions = 0; }
			return base.ValidateParameters ();
		}
	}
}