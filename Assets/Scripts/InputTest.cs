using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTest : MonoBehaviour
{
    public string[] m_axes;
    public Text m_output;
    
    void Update ()
    {
        string output = "";
        foreach (string axis in m_axes)
        {
            output += axis + " : " + Input.GetAxis(axis) + "\n";
        }
        m_output.text = output;
    }
}
