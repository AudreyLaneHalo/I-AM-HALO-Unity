using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BL.Splines;

public class Pivot : MonoBehaviour 
{
    public List<PathToNextPivot> paths;

    public PathToNextPivot GetPathToPivot (string pivotName)
    {
        return paths.Find(p => p.destinationName == pivotName);
    }
}

[System.Serializable]
public class PathToNextPivot
{
    public string destinationName;
    public Spline splineToNextPivot;
    public float speed;
}
