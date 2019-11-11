using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BL;

public class PivotSwitcher : MonoBehaviour 
{
    public Pivot[] pivots;

    int lastIndex;
    int currentIndex;
    bool switching;
    [SerializeField] PathToNextPivot currentPath;

    FollowSpline _followSpline;
    FollowSpline followSpline
    {
        get
        {
            if (_followSpline == null)
            {
                _followSpline = GetComponent<FollowSpline>();
                if (_followSpline == null)
                {
                    _followSpline = gameObject.AddComponent<FollowSpline>();
                }
            }
            return _followSpline;
        }
    }

    AmbientCameraMover _cameraMover;
    AmbientCameraMover cameraMover
    {
        get
        {
            if (_cameraMover == null)
            {
                _cameraMover = GetComponent<AmbientCameraMover>();
            }
            return _cameraMover;
        }
    }

    void Start ()
    {
        transform.position = pivots[currentIndex].transform.position;
        transform.rotation = pivots[currentIndex].transform.rotation;
    }

	void Update () 
    {
        if (switching)
        {
            followSpline.GoToNextSplinePosition(false);
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                SwitchToPivot(currentIndex + 1 >= pivots.Length ? 0 : currentIndex + 1);
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                SwitchToPivot(currentIndex - 1 < 0 ? pivots.Length - 1 : currentIndex - 1);
            }
        }

	}

    void SwitchToPivot (int index)
    {
        currentPath = pivots[currentIndex].GetPathToPivot(pivots[index].name);
        if (currentPath == null)
        {
            Debug.Log(pivots[currentIndex].name + " has no path to " + pivots[index].name);
            return;
        }

        Debug.Log("Switch to " + pivots[index].name);

        cameraMover.ambientlyRotate = false;

        followSpline.Setup(transform.eulerAngles, pivots[index].transform.rotation, 
            currentPath.splineToNextPivot, currentPath.speed, FinishSwitching);

        lastIndex = currentIndex;
        currentIndex = index;
        switching = true;
    }

    void FinishSwitching ()
    {
        switching = false;

        transform.position = pivots[currentIndex].transform.position;
        cameraMover.ambientlyRotate = true;
    }
}
