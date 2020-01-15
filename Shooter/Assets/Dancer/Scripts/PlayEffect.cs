using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;
public class PlayEffect : MonoBehaviour {

	public GameObject gripPrefab;
	public GameObject triggerPrefab;

	//controlelr stuff
	public SteamVR_Input_Sources source;
	public SteamVR_Action_Boolean TriggerAction, GripAction;

	private void Update()
	{
		if (GripAction.GetStateDown(source))
		{
			Instantiate(gripPrefab, transform.position, Quaternion.identity);
		}
		else if (GripAction.GetStateUp(source))
		{
			Instantiate(triggerPrefab, transform.position, Quaternion.identity);
		}
	}
}
