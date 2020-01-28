using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorForce : MonoBehaviour
{
	float startForce;
	ParticleSystemForceField force;
	public bool low;

	private const float highForce = .2f;

    void Start()
    {
		force = GetComponent<ParticleSystemForceField>();
		startForce = force.gravity.constant;
		low = true;
    }


	public void SwitchForceStrength()
	{
		if (low)
		{
			// switch to high
			force.gravity = highForce;
			Debug.Log("to high");
		}
		else{
			// switch to low
			force.gravity = startForce;
			Debug.Log("to low");
		}
		low = !low;
	}
}
