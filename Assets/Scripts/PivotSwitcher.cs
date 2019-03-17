using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotSwitcher : MonoBehaviour 
{
    public Transform observer;
    public Pivot[] pivots;

    int currentIndex;

    void Start ()
    {
        for (int i = 0; i < pivots.Length; i++)
        {
            pivots[i].ToggleWorlds( i == currentIndex );
        }

        observer.position = pivots[currentIndex].transform.position;
        observer.rotation = pivots[currentIndex].transform.rotation;
    }

	void Update () 
    {
        if (Input.GetKeyUp( KeyCode.UpArrow ))
        {
            SwitchToPivot( currentIndex + 1 >= pivots.Length ? 0 : currentIndex + 1 );
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SwitchToPivot( currentIndex - 1 < 0 ? pivots.Length - 1 : currentIndex - 1 );
        }
	}

    void SwitchToPivot (int index)
    {
        Debug.Log("Switch to " + index);
        pivots[currentIndex].ToggleWorlds( false );
        pivots[index].ToggleWorlds( true );

        observer.position = pivots[index].transform.position;
        observer.rotation = pivots[index].transform.rotation;

        currentIndex = index;
    }
}
