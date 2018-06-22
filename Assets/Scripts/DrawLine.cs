using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour 
{
	public float minimumPointDistance = 1f;

	LineRenderer currentLine;
	bool drawing;
	List<Vector3> points = new List<Vector3>();

	Vector3 lastPoint
	{
		get
		{
			return points[points.Count - 1];
		}
	}

	Transform _lines;
	Transform lines
	{
		get
		{
			if (_lines == null)
			{
				_lines = new GameObject( "Lines" ).transform;
			}
			return _lines;
		}
	}

	int _n;
	int n
	{
		get
		{
			_n++;
			return _n;
		}
	}

	bool shouldDraw
	{
		get
		{
			return Input.GetKey( KeyCode.Space );
		}
	}
	
	void Update () 
	{
		if (shouldDraw)
		{
			if (!drawing)
			{
				StartLine();
				drawing = true;
			}
			UpdateLine();
		}
		else
		{
			drawing = false;
		}
	}

	void StartLine ()
	{
		currentLine = new GameObject( "Line" + n ).AddComponent<LineRenderer>();
		currentLine.transform.SetParent( lines );
		points.Clear();
		points.Add( transform.position );
	}

	void UpdateLine ()
	{
		if (Vector3.Distance( transform.position, lastPoint ) >= minimumPointDistance)
		{
			points.Add( transform.position );
			currentLine.positionCount = points.Count;
			currentLine.SetPositions( points.ToArray() );
		}
	}
}
