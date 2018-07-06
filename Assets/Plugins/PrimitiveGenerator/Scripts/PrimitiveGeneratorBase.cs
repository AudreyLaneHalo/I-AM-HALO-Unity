using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace PrimitiveGenerator
{
	[ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
	[DisallowMultipleComponent]
    public class PrimitiveGeneratorBase : MonoBehaviour
    {
		public class MeshData {
			public List<Vector3> vertex;
			public List<Vector3> normal;
			public List<int> index;
			public List<Vector2> uv0;
			public List<Vector2> uv1;
			public MeshData(  ) { 
				vertex = new List<Vector3>();
				normal = new List<Vector3>();
				index = new List<int>();
				uv0 = new List<Vector2>();
				uv1 = new List<Vector2>();
			}
		}

		[HideInInspector]
		public MeshFilter mf;
		[HideInInspector]
		public Mesh mesh;
		public MeshData meshData = new MeshData();
		private const string _name = "GeneratedMesh";
		
		[Tooltip("Wether to calculate normals to create smooth corners")]
		public bool SmoothShading = false;
		[Tooltip("This will reduce vertex count but will add processing time.")]
		public bool MergeIdenticalVertices = false;
		[Tooltip("Regenerate mesh when parameters are changed. ( only in editor! )")]
		public bool autoReGenerate = true;

		

		private void OnEnable() {
			mf = GetComponent<MeshFilter>();
			mesh = new Mesh();
			mf.sharedMesh = mesh;
			mesh.name = _name;
			Generate();
		}

		public virtual void Generate() {
			CreateData();
			if (MergeIdenticalVertices) { 
				RemoveDoubleVertices();
			}
			FinalizeMesh();
		}

		public virtual void CreateData() {
			meshData.vertex = new List<Vector3>();
			meshData.uv0 = new List<Vector2>();
			meshData.normal = new List<Vector3>();
			meshData.index = new List<int>();
		}

		public virtual void FinalizeMesh() {
			mesh.Clear();
			if (meshData.vertex != null) { mesh.SetVertices( meshData.vertex ); }
			if (meshData.normal != null) { mesh.SetNormals(meshData.normal); }
			if (meshData.index != null) { mesh.SetIndices( meshData.index.ToArray(), MeshTopology.Triangles, 0); }
			if (meshData.uv0 != null) { mesh.SetUVs(0, meshData.uv0 ); }
			mesh.RecalculateBounds();
			mesh.RecalculateTangents();
		}

		public void RemoveDoubleVertices() {
			
			List<Vector2> newUv0 = new List<Vector2>();
			List<Vector3> newVertices = new List<Vector3>();
			List<Vector3> newNormals = new List<Vector3>();

			for (int i = 0; i < meshData.index.Count; i++) {
				Vector3 v0 = meshData.vertex[meshData.index[i]];
				Vector2 UV0 = meshData.uv0[meshData.index[i]];
				Vector3 n0 = meshData.normal[meshData.index[i]];
				bool exists = false;
				if (newVertices.Count > 0) {
					for (int j = 0; j < newVertices.Count; j++) {
						Vector3 v1 = newVertices[j];
						Vector2 UV1 = newUv0[j];
						Vector3 n1 = newNormals[j];
						if (v0.Equals(v1) && UV0.Equals(UV1) && n0.Equals(n1)) {
							exists = true;
							meshData.index[i] = j;
							break;
						}
					}
				}
				if (!exists) {
					newVertices.Add(v0);
					newUv0.Add(UV0);
					newNormals.Add(n0);
					meshData.index[i] = newVertices.Count-1;
				}
			}
			meshData.vertex = newVertices;
			meshData.uv0 = newUv0;
			meshData.normal = newNormals;
		}

		public virtual bool ValidateParameters(){
			return true;
		}

		void OnValidate(){
			if (autoReGenerate) {
				if (ValidateParameters ()) {
					Generate ();
				}
			}
		}

		void OnDestroy(){
			if(mf != null){
				mf.sharedMesh = null;
			}
			if (mesh != null) {
				mesh.Clear ();
				DestroyImmediate (mesh);
			}
		}
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PrimitiveGeneratorBase), true)]
    public class PrimitiveGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            
			PrimitiveGeneratorBase t = (PrimitiveGeneratorBase)target;
			EditorGUILayout.HelpBox("using 'Merge Identical Vertices' increases generation time greatly!", MessageType.Info);
			base.OnInspectorGUI();

			if (!t.ValidateParameters ()) {
				EditorGUILayout.HelpBox ("invalid Parameters entered!",MessageType.Error);
				return;
			}

			if (GUILayout.Button ("Generate")) {
				((PrimitiveGeneratorBase)target).Generate ();
			}

			if (t.mesh != null) {
				if (t.mesh.vertexCount < 65000) {
					EditorGUILayout.HelpBox("vertex count : " + t.mesh.vertexCount, MessageType.Info);
				} else {
					EditorGUILayout.HelpBox("vertex count : " + t.mesh.vertexCount+"\n Vertexcount is over 65000. This is unity's vertex limit per mesh. Generated mesh will have some faces missing in its shape!", MessageType.Warning);
				}
			}

			if (t.mesh.vertexCount > 0) {
				if (GUILayout.Button("Export Obj File")) { 
					string dir = EditorUtility.SaveFilePanel("Save mesh as *.OBJ", "/",t.mesh.name,"obj");
					if (dir.Length != 0) { 
						string fileString = "# OBJ Exported from Unity\n# artkalev@gmail.com\n";
						fileString += "o "+t.mesh.name+"\n";
						foreach (Vector3 v in t.meshData.vertex) {
							fileString += "v "+v.x+" "+v.y+" "+v.z+"\n";
						}
						foreach (Vector2 vt in t.meshData.uv0) { 
							fileString += "vt "+vt.x+" "+vt.y+"\n";
						}
						foreach (Vector3 n in t.meshData.normal) {
							fileString += "vn " + n.x + " " + n.y + " " + n.z + "\n";
						}
						for (int i = 0; i < t.meshData.index.Count; i+=3) {
							int i0 = t.meshData.index[i + 0] + 1;
							int i1 = t.meshData.index[i + 1] + 1;
							int i2 = t.meshData.index[i + 2] + 1;
							fileString += "f ";
							fileString += i0 + "/" + i0 + "/" + i0 + " ";
							fileString += i1 + "/" + i1 + "/" + i1 + " ";
							fileString += i2 + "/" + i2 + "/" + i2 + "\n";
						}
						File.WriteAllText(dir, fileString);
					}
				}
			}
        }

    }
#endif
}