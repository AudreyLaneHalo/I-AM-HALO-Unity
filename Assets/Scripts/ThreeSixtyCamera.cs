using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ThreeSixtyCamera : MonoBehaviour
{
    public int faceToRender;
    public RenderTexture cubemap;

    Camera _camera;
    Camera cam
    {
        get
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }
            return _camera;
        }
    }

    void Update()
    {
        cam.RenderToCubemap(cubemap, 1 << faceToRender);
    }
}
