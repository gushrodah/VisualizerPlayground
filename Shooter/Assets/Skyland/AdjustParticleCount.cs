using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustParticleCount : MonoBehaviour
{
	public int minParts, maxParts;
	ParticleSystem.EmissionModule parts;

	bool isLow;

	void Start()
	{
		parts = GetComponent<ParticleSystem>().emission;
		isLow = true;
	}

	public void ToggleNumParts()
	{
		if (isLow)
		{
			parts.rateOverDistance = maxParts;
		}
		else
		{
			parts.rateOverDistance = minParts;
		}
		isLow = !isLow;
	}
}
