using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InfiniteZoom;

public class InputManager : MonoBehaviour 
{
	public InfiniteObject infiniteObject;

	static InputManager _Instance;
	public static InputManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = GameObject.FindObjectOfType<InputManager>();
			}
			return _Instance;
		}
	}

	public void Zoom (float delta)
	{
		Debug.Log( delta );
		infiniteObject.currentZoom += delta;
	}
}
