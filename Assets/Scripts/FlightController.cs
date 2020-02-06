using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour
{
    public string throttleAxisName = "Throttle";
    public string brakeAxisName = "Brake";
    public string verticalAxisName = "Vertical";
    public string horizontalAxisName = "Horizontal";
    public string rollAxisName = "Roll";
    
    public float turnSpeed = 10f;
    public float startSpeed = 50f;
    public float maxSpeed = 100f;
    
    float speed;
    float pitchInput;
    float yawInput;
    float rollInput;
    
    void Start ()
    {
        speed = startSpeed;
    }
    
    void Update ()
    {
        speed += 0.1f * (Input.GetAxis(throttleAxisName) - Input.GetAxis(brakeAxisName));

        transform.position += transform.forward * 0.001f * Mathf.Clamp(speed, -maxSpeed, maxSpeed);
        
        pitchInput = Input.GetAxis(verticalAxisName) * Time.deltaTime * 3f * turnSpeed;
        yawInput = Input.GetAxis(horizontalAxisName) * Time.deltaTime * turnSpeed;
        rollInput = Input.GetAxis(rollAxisName) * Time.deltaTime * 0.2f * turnSpeed;

        transform.Rotate(pitchInput, yawInput, -0.3f * yawInput + rollInput);
    } 
}