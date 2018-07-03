using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeSixtyCapture : MonoBehaviour 
{
    public RenderTexture cubemap;
    public RenderTexture output;
	
	void Update ()
    {
        cubemap.ConvertToEquirect( output, Camera.MonoOrStereoscopicEye.Mono );
    }
}
