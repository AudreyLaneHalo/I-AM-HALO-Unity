using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOptimizer : MonoBehaviour
{
    Light _light;
    Light theLight
    {
        get
        {
            if (_light == null)
            {
                _light = GetComponent<Light>();
            }
            return _light;
        }
    }

    void Start ()
    {
        SphereCollider _collider = gameObject.AddComponent<SphereCollider>();
        _collider.radius = theLight.range;
    }

    void OnTriggerEnter (Collider other)
    {
        theLight.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        theLight.enabled = false;
    }
}
