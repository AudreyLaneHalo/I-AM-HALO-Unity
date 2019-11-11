using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AS.MicControl;

//This module has to be placed on your controller. This modules is also dependent on the sample amount.
public class Range_Selection : MonoBehaviour
{
    public float amplify = 1f;
    public bool remapZeroToOne;
    public MinMaxSpectrumRange[] selectRanges;
    MicControlC controller;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<MicControlC>();
    }

    // Update is called once per frame
    void Update()
    {

        float tmpAverage = 0f;
        foreach (MinMaxSpectrumRange specR in selectRanges)
        {

            for (int i = 0; i < controller.spectrumData.Length; i++)//loop through each float in the spectrumData array and filter based on min-max values.
            {
                if (i >= specR.min && i <= specR.max)
                {
                    tmpAverage += controller.spectrumData[i];
                }
                else
                {
                    tmpAverage += 0f;
                }

            }
            //after the loop is finished counting the array values average them out.
            tmpAverage = tmpAverage / controller.spectrumData.Length * amplify;

            if (remapZeroToOne)
            {
                tmpAverage = Mathf.InverseLerp(0f, 1f, tmpAverage);

            }


            specR.loudness = tmpAverage;
        }

    }



}

[Serializable]
public class MinMaxSpectrumRange
{
    public float min = 0f;
    public float max = 0f;

    public float loudness = 0f;


}

