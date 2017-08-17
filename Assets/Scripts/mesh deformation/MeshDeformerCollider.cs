using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.MeshDeformation
{
	// Add a force to a MeshDeformer when colliding
	public class MeshDeformerCollider : MonoBehaviour 
	{
		public float force = 10f;
		public float forceOffset = 0.1f;

		MeshDeformer _deformer;
		MeshDeformer deformer
		{
			get
			{
				if (_deformer == null)
				{
					_deformer = GetComponentInParent<MeshDeformer>();
				}
				return _deformer;
			}
		}

		void OnCollisionStay (Collision collision) 
		{
			DeformMeshOnCollision( collision.contacts );
		}

		void DeformMeshOnCollision (ContactPoint[] contacts) 
		{
			foreach (ContactPoint contact in contacts)
			{
				deformer.AddDeformingForce( contact.point + forceOffset * contact.normal, -force );
			}
		}
	}
}