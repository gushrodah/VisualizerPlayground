using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

public class ControllerEvent : MonoBehaviour
{
	[SerializeField]
	private SteamVR_Input_Sources source;
	[SerializeField]
	public SteamVR_Action_Boolean TriggerAction, GripAction;

	public UnityEvent OnTriggerDown, OnTriggerUp;
	public UnityEvent OnGripDown, OnGripUp;

	private void Update()
	{
		if (TriggerAction.GetStateDown(source))
		{
			if (OnTriggerDown != null) OnTriggerDown.Invoke();
		}
		else if (TriggerAction.GetStateUp(source))
		{
			if (OnTriggerUp != null) OnTriggerUp.Invoke();
		}
		if (GripAction.GetStateDown(source))
		{
			if (OnGripDown != null) OnGripDown.Invoke();
		}
		else if (GripAction.GetStateUp(source))
		{
			if (OnGripUp != null) OnGripUp.Invoke();
		}
	}
}
