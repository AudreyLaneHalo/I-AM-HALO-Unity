using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BL.Splines;

public class Pivot : MonoBehaviour 
{
    public GameObject[] worlds;
    public List<PathToNextPivot> paths;

    public PathToNextPivot GetPathToPivot (string pivotName)
    {
        return paths.Find(p => p.destinationName == pivotName);
    }

    public void ToggleWorlds (bool on)
    {
        foreach (GameObject world in worlds)
        {
            world.SetActive( on );
        }
    }
}

[System.Serializable]
public class PathToNextPivot
{
    public string destinationName;
    public Spline splineToNextPivot;
    public Vector2 worldBoundaryPercentOnSpline = new Vector2(40f, 60f);
}
