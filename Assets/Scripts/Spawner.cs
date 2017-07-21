using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public int n;
    public float radius;

	void Start ()
    {
        GameObject obj;
        for (int i = 0; i < n; i++)
        {
            obj = Instantiate( prefab, transform );
            obj.transform.localPosition = Random.Range( 0.1f, radius ) * Random.onUnitSphere;
            obj.transform.rotation = Random.rotation;
        }
	}
}
