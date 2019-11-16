using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSwitcher : MonoBehaviour
{
    public GameObject[] avatars;
    
    int current;
    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            Switch();
        }
    }
    
    void Switch ()
    {
        int next = current < avatars.Length-1 ? current+1 : 0;
        print(next);
        avatars[current].SetActive(false);
        avatars[next].SetActive(true);
        current = next;
    }
}
