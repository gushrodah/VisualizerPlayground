using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.Events;

public class BeatTrigger : MonoBehaviour {

	[EventID]
	public string eventID;

	public UnityEvent OnBeatEvent;

	// Use this for initialization
	void Start () {
		// Register for Koreography Events.  This sets up the callback.
		Koreographer.Instance.RegisterForEvents(eventID, EventTrigger);
	}

	void EventTrigger(KoreographyEvent _event)
	{
		if (OnBeatEvent != null)
		{
			OnBeatEvent.Invoke();
		}
	}

}
