using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public int n = 10;
    public Vector2 scaleRange = new Vector2( 0.8f, 1.2f );

    GameObject prefab
    {
        get
        {
            return prefabs[Random.Range(0, prefabs.Length)];
        }
    }

    protected void SpawnObject (Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate( prefab, transform );
        obj.transform.localPosition = position;
        obj.transform.localRotation = rotation;
        obj.transform.localScale = Random.Range( scaleRange.x, scaleRange.y ) * Vector3.one;
    }
}
