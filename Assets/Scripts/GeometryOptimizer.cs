using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryOptimizer : MonoBehaviour
{
    MeshRenderer _theRenderer;
    MeshRenderer theRenderer
    {
        get
        {
            if (_theRenderer == null)
            {
                _theRenderer = GetComponent<MeshRenderer>();
            }
            return _theRenderer;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        theRenderer.enabled = true;
    }

    void OnTriggerExit(Collider other)
    {
        theRenderer.enabled = false;
    }
}
