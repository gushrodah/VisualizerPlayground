//----------------------------------------------
//            	   Koreographer                 
//    Copyright © 2014-2019 Sonic Bloom, LLC    
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace SonicBloom.Koreo.Demos
{
	public class LeadInController : MonoBehaviour
	{
		#region Fields

		[Tooltip("The Event ID of the track to use for target generation.")]
		[EventID]
		public string eventID;

		[Tooltip("The number of milliseconds (both early and late) within which input will be detected as a Hit.")]
		[Range(8f, 150f)]
		public float hitWindowRangeInMS = 80;

		[Tooltip("The amount of time in seconds to provide before playback of the audio begins.  Changes to this value are not immediately handled during the lead-in phase while playing in the Editor.")]
		public float leadInTime;

		[Tooltip("The Audio Source through which the Koreographed audio will be played.  Be sure to disable 'Auto Play On Awake' in the Music Player.")]
		public AudioSource audioCom;

		// The list that will contain all events in this lane.  These are added by the Rhythm Game Controller.
		List<KoreographyEvent> laneEvents = new List<KoreographyEvent>();

		// Index of the next event to check for spawn timing in this lane.
		int pendingEventIdx = 0;

		// The amount of leadInTime left before the audio is audible.
		float leadInTimeLeft;

		// The amount of time left before we should play the audio (handles Event Delay).
		float timeLeftToPlay;

		// Local cache of the Koreography loaded into the Koreographer component.
		Koreography playingKoreo;

		// Koreographer works in samples.  Convert the user-facing values into sample-time.  This will simplify
		//  calculations throughout.
		int hitWindowRangeInSamples;	// The sample range within which a viable event may be hit.

		#endregion
		#region Properties

		// Public access to the hit window.
		public int HitWindowSampleWidth
		{
			get
			{
				return hitWindowRangeInSamples;
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

		// The current sample time, including any necessary delays.
		public int DelayedSampleTime
		{
			get
			{
				// Offset the time reported by Koreographer by a possible leadInTime amount.
				return playingKoreo.GetLatestSampleTime() - (int)(audioCom.pitch * leadInTimeLeft * SampleRate);
			}
		}

		#endregion
		#region Methods

		void Start()
		{
			InitializeLeadIn();

			// Initialize events.
			playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);

			// Grab all the events out of the Koreography.
			KoreographyTrackBase rhythmTrack = playingKoreo.GetTrackByID(eventID);
			laneEvents = rhythmTrack.GetAllEvents();
		}

		// Sets up the lead-in-time.  Begins audio playback immediately if the specified lead-in-time is zero.
		void InitializeLeadIn()
		{
			// Initialize the lead-in-time only if one is specified.
			if (leadInTime > 0f)
			{
				// Set us up to delay the beginning of playback.
				leadInTimeLeft = leadInTime;
				timeLeftToPlay = leadInTime - Koreographer.Instance.EventDelayInSeconds;
			}
			else
			{
				// Play immediately and handle offsetting into the song.  Negative zero is the same as
				//  zero so this is not an issue.
				audioCom.time = -leadInTime;
				audioCom.Play();
			}
		}

		void Update()
		{
			// This should be done in Start().  We do it here to allow for testing with Inspector modifications.
			UpdateInternalValues();

			// Count down some of our lead-in-time.
			if (leadInTimeLeft > 0f)
			{
				leadInTimeLeft = Mathf.Max(leadInTimeLeft - Time.unscaledDeltaTime, 0f);
			}

			// Count down the time left to play, if necessary.
			if (timeLeftToPlay > 0f)
			{
				timeLeftToPlay -= Time.unscaledDeltaTime;

				// Check if it is time to begin playback.
				if (timeLeftToPlay <= 0f)
				{
					audioCom.time = -timeLeftToPlay;
					audioCom.Play();

					timeLeftToPlay = 0f;
				}
			}

			// Check for new spawns.
			CheckSpawnNext();

			// Check for input.  Note that touch controls are handled by the Event System, which is all
			//  configured within the Inspector on the buttons themselves, using the same functions as
			//  what is found here.  Touch input does not have a built-in concept of "Held", so it is not
			//  currently supported.
			if (Input.GetKeyDown(KeyCode.Space))
			{
				CheckNoteHit();
			}
		}

		#region Timings
		// Checks if a Note Object is hit.  If one is, it will perform the Hit and remove the object
		//  from the trackedNotes Queue.
		public void CheckNoteHit()
		{
			// Always check only the first event as we clear out missed entries before.
			if (IsNoteHittable())
			{
				//SUCCESSFUL NOTE HIT!
			}
		}

		// Checks if the next Note Object should be spawned.  If so, it will spawn the Note Object and
		//  add it to the trackedNotes Queue.
		void CheckSpawnNext()
		{
			// TODO : change GetSpawnSampleOffset
			int samplesToTarget = GetSpawnSampleOffset();

			int currentTime = DelayedSampleTime;

			// Spawn for all events within range.
			while (pendingEventIdx < laneEvents.Count &&
				   laneEvents[pendingEventIdx].StartSample < currentTime + samplesToTarget)
			{
				KoreographyEvent evt = laneEvents[pendingEventIdx];

				//LEAD IN SPAWN!

				pendingEventIdx++;
			}
		}

		// Checks to see if the Note Object is currently hittable or not based on current audio sample
		//  position and the configured hit window width in samples (this window used during checks for both
		//  before/after the specific sample time of the Note Object).
		public bool IsNoteHittable()
		{
			int noteTime = laneEvents[pendingEventIdx].StartSample;
			int curTime = DelayedSampleTime;
			int hitWindow = HitWindowSampleWidth;

			return (Mathf.Abs(noteTime - curTime) <= hitWindow);
		}

		// Checks to see if the note is no longer hittable based on the configured hit window width in
		//  samples.
		public bool IsNoteMissed()
		{
			bool bMissed = true;

			if (enabled)
			{
				int noteTime = laneEvents[pendingEventIdx].StartSample;
				int curTime = DelayedSampleTime;
				int hitWindow = HitWindowSampleWidth;

				bMissed = (curTime - noteTime > hitWindow);
			}

			return bMissed;
		}
		#endregion

		// Uses the Target position and the current Note Object speed to determine the audio sample
		//  "position" of the spawn location.  This value is relative to the audio sample position at
		//  the Target position (the "now" time).
		int GetSpawnSampleOffset()
		{
			// Given the current speed, what is the sample offset of our current.
			//float spawnDistToTarget = spawnY - transform.position.y;

			// At the current speed, what is the time to the location?
			//double spawnSecsToTarget = (double)spawnDistToTarget / (double)gameController.noteSpeed;

			// Figure out the samples to the target.
			//return (int)(spawnSecsToTarget * SampleRate);
			return 0;
		}

		// Update any internal values that depend on externally accessible fields (public or Inspector-driven).
		void UpdateInternalValues()
		{
			hitWindowRangeInSamples = (int)(0.001f * hitWindowRangeInMS * SampleRate);
		}

		// Restarts the game, causing all Lanes and any active Note Objects to reset or otherwise clear.
		public void Restart()
		{
			// Reset the audio.
			audioCom.Stop();
			audioCom.time = 0f;

			// Flush the queue of delayed event updates.  This effectively resets the Koreography and ensures that
			//  delayed events that haven't been sent yet do not continue to be sent.
			Koreographer.Instance.FlushDelayQueue(playingKoreo);

			// Reset the Koreography time.  This is usually handled by loading the Koreography.  As we're simply
			//  restarting, we need to handle this ourselves.
			playingKoreo.ResetTimings();

			// Reinitialize the lead-in-timing.
			InitializeLeadIn();
		}

		#endregion
	}
}
