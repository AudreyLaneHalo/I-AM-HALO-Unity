using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrimitiveGenerator{
	[AddComponentMenu("Primitive Generators/Box")]
	public class BoxGenerator : PrimitiveGeneratorBase {
		[Header("Scaling")]
		public float width = 1;
		public float height = 1;
		public float depth = 1;
		[Header("Resolution")]
		public int widthSegments = 1;
		public int heightSegments = 1;
		public int depthSegments = 1;
		[Header("Texture Mapping")]
		public float uvWidth = 1;
		public float uvHeight = 1;
		public float uvDepth = 1;

		private const string _name = "GeneratedBox";

		MeshData Plane( float width, float height, int segmentsX, int segmentsY, float uvSizeX, float uvSizeY, float offset ){
			MeshData plane = new MeshData ();
			float stepX = width / segmentsX;
			float stepY = height / segmentsY;
			float x0 = 0;
			float y0 = 0;
			float x1 = 0;
			float y1 = 0;
			float u0 = 0;
			float u1 = 0;
			float v0 = 0;
			float v1 = 0;
			Vector3 n00;
			Vector3 n10;
			Vector3 n01;
			Vector3 n11;
			for (int x = 0; x < segmentsX; x++){
				for(int y = 0; y < segmentsY; y++){

					/*
					 * Unity uses clockwise winding for triangles

					   x1 y1                 x0 y1
					        +---------------+
					        | \             |        Y increase
							|   \           |              / \
							|     \         |               |
							|       \       |               |
							|         \     |               |
							|           \   |               |
					        |             \ |               |
					        +---------------+               |
						x1 y0                x0 y0          |
						                                    |
						X increase  /_______________________+
									\

					 */

					x0 = x * stepX - width / 2; x1 = x0 + stepX;
					y0 = y * stepY - height / 2; y1 = y0 + stepY;
					u0 = x0 / width + 0.5f; u1 = x1 / width + 0.5f;
					v0 = y0 / height + 0.5f; v1 = y1 / height + 0.5f;
					u0 *= uvSizeX; u1 *= uvSizeX;
					v0 *= uvSizeY; v1 *= uvSizeY;

					n00 = new Vector3();
					n10 = new Vector3();
					n01 = new Vector3();
					n11 = new Vector3();

					plane.vertex.Add(new Vector3(x0, y0, offset)); plane.uv0.Add(new Vector2(u0, v0));
					plane.vertex.Add( new Vector3( x1, y0, offset ) ); plane.uv0.Add ( new Vector2(u1, v0) );
					plane.vertex.Add( new Vector3( x1, y1, offset) ); plane.uv0.Add ( new Vector2(u1, v1) );
					plane.vertex.Add(new Vector3(x0, y1, offset)); plane.uv0.Add(new Vector2(u0, v1));
					plane.vertex.Add( new Vector3( x0, y0, offset) ); plane.uv0.Add ( new Vector2(u0, v0) );
					plane.vertex.Add( new Vector3( x1, y1, offset) ); plane.uv0.Add ( new Vector2(u1, v1) );

					if (SmoothShading) {
						// normals will be different for different parts of the plane
						// in case of smooth shading

						if (x == 0 && y == 0) {
							// bottom right corner
							if (segmentsY > 1) {
								if (segmentsX > 1) {
									n00.Set(-1, -1, 1);
									n10.Set(0, -1, 1);
									n01.Set(-1, 0, 1);
									n11.Set(0, 0, 1);
								} else {
									n00.Set(-1, -1, 1);
									n10.Set(1, -1, 1);
									n01.Set(-1, 0, 1);
									n11.Set(1, 0, 1);
								}
							} else if (segmentsX == 1) {
								n00.Set(-1, -1, 1);
								n10.Set(1, -1, 1);
								n01.Set(-1, 1, 1);
								n11.Set(1, 1, 1);
							} else {
								n00.Set(-1, -1,  1);
								n10.Set( 0, -1,  1);
								n01.Set(-1,  1,  1);
								n11.Set( 0,  1,  1);
							}
						} else if (x == 0 && y == segmentsY-1 && segmentsY > 1) {
							// top right corner
							if (segmentsX > 1) {
								n00.Set(-1,  0,  1);
								n10.Set( 0,  0,  1);
								n01.Set(-1,  1,  1);
								n11.Set( 0,  1,  1);
							} else {
								n00.Set(-1,  0,  1);
								n10.Set( 1,  0,  1);
								n01.Set(-1,  1,  1);
								n11.Set( 1,  1,  1);
							}
						} else if (x == segmentsX-1 && y == 0){
							// bottom left corner
							if (segmentsY > 1) {
								n00.Set( 0, -1,  1);
								n10.Set( 1, -1,  1);
								n01.Set( 0,  0,  1);
								n11.Set( 1,  0,  1);
							} else if(segmentsX == 1){
								n00.Set(-1, -1,  1);
								n10.Set( 1, -1,  1);
								n01.Set(-1,  1,  1);
								n11.Set( 1,  1,  1);
							} else {
								n00.Set( 0, -1,  1);
								n10.Set( 1, -1,  1);
								n01.Set( 0,  1,  1);
								n11.Set( 1,  1,  1);
							}
						} else if (x == segmentsX-1 && y == segmentsY-1 && segmentsY > 1){
							// top left corner
							n00.Set( 0,  0,  1);
							n10.Set( 1,  0,  1);
							n01.Set( 0,  1,  1);
							n11.Set( 1,  1,  1);
						} else if (x == 0) {
							// right edge
							if (segmentsX > 1) {
								n00.Set(-1, 0, 1);
								n10.Set(0, 0, 1);
								n01.Set(-1, 0, 1);
								n11.Set(0, 0, 1);
							} else {
								n00.Set(-1, 0, 1);
								n10.Set(1, 0, 1);
								n01.Set(-1, 0, 1);
								n11.Set(1, 0, 1);
							}
						} else if (x == segmentsX - 1) {
							// left edge
							n00.Set( 0,  0,  1);
							n10.Set( 1,  0,  1);
							n01.Set( 0,  0,  1);
							n11.Set( 1,  0,  1);
						} else if (y == 0) {
							// bottom edge
							if (segmentsY > 1) {
								n00.Set(0, -1, 1);
								n10.Set(0, -1, 1);
								n01.Set(0,  0, 1);
								n11.Set(0,  0, 1);
							} else {
								n00.Set(0, -1, 1);
								n10.Set(0, -1, 1);
								n01.Set(0,  1, 1);
								n11.Set(0,  1, 1);
							}
						} else if (y == segmentsY - 1 && segmentsY > 1) {
							// top edge
							n00.Set( 0,  0,  1);
							n10.Set( 0,  0,  1);
							n01.Set( 0,  1,  1);
							n11.Set( 0,  1,  1);
						} else {
							n00.Set(0, 0, 1);
							n10.Set(0, 0, 1);
							n01.Set(0, 0, 1);
							n11.Set(0, 0, 1);
						}
					} else {
						n00.Set(0, 0, 1);
						n10.Set(0, 0, 1);
						n01.Set(0, 0, 1);
						n11.Set(0, 0, 1);
					}

					plane.normal.Add(n00.normalized);
					plane.normal.Add(n10.normalized);
					plane.normal.Add(n11.normalized);

					plane.normal.Add(n01.normalized);
					plane.normal.Add(n00.normalized);
					plane.normal.Add(n11.normalized);
				}
			}
			return plane;
		}

		public override void CreateData ()
		{
			base.CreateData ();
			int i = 0;
			int index = 0;
			//////////////
			// Z Planes //
			//////////////
			MeshData zplane = Plane( width, height, widthSegments, heightSegments, uvWidth, uvHeight, depth/2 );
			// +z
			i = 0;
			while (i < zplane.vertex.Count) {
				meshData.vertex.Add( zplane.vertex[i] );
				meshData.normal.Add( zplane.normal[i] );
				meshData.uv0.Add(zplane.uv0[i]);
				meshData.index.Add(index);
				index++;
				i++;
			}
			// -z
			i = 0;
			while (i < zplane.vertex.Count) {
				meshData.vertex.Add(new Vector3( -zplane.vertex[i].x, zplane.vertex[i].y, -zplane.vertex[i].z));
				meshData.normal.Add(new Vector3( -zplane.normal[i].x, zplane.normal[i].y, -zplane.normal[i].z));
				meshData.uv0.Add(new Vector2( 1f-zplane.uv0[i].x, zplane.uv0[i].y));
				meshData.index.Add(index);
				index++;
				i++;
			}

			//////////////
			// X Planes //
			//////////////
			MeshData xplane = Plane(depth, height, depthSegments, heightSegments, uvDepth, uvHeight, width / 2);
			// +x
			i = 0;
			while (i < xplane.vertex.Count) {
				meshData.vertex.Add(new Vector3( xplane.vertex[i].z, xplane.vertex[i].y, -xplane.vertex[i].x));
				meshData.normal.Add(new Vector3( xplane.normal[i].z, xplane.normal[i].y, -xplane.normal[i].x));
				meshData.uv0.Add(xplane.uv0[i]);
				meshData.index.Add(index);
				index++;
				i++;
			}
			// -x
			i = 0;
			while (i < xplane.vertex.Count) {
				meshData.vertex.Add(new Vector3(-xplane.vertex[i].z, xplane.vertex[i].y, xplane.vertex[i].x));
				meshData.normal.Add(new Vector3(-xplane.normal[i].z, xplane.normal[i].y, xplane.normal[i].x));
				meshData.uv0.Add(xplane.uv0[i]);
				meshData.index.Add(index);
				index++;
				i++;
			}

			//////////////
			// Y Planes //
			//////////////
			MeshData yplane = Plane(depth, width, depthSegments, widthSegments, uvDepth, uvWidth, height / 2);
			// +y
			i = 0;
			while (i < yplane.vertex.Count) {
				meshData.vertex.Add(new Vector3(yplane.vertex[i].y, yplane.vertex[i].z, yplane.vertex[i].x));
				meshData.normal.Add(new Vector3(yplane.normal[i].y, yplane.normal[i].z, yplane.normal[i].x));
				meshData.uv0.Add(yplane.uv0[i]);
				meshData.index.Add(index);
				index++;
				i++;
			}
			// -y
			i = 0;
			while (i < yplane.vertex.Count) {
				meshData.vertex.Add(new Vector3(yplane.vertex[i].y, -yplane.vertex[i].z, -yplane.vertex[i].x));
				meshData.normal.Add(new Vector3(yplane.normal[i].y, -yplane.normal[i].z, -yplane.normal[i].x));
				meshData.uv0.Add(yplane.uv0[i]);
				meshData.index.Add(index);
				index++;
				i++;
			}

			mesh.name = "Generated_Box";
		}

		public override bool ValidateParameters ()
		{
			return (width > 0 && height > 0 && widthSegments > 0 && heightSegments > 0);
		}
	}
}