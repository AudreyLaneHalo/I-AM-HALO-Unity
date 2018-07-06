using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrimitiveGenerator{
	[AddComponentMenu("Primitive Generators/Triangle Soup")]
	public class TriangleSoupGenerator : PrimitiveGeneratorBase {
		[Header("Scaling")]
		public float width = 1;
		public float height = 1;
		public float depth = 1;
		public float triangleSize = 0.05f;
		[Header("Resolution")]
		public int numTriangles = 1000;
		[Header("Texture Mapping")]
		public float uvWidth = 1;
		public float uvHeight = 1;

		private const string _name = "GeneratedTriangleSoup";

		public override void CreateData()
		{
			base.CreateData();
			int index = 0;
			float s = Mathf.PI * 2 / 3;
			for (int i = 0; i < numTriangles; i++) {
				Vector3 center = new Vector3( 
					Random.Range(-width / 2, width / 2),
					Random.Range(-height / 2, height / 2),
					Random.Range(-depth / 2, depth / 2)
				);
				
				Vector3 v0 = new Vector3(Mathf.Sin(s)*triangleSize, Mathf.Cos(s) * triangleSize, 0);
				Vector3 v1 = new Vector3(Mathf.Sin(s*2) * triangleSize, Mathf.Cos(s*2) * triangleSize, 0);
				Vector3 v2 = new Vector3(Mathf.Sin(s*3) * triangleSize, Mathf.Cos(s*3) * triangleSize, 0);

				Vector2 uv0 = new Vector2(v0.x, v0.y).normalized;
				Vector2 uv1 = new Vector2(v1.x, v1.y).normalized;
				Vector2 uv2 = new Vector2(v2.x, v2.y).normalized;

				Quaternion r = Random.rotation;
				v0 = r * v0;
				v1 = r * v1;
				v2 = r * v2;
				Vector3 n = Vector3.Cross( v0, v1 ).normalized;

				v0 += center;
				v1 += center;
				v2 += center;

				meshData.vertex.Add(v0); meshData.normal.Add(n); meshData.index.Add(index); index++; meshData.uv0.Add( uv0 );
				meshData.vertex.Add(v1); meshData.normal.Add(n); meshData.index.Add(index); index++; meshData.uv0.Add( uv1 );
				meshData.vertex.Add(v2); meshData.normal.Add(n); meshData.index.Add(index); index++; meshData.uv0.Add( uv2 );
			}
		}

		public override bool ValidateParameters ()
		{
			return (numTriangles > 0);
		}
	}
}