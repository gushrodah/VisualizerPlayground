using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MirzaBeig.ParticleSystems;

public class NoteFirefly : Note {

	public Rotator rotator;
	public Vector3 RotatorRange;

	private void Start()
	{
		RandomizeRotatorRange();
	}

	private void RandomizeRotatorRange()
	{
		rotator.localRotationSpeed = new Vector3(Random.Range(-RotatorRange.x, RotatorRange.x), Random.Range(-RotatorRange.y, RotatorRange.y), Random.Range(-RotatorRange.z, RotatorRange.z));
		rotator.worldRotationSpeed = new Vector3(Random.Range(-RotatorRange.x, RotatorRange.x), Random.Range(-RotatorRange.y, RotatorRange.y), Random.Range(-RotatorRange.z, RotatorRange.z));
	}

	public override void ChangeState(NoteState _state)
	{
		base.ChangeState(_state);

		if (_state == NoteState.NEUTRAL)
		{
			GetComponent<ParticleSystem>().Play();
		}
	}
}
