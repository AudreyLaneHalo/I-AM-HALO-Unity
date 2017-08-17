using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.MeshDeformation
{
	public class AmbientMeshDeformer : MonoBehaviour 
	{
		public bool addRandomForces = true;
		public float randomForceRate = 5f;
		public float meanRandomForce = 10f;
		public bool addNoise;
		public float noiseRate = 2f;
		public float noiseForce = 10f;

		MeshDeformer _deformer;
		MeshDeformer deformer
		{
			get
			{
				if (_deformer == null)
				{
					_deformer = GetComponentInChildren<MeshDeformer>();
				}
				return _deformer;
			}
		}

		void Update ()
		{
			if (addRandomForces)
			{
				AddRandomForces();
			}
			if (addNoise)
			{
				AddNoise();
			}
		}

		void AddRandomForces ()
		{
			if (Random.Range( 0, 1f ) <= randomForceRate * Time.deltaTime)
			{
				deformer.AddRandomForces( 1, meanRandomForce );
			}
		}

		void AddNoise ()
		{
			if (Random.Range( 0, 1f ) <= noiseRate * Time.deltaTime)
			{
				deformer.AddRandomNoise( noiseForce );
			}
		}
	}
}