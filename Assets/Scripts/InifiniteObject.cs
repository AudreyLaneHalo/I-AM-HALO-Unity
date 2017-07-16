using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ContinuousZoom
{
	[System.Serializable]
	public class ZoomObject
	{
		public GameObject prefab;
		public float zoomLocation;
		public float zoomRange = 2f;
		public Vector2 scaleRange = new Vector2( 0.01f, 100f );

		Vector2 logScaleRange;

		GameObject _instance;
		public GameObject instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.Instantiate( prefab, inifiniteObject.transform );
					instance.transform.localPosition = Vector3.zero;

					logScaleRange = new Vector2( Mathf.Log10( scaleRange.x ), Mathf.Log10( scaleRange.y ) );
				}
				return _instance;
			}
		}

		InifiniteObject inifiniteObject;
		public void SetInfiniteObject (InifiniteObject _inifiniteObject)
		{
			inifiniteObject = _inifiniteObject;
		}

		public float minZoom
		{
			get
			{
				return zoomLocation - zoomRange / 2f ;
			}
		}

		public float maxZoom
		{
			get
			{
				float max = zoomLocation + zoomRange / 2f;
				if (max > inifiniteObject.maxZoom)
				{
					max = zoomRange / 2f;
				}
				return max;
			}
		}

		public void Update ()
		{
			bool turnedOn = false;
			if (maxZoom < minZoom)
			{
				if (inifiniteObject.currentZoom <= maxZoom)
				{
					instance.transform.localScale = Mathf.Pow( 10f, (logScaleRange.y - logScaleRange.x) / 2f * inifiniteObject.currentZoom / (zoomRange / 2f) ) * Vector3.one;
					turnedOn = true;
				}
				else if (inifiniteObject.currentZoom >= minZoom)
				{
					instance.transform.localScale = Mathf.Pow( 10f, logScaleRange.x + (logScaleRange.y - logScaleRange.x) * (inifiniteObject.currentZoom - minZoom) / zoomRange ) * Vector3.one;
					turnedOn = true;
				}
			}
			else if (inifiniteObject.currentZoom >= minZoom && inifiniteObject.currentZoom <= maxZoom)
			{
				instance.transform.localScale = Mathf.Pow( 10f, logScaleRange.x + (logScaleRange.y - logScaleRange.x) * (inifiniteObject.currentZoom - minZoom) / zoomRange ) * Vector3.one;
				turnedOn = true;
			}

			if (turnedOn)
			{
				if (!instance.activeSelf)
				{
					instance.SetActive( true );
				}
			}
			else if (instance.activeSelf)
			{
				instance.SetActive( false );
			}
		}
	}

	public class InifiniteObject : MonoBehaviour 
	{
		public float startZoom;
		public List<ZoomObject> objects;

		[SerializeField]
		float _currentZoom = -1f;
		public float currentZoom
		{
			get
			{
				return _currentZoom;
			}
			set
			{
				if (value != _currentZoom)
				{
					_currentZoom = value;
					if (_currentZoom > maxZoom)
					{
						_currentZoom = minZoom;
					}
					else if (_currentZoom < minZoom)
					{
						_currentZoom = maxZoom;
					}

					UpdateZoomLevel();
				}
			}
		}

		public float minZoom
		{
			get
			{
				return objects[0].minZoom;
			}
		}

		public float maxZoom
		{
			get
			{
				return objects[objects.Count - 1].zoomLocation;
			}
		}

		void Start ()
		{
			foreach (ZoomObject obj in objects)
			{
				obj.SetInfiniteObject( this );
			}
			currentZoom = startZoom;
		}

		void UpdateZoomLevel ()
		{
			foreach (ZoomObject obj in objects)
			{
				obj.Update();
			}
		}

		void Update () // for testing
		{
			if (Input.GetKey( KeyCode.DownArrow ))
			{
				currentZoom -= 0.01f;
			}
			if (Input.GetKey( KeyCode.UpArrow ))
			{
				currentZoom += 0.01f;
			}
		}
	}
}