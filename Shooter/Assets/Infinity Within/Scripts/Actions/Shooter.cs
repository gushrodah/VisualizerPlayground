using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Shooter : MonoBehaviour {
	public GameObject prefab;
	public Transform direction;
	public float velocity;

	public SteamVR_Input_Sources source;
	public SteamVR_Action_Boolean TriggerAction;

	private void Update()
	{
		if (TriggerAction.GetStateDown(source))
		{
			Shoot();
		}
	}

	public void Shoot()
	{
		GameObject g = Instantiate(prefab);
		g.transform.position = transform.position;
		g.GetComponent<Rigidbody>().AddForce(direction.forward * velocity, ForceMode.Impulse);
	}
}
