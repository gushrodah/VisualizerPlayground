using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MirzaBeig.ParticleSystems;

public class RotatorRandomizer : MonoBehaviour
{
	[SerializeField]
	private float maxRot;

	public void ChangeRotation()
	{
		Rotator rot = GetComponent<Rotator>();
		rot.worldRotationSpeed = new Vector3(Random.Range(-maxRot, maxRot), Random.Range(-maxRot, maxRot), Random.Range(-maxRot, maxRot));
	}
}
