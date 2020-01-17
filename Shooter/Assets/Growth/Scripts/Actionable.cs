using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ActionHandler))]
public class Actionable : MonoBehaviour
{
	public virtual void TriggerAction() { }

	private void OnEnable()
	{
		GetComponent<ActionHandler>().TriggerEvent.AddListener(TriggerAction);

	}
	private void OnDisable()

	{
		GetComponent<ActionHandler>().TriggerEvent.RemoveListener(TriggerAction);

	}
}
