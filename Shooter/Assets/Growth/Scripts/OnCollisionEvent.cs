using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// todo : change to derive from class so that actions can be called ambiguously 
[RequireComponent(typeof(Collider))]
public class OnCollisionEvent : MonoBehaviour
{
	Collider col;
	private const float colDuration = .2f;

	public UnityEvent OnCollision;

	private void Start()
	{
		col = GetComponent<Collider>();
		col.isTrigger = true;
		col.enabled = false;
	}

	public void ActivateCollider()
	{
		if (col.enabled) return;

		col.enabled = true;
		Invoke("DeactivateCollider", colDuration);
	}

	public void DeactivateCollider()
	{
		col.enabled = false;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Interactable")
		{
			Debug.Log(col.name + " entered");
			// call events on this
			if (OnCollision != null) OnCollision.Invoke();

			// call action on other object
			if (col.gameObject.GetComponent<ActionHandler>() != null)
			{
				col.gameObject.GetComponent<ActionHandler>().TriggerEvent.Invoke();
			}

			// disable for next
			DeactivateCollider();
		}
	}
}
