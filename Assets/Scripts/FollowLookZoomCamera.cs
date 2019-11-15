using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Cameras;
using UnityEngine.EventSystems;

public class FollowLookZoomCamera : MonoBehaviour
{
	public Transform target;
	public float followSpeed = 1f;
	public float lookSpeed = 1.5f;
	public float timeToLookAtTarget = 1f;
	public Vector2 tiltLimits = new Vector2( -45f, 75f ); // limits of the x axis rotation of the pivot
	public float zoomSpeedScroll = 1f;
	public float zoomSpeedArrows = 0.1f;
	public Vector2 zoomLimits = new Vector2( -5f, -50f );
	public KeyCode zoomInKey = KeyCode.UpArrow;
	public KeyCode zoomOutKey = KeyCode.DownArrow;
	public bool useScrollToZoom = false;

	float lookAngle = 0; // rig's y axis rotation.
	float tiltAngle = 0; // pivot's x axis rotation.
	Vector3 pivotEulers;
	bool animatingLook;
	float animationStartTime;
	bool animateTilt = false;
    bool interacting = false;
    float lastInteractTime = -100f;
    Quaternion startTiltRotation;
    Quaternion startLookRotation;
    float interactTime = 1f;

	static FollowLookZoomCamera _Instance;
	public static FollowLookZoomCamera Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = GameObject.FindObjectOfType<FollowLookZoomCamera>();
			}
			return _Instance;
		}
	}

	Transform _cam;
	Transform cam
	{
		get
		{
			if (_cam == null)
			{
				_cam = GetComponentInChildren<Camera>().transform;
			}
			return _cam;
		}
	}

	Transform _pivot;
	Transform pivot
	{
		get
		{
			if (_pivot == null)
			{
				_pivot = cam.parent;
			}
			return _pivot;
		}
	}

	Rigidbody _targetRigidbody;
	Rigidbody targetRigidbody
	{
		get
		{
			if (_targetRigidbody == null && target != null)
			{
				_targetRigidbody = target.GetComponent<Rigidbody>();
			}
			return _targetRigidbody;
		}
	}

	AmbientCameraMover _ambientMover;
	AmbientCameraMover ambientMover
	{
		get
		{
			if (_ambientMover == null)
			{
				_ambientMover = GetComponentInChildren<AmbientCameraMover>();
			}
			return _ambientMover;
		}
	}

	bool dragging
	{
		get
		{
//			return CrossPlatformInputManager.GetButton( "Fire1" ) && !EventSystem.current.IsPointerOverGameObject();
            return (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)
            || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow));
		}
	}

	bool targetIsPhysicsObject
	{
		get
		{
			return targetRigidbody != null && !targetRigidbody.isKinematic;
		}
	}

	void Awake()
	{
		pivotEulers = pivot.rotation.eulerAngles;
        ambientMover.StartPush();
        ambientMover.StartRotation();
	}

	void Update () 
	{
		DragToLook();
		AnimateRotation();
//		ScrollToZoom();
        ZoomWithKeys();
	}

	// ----------------------------------------------------------------------------------- Follow

	void FixedUpdate ()
	{
		if (targetIsPhysicsObject)
		{
			FollowTarget();
		}
	}

	void LateUpdate ()
	{
		if (!targetIsPhysicsObject)
		{
			FollowTarget();
            DoAmbientLook();
            DoAmbientZoom();
		}
	}

	void FollowTarget ()
	{
		if (target != null) 
		{
			transform.position = Vector3.Lerp( transform.position, target.position, Time.deltaTime * followSpeed );
		}
	}

	// ----------------------------------------------------------------------------------- Look

	void DragToLook ()
	{
		if (dragging)
		{
            if (!interacting)
            {
                lookAngle = transform.localRotation.eulerAngles.y;
                tiltAngle = pivot.localRotation.eulerAngles.x;
                if (tiltAngle > 180f) { tiltAngle -= 360f; }
                tiltAngle = Mathf.Clamp( tiltAngle, tiltLimits.x, tiltLimits.y );
                startTiltRotation = pivot.localRotation;
                startLookRotation = transform.localRotation;
                lastInteractTime = Time.time;
            }
			HandleRotationInput();
            interacting = true;
		}
	}
    
    void DoAmbientLook ()
    {
        if (!dragging)
        {
            if (Time.time - lastInteractTime > interactTime && interacting)
            {
                ambientMover.StartRotation();
                startTiltRotation = pivot.localRotation;
                startLookRotation = transform.localRotation;
                lastInteractTime = Time.time;
            }
            ambientMover.AmbientlyRotate();
            interacting = false;
        }
    }

	void HandleRotationInput ()
	{
//		var x = CrossPlatformInputManager.GetAxis("Mouse X");
//		var y = CrossPlatformInputManager.GetAxis("Mouse Y");

        var x = (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + (Input.GetKey(KeyCode.RightArrow) ? 1 : 0);
		var y = (Input.GetKey(KeyCode.DownArrow) ? -1 : 0) + (Input.GetKey(KeyCode.UpArrow) ? 1 : 0);

		lookAngle += x * lookSpeed * (Input.GetKey(KeyCode.Space) ? 10 : 1);
		tiltAngle -= y * lookSpeed * (Input.GetKey(KeyCode.Space) ? 10 : 1);
		tiltAngle = Mathf.Clamp( tiltAngle, tiltLimits.x, tiltLimits.y );

		SetRotation();
	}

	void SetRotation ()
	{
//		pivot.localRotation = goalTiltRotation;
//		transform.localRotation = goalLookRotation;
        
        pivot.localRotation = Quaternion.Slerp(startTiltRotation, goalTiltRotation, Mathf.SmoothStep(0, 1f, (Time.time - lastInteractTime) / interactTime));
        transform.localRotation = Quaternion.Slerp(startLookRotation, goalLookRotation, Mathf.SmoothStep(0, 1f, (Time.time - lastInteractTime) / interactTime));
	}

	public void LookAtTarget ()
	{
		Vector3 localTarget = cam.InverseTransformPoint( target.position );
		lookAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;

		if (animateTilt)
		{
			tiltAngle = Mathf.Atan2( localTarget.y, localTarget.z ) * Mathf.Rad2Deg;
			tiltAngle = Mathf.Clamp( tiltAngle, tiltLimits.x, tiltLimits.y );
		}

		animatingLook = true;
		animationStartTime = Time.time;
	}

	Quaternion goalLookRotation
	{
		get
		{
			return Quaternion.Euler( 0f, lookAngle, 0f );
		}
	}

	Quaternion goalTiltRotation
	{
		get
		{
			return Quaternion.Euler( tiltAngle, pivotEulers.y , pivotEulers.z );
		}
	}

	void AnimateRotation ()
	{
		if (animatingLook && !dragging)
		{
			float t = (Time.time - animationStartTime) / timeToLookAtTarget;
			if (t <= 1f)
			{
				if (animateTilt)
				{
					pivot.localRotation = Quaternion.Slerp( pivot.localRotation, goalTiltRotation, t );
				}
				transform.localRotation = Quaternion.Slerp( transform.localRotation, goalLookRotation, t );
			}
			else
			{
				animatingLook = false;
			}
		}
	}

	// ----------------------------------------------------------------------------------- Zoom
    
    int zoomDirection 
    {
        get
        {
            int dir = 0;
            if (Input.GetKey( zoomInKey ))
            {
                dir++;
            }
            else if (Input.GetKey( zoomOutKey ))
            {
                dir--;
            }
            return dir;
        }
    }

	void ScrollToZoom ()
	{
		if (useScrollToZoom)
		{
            Zoom(Mathf.RoundToInt(CrossPlatformInputManager.GetAxis( "Mouse ScrollWheel" )), zoomSpeedScroll);
            return;
		}
        
//		float speed = zoomSpeedArrows;
//		Zoom(zoomDirection);
	}
    
    bool overridingZoom = false;
    
    void ZoomWithKeys ()
	{
        int dir = zoomDirection;
		if (dir != 0)
		{
			Zoom(dir, zoomSpeedArrows);
            overridingZoom = true;
		}
	}
    
    void DoAmbientZoom ()
    {
        if (zoomDirection == 0)
        {
            if (overridingZoom)
            {
                ambientMover.StartPush();
            }
            ambientMover.AmbientlyPush();
            overridingZoom = false;
        }
    }
    
    void Zoom (int delta, float speed)
    {
        if (delta > 0) // zoom in
		{
			if (cam.localPosition.z < -Mathf.Abs( zoomLimits.x ))
			{
				cam.localPosition += speed * Vector3.forward;
			}
		}
		else if (delta < 0) // zoom out
		{
			if (cam.localPosition.z > -Mathf.Abs( zoomLimits.y ))
			{
				cam.localPosition -= speed * Vector3.forward;
			}
		}
    }
}
