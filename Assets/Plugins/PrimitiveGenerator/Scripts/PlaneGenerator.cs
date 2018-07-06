using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrimitiveGenerator{
	[AddComponentMenu("Primitive Generators/Plane")]
	public class PlaneGenerator : PrimitiveGeneratorBase {
		[Header("Scaling")]
		public float width = 1;
		public float height = 1;
		[Header("Resolution")]
		public int widthSegments = 1;
		public int heightSegments = 1;
		[Header("Texture Mapping")]
		public float uvWidth = 1;
		public float uvHeight = 1;

		private const string _name = "GeneratedPlane";

		public override void CreateData()
		{
			base.CreateData();
			float stepX = width / widthSegments;
			float stepY = height / heightSegments;
			float x0 = 0;
			float y0 = 0;
			float x1 = 0;
			float y1 = 0;
			float u0 = 0;
			float u1 = 0;
			float v0 = 0;
			float v1 = 0;
			int i = 0;
			for (int x = 0; x < widthSegments; x++) {
				for (int y = 0; y < heightSegments; y++) {
					x0 = x*stepX - width/2; x1 = x0 + stepX;
					y0 = y*stepY - height/2; y1 = y0 + stepY;
					u0 = x0 / width+0.5f;	u1 = x1 / width+0.5f;
					v0 = y0 / height+0.5f;	v1 = y1 / height+0.5f;
					u0 *= uvWidth; u1 *= uvWidth;
					v0 *= uvHeight; v1 *= uvHeight;
					meshData.vertex.Add ( new Vector3( x1, 0, y0 ) ); meshData.uv0.Add (new Vector2( u1, v0 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;
					meshData.vertex.Add ( new Vector3( x0, 0, y0 ) ); meshData.uv0.Add (new Vector2( u0, v0 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;
					meshData.vertex.Add ( new Vector3( x1, 0, y1 ) ); meshData.uv0.Add (new Vector2( u1, v1 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;

					meshData.vertex.Add ( new Vector3( x0, 0, y0 ) ); meshData.uv0.Add (new Vector2( u0, v0 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;
					meshData.vertex.Add ( new Vector3( x0, 0, y1 ) ); meshData.uv0.Add (new Vector2( u0, v1 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;
					meshData.vertex.Add ( new Vector3( x1, 0, y1 ) ); meshData.uv0.Add (new Vector2( u1, v1 )); meshData.normal.Add(new Vector3(0, 1, 0)); meshData.index.Add ( i ); i++;
				}
			}
		}

		public override bool ValidateParameters ()
		{
			return (width > 0 && height > 0 && widthSegments > 0 && heightSegments > 0);
		}
	}
}