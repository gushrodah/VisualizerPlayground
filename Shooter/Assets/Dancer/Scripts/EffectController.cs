using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;

/// <summary>
/*	- grip will spawn trail
 *	- trigger spanws burst
 *	- wheel controls trail size
 *	- wheel press toggles 2 types of bursts
*/
/// </summary>


public class EffectController : MonoBehaviour
{
	// music manager
	public NoteManager manager;

	//controlelr stuff
	public SteamVR_Input_Sources source;
	public SteamVR_Action_Boolean TriggerAction, GripAction;

	// particles that will shoot out 
	public ParticleSystem TrailParticles, BurstParticles;

	private ParticleSystem trailParts, burst;

	private Coroutine createRoutine;

	private void Start()
	{
		trailParts = Instantiate(TrailParticles, transform);
		trailParts.transform.localPosition = Vector3.zero;
		trailParts.transform.localScale = Vector3.one;
		trailParts.Stop();

		
	}

	void Update()
	{
		if (TriggerAction.GetStateDown(source))
		{
			burst = Instantiate(BurstParticles, transform);
			burst.transform.localPosition = Vector3.zero;
			burst.transform.localScale = Vector3.one;
			burst.Play();
		}
		else if (TriggerAction.GetStateUp(source))
		{
			burst.Stop();
			GameObject g = burst.gameObject;
			Destroy(g, 5);
		}
		if (GripAction.GetStateDown(source))
		{
			trailParts.Play();
		}
		else if (GripAction.GetStateUp(source))
		{
			trailParts.Stop();
		}
	}

	IEnumerator CreateParts()
	{
		while (true)
		{
			
			yield return null;
		}
		yield return 0;
	}
}
