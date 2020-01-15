using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

public enum Hand { LEFT, RIGHT }

public class Transistor : MonoBehaviour {

	// music manager
	public NoteManager manager;

	//controlelr stuff
	public SteamVR_Input_Sources source;
	public SteamVR_Action_Boolean TriggerAction, TouchpadAction;

	// 100% and can shoot
	public int charge = 0;
	const int amtPerCharge = 5;
	public Hand hand;

	// particles that will shoot out 
	public ParticleSystem LightParticles;

	// meter charge
	public GameObject ChargeVisual;

	public UnityEvent<Hand> OnChargedShot;
	private bool chargeReady = false;
	
	void Start () {
		
	}
	
	void Update () {
		// TODO : check absorb neg particles
		if (TriggerAction.GetStateDown(source))
		{
			// call event that successfully hit
			//manager.OnBeatHit.Invoke(transform);
			AbsorbNearestNote();
		}
		// check shoot positive particles
		if (chargeReady && TouchpadAction.GetStateDown(source))
		{
			Shoot();
		}
	}

	// change to called internall only (accessible only to intro)
	// _amnt = specific precent to skip to
	public void ChangeCharge(int _increment = amtPerCharge)
	{
		charge += _increment;

		chargeReady = charge >= 20;

		if (charge <= 100)
		{
			// physical meter
			ChargeVisual.transform.localScale = new Vector3(1, 1, charge / 100f);
		}
		else charge = 100;
	}
	
	private void Shoot()
	{
		LightParticles.Play();
		ChangeCharge(-20);
		ShootNearestNote();
	}

	private void ShootNearestNote()
	{
		RaycastHit hit;
		if (Physics.SphereCast(transform.position, .5f, LightParticles.transform.forward, out hit, 10))
		{
			Debug.Log(hit.collider.name);
			Note hitNote = hit.collider.gameObject.GetComponent<Note>();
			if (hitNote != null) hitNote.CheckCanAbsorb(transform);
		}
		else
		{
			Debug.Log("Nothing hit");
		}
	}

	private void AbsorbNearestNote()
	{
		RaycastHit hit;
		if (Physics.SphereCast(transform.position, 1f, LightParticles.transform.forward, out hit, 10))
		{
			//Debug.Log(hit.collider.name);
			Note hitNote = hit.collider.gameObject.GetComponent<Note>();
			if (hitNote != null)
			{
				hitNote.CheckCanAbsorb(transform);
				//increment charge
				ChangeCharge();
			}
		}
		else
		{
			Debug.Log("Nothing hit");
		}
	}

	IEnumerator StopShooting()
	{
		yield return new WaitForSeconds(4);
		LightParticles.Stop();
	}
}
