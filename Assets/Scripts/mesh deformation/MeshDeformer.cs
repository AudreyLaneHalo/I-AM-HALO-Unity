using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.MeshDeformation
{
	[System.Serializable]
	public class Vertex
	{
		public Vector3 originalPosition;
		public Vector3 originalNormal;
		public Vector3 velocity;

		public Vertex (Vector3 _originalPosition, Vector3 _originalNormal, Vector3 _velocity)
		{
			originalPosition = _originalPosition;
			originalNormal = _originalNormal;
			velocity = _velocity;
		}
	}

	// From Catlike Coding's Mesh Deformation: Making a Stress Ball (http://catlikecoding.com/unity/tutorials/mesh-deformation/)
	[RequireComponent( typeof(MeshFilter) )]
	public class MeshDeformer : MonoBehaviour 
	{
		public float springForce = 20f;
		public float damping = 5f;

		Mesh deformingMesh;
		Vertex[] vertices;
		Vector3[] currentVertexPositions;
		float uniformScale = 1f;

		void Start () 
		{
			deformingMesh = GetComponent<MeshFilter>().mesh; 
			currentVertexPositions = deformingMesh.vertices;
			Vector3[] normals = deformingMesh.normals;
			vertices = new Vertex[currentVertexPositions.Length];

			for (int i = 0; i < currentVertexPositions.Length; i++) 
			{
				vertices[i] = new Vertex( currentVertexPositions[i], normals[i], Vector3.zero );
			}
		}

		void Update () 
		{
			AnimateVertices();
		}

		void AnimateVertices ()
		{
			uniformScale = transform.localScale.x;
			for (int i = 0; i < vertices.Length; i++) 
			{
				UpdateVertex( i );
			}
			deformingMesh.vertices = currentVertexPositions;
			deformingMesh.RecalculateNormals();
		}

		void UpdateVertex (int i) 
		{
			Vector3 velocity = vertices[i].velocity;
			Vector3 displacement = uniformScale * (currentVertexPositions[i] - vertices[i].originalPosition);
			velocity -= displacement * springForce * Time.deltaTime;
			velocity *= 1f - damping * Time.deltaTime;

			vertices[i].velocity = velocity;
			currentVertexPositions[i] += velocity * (Time.deltaTime / uniformScale);
		}

		public void AddDeformingForce (Vector3 point, float force, bool local = true) 
		{
			for (int i = 0; i < vertices.Length; i++) 
			{
				vertices[i].velocity += CalculateAccelerationForVertex( currentVertexPositions[i], (local ? transform.InverseTransformPoint(point) : point), force );
			}
		}

		Vector3 CalculateAccelerationForVertex (Vector3 vertexPosition, Vector3 point, float force) 
		{
			Vector3 pointToVertex = uniformScale * (vertexPosition - point);
			return force / (1f + pointToVertex.sqrMagnitude) * Time.deltaTime * pointToVertex.normalized;
		}

		public void AddRandomForces (int n, float meanForce)
		{
			List<int> indices = GetRandomVertices( n );
			foreach (int index in indices)
			{
				AddDeformingForce( currentVertexPositions[index], SampleExponentialDistribution( meanForce ), false );
			}
		}

		List<int> GetRandomVertices (int n)
		{
			List<int> indices = new List<int>();
			float probability = (float)n / (float)vertices.Length;
			int j = 0;
			for (int i = 0; i < vertices.Length; i++)
			{
				if (Random.Range( 0, 1f ) <= probability)
				{
					indices.Add( i );

					j++;
					if (j >= n - 1)
					{
						break;
					}
				}
			}
			return indices;
		}

		public void AddRandomNoise (float meanForce)
		{
			for (int i = 0; i < vertices.Length; i++) 
			{
				vertices[i].velocity += SampleExponentialDistribution( meanForce ) * vertices[i].originalNormal;
			}
		}

		float SampleExponentialDistribution (float mean)
		{
			return Mathf.Log( Random.Range( float.Epsilon, 1f ) ) / (-1f / mean);
		}
	}
}