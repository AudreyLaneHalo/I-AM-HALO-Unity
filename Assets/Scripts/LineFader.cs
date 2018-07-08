using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(LineRenderer) )]
public class LineFader : MonoBehaviour
{
    public float lifetime = 1f;
    public float fadeTime = 0.4f;
    float startTime;
    public Color color;

    LineRenderer _renderer;
    LineRenderer lineRenderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<LineRenderer>();
                Debug.Log( _renderer == null );
                color = _renderer.material.GetColor( "_TintColor" );
            }
            return _renderer;
        }
    }

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time - startTime > lifetime - fadeTime)
        {
            if (Time.time - startTime < lifetime)
            {
                color.a = 1f - ((Time.time - startTime) - (lifetime - fadeTime)) / fadeTime;
                Debug.Log( color.a );
                lineRenderer.material.SetColor( "_TintColor", color );
            }
            else
            { 
                Destroy( gameObject );
            }
        }
    }
}