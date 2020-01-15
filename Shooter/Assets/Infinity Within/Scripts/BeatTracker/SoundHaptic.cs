using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SoundHaptic : MonoBehaviour {
	
	public SteamVR_Action_Vibration hapticAction;
	private AudioSource audioSource;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		
	}

	private void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source)
	{
		hapticAction.Execute(0, duration, frequency, amplitude, source);
	}
}
