using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOptimizer : MonoBehaviour
{   
    public float fadeDuration;
    [SerializeField]
    bool fading;
    
    float intensity;
    float t;
    int fadeDirection;
    
    Light _light;
    Light theLight
    {
        get
        {
            if (_light == null)
            {
                _light = GetComponent<Light>();
                intensity = _light.intensity;
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
        Fade(1);
    }

    void OnTriggerExit(Collider other)
    {
        theLight.enabled = false;
        Fade(-1);
    }

    void Fade (int _direction)
    {
        fadeDirection = _direction;
        t = _direction > 0 ? 0 : 1f;
        theLight.intensity = t * intensity;
        fading = true;
    }

    void Update()
    {
        if (fading)
        {
            t += fadeDirection * 1f / fadeDuration * Time.deltaTime;
            if ((fadeDirection > 0 && t >= 1f) || (fadeDirection < 0 && t <= 0))
            {
                t = fadeDirection > 0 ? 1f : 0;
                fading = false;
            }
            
            theLight.intensity = t * intensity;
        }
    }
}
