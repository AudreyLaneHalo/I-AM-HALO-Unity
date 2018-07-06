using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS
{
	public class ShellSpawner : Spawner 
	{	
		public Vector2 shellRadius = new Vector2( 20f, 50f );
		public float crowding = 0.75f;

		void Start () 
		{
            SpawnRandomlyOnSphere();
		}

		void SpawnRandomlyOnSphere ()
		{
			List<Vector3> positions = GetPointsOnSphere( Mathf.Max( n, Mathf.RoundToInt( n / Mathf.Clamp( crowding, 0.1f, 1f ) ) ) );
			for (int i = 0; i < n; i++)
			{
				int index = Random.Range( 0, positions.Count );
				SpawnObject( positions[index], Random.rotation );
				positions.RemoveAt( index );
			}
		}

		List<Vector3> GetPointsOnSphere (int n)
		{
			List<Vector3> points = new List<Vector3>();
			float inc = Mathf.PI * (3f - Mathf.Sqrt( 5f ));
			float off = 2f / n;
			float y = 0, r = 0, phi = 0;

			for (var i = 0; i < n; i++)
			{
				y = i * off - 1f + (off / 2f);
				r = Mathf.Sqrt( 1f - y * y );
				phi = i * inc;

				points.Add( Random.Range( shellRadius.x, shellRadius.y ) * new Vector3( r * Mathf.Cos( phi ), y, r * Mathf.Sin( phi ) ) );
			}
			return points;
		}
	}
}