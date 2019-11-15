using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryOptimizer : MonoBehaviour
{
    MeshRenderer[] _renderers;
    MeshRenderer[] renderers
    {
        get
        {
            if (_renderers == null)
            {
                _renderers = GetComponentsInChildren<MeshRenderer>();
            }
            return _renderers;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        foreach (MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (MeshRenderer _renderer in renderers)
        {
            _renderer.enabled = false;
        }
    }
}
