using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscSpawner : Spawner
{
    public Vector2 discRadius = new Vector2( 0, 100f );

    void Start ()
    {
        for (int i = 0; i < n; i++)
        {
            SpawnRandomlyOnDisc();
        }
    }

    void SpawnRandomlyOnDisc ()
    {
        float theta = Random.Range( 0, 2 * Mathf.PI );
        SpawnObject( Random.Range( discRadius.x, discRadius.y ) * new Vector3( Mathf.Sin( theta ), 0, Mathf.Cos( theta ) ),
                     Quaternion.Euler( Random.Range( 0, 359f ) * Vector3.up ));
    }
}
