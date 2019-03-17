using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour 
{
    public GameObject[] worlds;

    public void ToggleWorlds (bool on)
    {
        foreach (GameObject world in worlds)
        {
            Debug.Log(world.name + " " + on);
            world.SetActive( on );
        }
    }
}
