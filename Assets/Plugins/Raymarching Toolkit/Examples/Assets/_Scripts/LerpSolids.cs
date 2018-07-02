using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RaymarchingToolkit;

namespace RaymarchingToolkit.Examples
{

public class LerpSolids : MonoBehaviour
{
	public RaymarchBlend xBlend;
	public RaymarchBlend yBlend;

	void Update ()
	{
		var viewportPt = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		xBlend.GetInput("whichObject").SetFloat(viewportPt.x);
		yBlend.GetInput("whichObject").SetFloat(viewportPt.y);
	}
}

}