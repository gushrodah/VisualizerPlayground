using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// todo : change to derive from class so that actions can be called ambiguously 
[RequireComponent(typeof(Collider))]
public class OnCollisionEvent : MonoBehaviour
{
	Collider col;
	private const float colDuration = .2f;

	public Action OnCollision;

	private void Start()
	{
		col = GetComponent<Collider>();
		col.isTrigger = true;
		col.enabled = false;
	}

	public void ActivateCollider()
	{
		col.enabled = true;
		Invoke("DeactivateCollider", colDuration);
	}

	public void DeactivateCollider()
	{
		col.enabled = false;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Interactable")
		{
			// action
			if (OnCollision != null) OnCollision.Invoke();

			// disable for next
			DeactivateCollider();
		}
	}
}
