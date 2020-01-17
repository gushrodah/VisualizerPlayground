using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderController : Actionable
{
	private const float disabledDuration = 1;

	public override void TriggerAction()
	{
		GetComponent<Collider>().enabled = false;
		Invoke("EnableCollider", disabledDuration);
	}

	void EnableCollider()
	{
		GetComponent<Collider>().enabled = true;
	}
}
