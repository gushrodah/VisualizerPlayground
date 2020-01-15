using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using System;

public class MusicManager : MonoBehaviour
{
	private static MusicManager _instance;

	public static MusicManager Instance
	{
		get { return _instance; }
		set { _instance = value; }
	}

	[Tooltip("The Event ID of the track to use for target generation.")]
	[EventID]
	public string eventID;

	[Tooltip("The number of milliseconds (both early and late) within which input will be detected as a Hit.")]
	[Range(150f, 300f)]
	public float hitWindowRangeInMS;

	// The list that will contain all events for specified event
	public List<KoreographyEvent> beatEvents = new List<KoreographyEvent>();

	// number of beats lead-in has
	public int numLeadInBeats;

	// lead-in event for pending event
	KoreographyEvent leadInEvent;
	// next event to be called
	KoreographyEvent pendingEvent { get { return beatEvents[pendingEventIndex]; } }
	// index of next event
	public int pendingEventIndex, leadInEventIndex;

	// action taken when lead 
	public static Action OnLeadIn;

	// Local cache of the Koreography loaded into the Koreographer component.
	Koreography playingKoreo;

	// Koreographer works in samples.  Convert the user-facing values into sample-time.  This will simplify
	//  calculations throughout.
	int hitWindowRangeInSamples;    // The sample range within which a viable event may be hit.

	// Public access to the hit window.
	public int HitWindowSampleWidth
	{
		get
		{
			return hitWindowRangeInSamples;
		}
	}

	// The Sample Rate specified by the Koreography.
	public int SampleTime
	{
		get
		{
			return playingKoreo.GetLatestSampleTime();
		}
	}

	// The Sample Rate specified by the Koreography.
	public int SampleRate
	{
		get
		{
			return playingKoreo.SampleRate;
		}
	}

	public int LeadInSamples = 0;

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}

	void Start()
	{
		// Initialize events.
		playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);

		// Grab all the events out of the Koreography.
		KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID);
		beatEvents = rhythmTrack.GetAllEvents();

		// Register for Koreography Events.  This sets up the callback.
		Koreographer.Instance.RegisterForEvents(eventID, CheckForLeadInNote);

		Init();
	}

	private void Init()
	{
		pendingEventIndex = 0;
		leadInEventIndex = pendingEventIndex;

		// check and set initial lead in note
		StartCoroutine(InitializeLeadInSample());
	}

	private void Update()
	{
		// This should be done in Start().  We do it here to allow for testing with Inspector modifications.
		UpdateInternalValues();

		// check for lead in timing
		CheckForLeadIn();

		// testing for OnNoteHit
		if (Input.GetKeyDown(KeyCode.Space) && IsNoteHittable())
		{
			Debug.Log("HIT!");
		}
	}

	#region Real Time Timings
	// Checks to see if the Note Object is currently hittable or not based on current audio sample
	//  position and the configured hit window width in samples (this window used during checks for both
	//  before/after the specific sample time of the Note Object).
	public bool IsNoteHittable()
	{
		int noteTime = pendingEvent.StartSample;
		int hitWindow = HitWindowSampleWidth;

		//debugging
		int diff = noteTime - SampleTime;
		if (Mathf.Abs(diff) >= hitWindow && diff > 0)
		{
			//Debug.Log("before: " + diff);
		}
		else if(Mathf.Abs(diff) >= hitWindow && diff < 0)
		{
			//Debug.Log("after: " + diff);
		}

		return (Mathf.Abs(noteTime - SampleTime) <= hitWindow);
	}

	void CheckForLeadInNote(KoreographyEvent _event)
	{
		UpdateNextEvent();
		//Debug.Log("Lead In");
	}

	// increment after event
	void UpdateNextEvent()
	{
		//pendingEventIndex++;
		StartCoroutine(IncrementAfterHitWindow());
	}

	// wait until after the hit range
	IEnumerator IncrementAfterHitWindow()
	{
		yield return new WaitForSeconds((0.001f * hitWindowRangeInMS) / 2f);
		pendingEventIndex++;
	}
	#endregion

	#region Lead In Stuff
	// lead in
	IEnumerator InitializeLeadInSample()
	{
		while (LeadInSamples == 0)
		{
			LeadInSamples = (int)playingKoreo.GetSamplesPerBeat(playingKoreo.GetLatestSampleTime()) * numLeadInBeats;
			yield return null;
		}
		SetLeadInEvent(leadInEventIndex);
		yield return 0;
	}

	private void CheckForLeadIn()
	{
		if (leadInEvent != null && playingKoreo.GetLatestSampleTime() >= leadInEvent.StartSample )
		{
			if (NoteManager.Instance.OnLeadInEvent != null)
				NoteManager.Instance.OnLeadInEvent.Invoke();
			IncrementLeadIn();
		}
	}

	private void IncrementLeadIn()
	{
		if (leadInEventIndex + 1 < beatEvents.Count)
		{
			leadInEventIndex++;
			SetLeadInEvent(leadInEventIndex);
		}
	}

	private void SetLeadInEvent(int _index)
	{
		leadInEvent = new KoreographyEvent();
		leadInEvent.StartSample = beatEvents[_index].StartSample - (LeadInSamples * numLeadInBeats);
		leadInEvent.EndSample = leadInEvent.StartSample;

		if (OnLeadIn != null) OnLeadIn.Invoke();
	}
	#endregion

	// Update any internal values that depend on externally accessible fields (public or Inspector-driven).
	void UpdateInternalValues()
	{
		hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);
	}
}
