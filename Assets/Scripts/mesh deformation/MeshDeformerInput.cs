using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.MeshDeformation
{
	// Add a force to a MeshDeformer when clicked
	public class MeshDeformerInput : MonoBehaviour 
	{
		public float force = 10f;
		public float forceOffset = 0.1f;

		void Update () 
		{
			if (Input.GetMouseButton( 0 )) 
			{
				DeformMeshOnClick();
			}
		}

		void DeformMeshOnClick () 
		{
			Ray inputRay = Camera.main.ScreenPointToRay( Input.mousePosition );
			RaycastHit hit;
			if (Physics.Raycast( inputRay, out hit )) 
			{
				MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer>();
				if (deformer != null) 
				{
					deformer.AddDeformingForce( hit.point + forceOffset * hit.normal, force );
				}
			}
		}
	}
}