using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {
public class LogoAnimation : MonoBehaviour {

	public Transform box;
	public Transform boxTarget;
	public Transform sphereTarget;

	public float frequency = 2;
	public Vector3[] distances = new Vector3[]{
		Vector3.zero,
		new Vector3(.45f,.45f,0)
	};

	void Start () {
		StartCoroutine(AnimationSequence());
	}

	IEnumerator AnimationSequence() {
		yield return 0;
		Vector3 p;
		while(true) {
			foreach(var d in distances) {
				p = d;
				boxTarget.position = p;
				sphereTarget.position = -p;
				yield return new WaitForSeconds(frequency);
			}
		}
	}
}
}