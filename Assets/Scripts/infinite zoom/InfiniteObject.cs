using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL.InfiniteZoom
{
	[System.Serializable]
	public class ZoomLevel
	{
		public GameObject prefab;
		public Vector2 scaleRange = new Vector2( 0.01f, 100f );

        [HideInInspector]
        public int index;
        float zoomRange = 2f;
        Vector2 logScaleRange;
        InfiniteObject inifiniteObject;

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

		public void Setup (int _index, InfiniteObject _inifiniteObject)
		{
            index = _index;
			inifiniteObject = _inifiniteObject;
		}

		public float minZoom
		{
			get
			{
				return index - zoomRange / 2f ;
			}
		}

		public float maxZoom
		{
			get
			{
				float max = index + zoomRange / 2f;
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

	public class InfiniteObject : MonoBehaviour 
	{
		public List<ZoomLevel> levels;

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

					UpdateZoomLevels();
				}
			}
		}

		void UpdateZoomLevels ()
		{
			foreach (ZoomLevel level in levels)
			{
				level.Update();
			}
		}

		public float minZoom
		{
			get
			{
				return levels[0].minZoom;
			}
		}

		public float maxZoom
		{
			get
			{
				return levels[levels.Count - 1].index;
			}
		}

		void Start ()
		{
            int i = 1;
			foreach (ZoomLevel level in levels)
			{
				level.Setup( i, this );
                i++;
			}
			currentZoom = 0;
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