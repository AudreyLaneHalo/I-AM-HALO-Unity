using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaymarchingToolkit.Examples {

[ExecuteInEditMode()]
public class AttachObjectsToParticles : MonoBehaviour
{
	public ParticleSystem system;
	public RaymarchObject[] objects;

	UnityEngine.ParticleSystem.Particle[] _particles;
	
	void Start()
	{
		var main = system.main;
		main.maxParticles = objects.Length;
		_particles = new UnityEngine.ParticleSystem.Particle[objects.Length];
	}

	void Update ()
	{
		system.GetParticles(_particles);
		if (_particles == null)
			return;
		var main = system.main;
		for (int i = 0; i < objects.Length; ++i)
		{
			var radiusInput = objects[i].GetObjectInput("radius");
			if (i >= _particles.Length)
			{
				radiusInput.SetFloat(0);
				continue;
			}
			var pos = main.simulationSpace == ParticleSystemSimulationSpace.Local ? system.transform.TransformPoint(_particles[i].position) : _particles[i].position;
			objects[i].transform.position = pos;
			radiusInput.SetFloat(_particles[i].GetCurrentSize(system) * .5f);
		}
	}
}
}