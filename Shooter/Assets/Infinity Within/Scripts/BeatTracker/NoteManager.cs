using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SonicBloom.Koreo;

// keeps track of all board objects and determines which Neg will be the next the activate
public class NoteManager : MonoBehaviour {

	private static NoteManager _instance;

	public static NoteManager Instance
	{
		get { return _instance; }
		set { _instance = value; }
	} 

	//indicator stuff
	public float indicatorSpeed;

	public List<NoteCluster> ClusterList;

	// onBeat event, choose note in list to activate
	public Action<Transform> OnBeatHit, OnBeatMiss;
	// lead in managed by musicManager
	public Action OnLeadInEvent;
	public Action OnClusterComplete;
	// -sets Notes.OnAbsorbed

	public GameObject indicator;
	private Coroutine indicatorCor;
	private float beatPadding = .25f;
	private float beatTimer = 0f;
	private NoteCluster currentCluster;
	private int clusterIndex = 0;

	// koreograph stuff
	[EventID]
	public string eventID;
	Koreography playingKoreo;

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

	private void OnEnable()
	{
		// sub to events
		OnBeatHit += SuccessfulBeatHit;
		OnClusterComplete += ChangeClusterCluster;
		//MusicManager.OnLeadIn += MoveIndicator;
	}

	private void OnDisable()
	{

		// sub to events
		OnBeatHit -= SuccessfulBeatHit;
		OnClusterComplete -= ChangeClusterCluster;
		//MusicManager.OnLeadIn -= MoveIndicator;
	}

	void Start()
	{
		// Register for Koreography Events.  This sets up the callback.
		//Koreographer.Instance.RegisterForEvents(eventID, ActivateCurrentNote);

		playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
		
		// set current cluster
		currentCluster = ClusterList[clusterIndex];
		currentCluster.ActivateCluster();

		// move indicator to start
		//StartCoroutine(MoveIndicator(currentCluster.NextNotePosition));
	}

	void OnDestroy()
	{
		// Sometimes the Koreographer Instance gets cleaned up before hand.
		//  No need to worry in that case.
		if (Koreographer.Instance != null)
		{
			Koreographer.Instance.UnregisterForAllEvents(this);
		}
	}

	void MoveIndicator()
	{
		if (indicatorCor != null) StopCoroutine(indicatorCor);
		StartCoroutine(MoveIndicatorRoutine(currentCluster.NextNotePosition));
	}

	private IEnumerator MoveIndicatorRoutine(Transform _endPos)
	{
		//TODO: change speed to hit exactly on next beat
		while (Vector3.Distance(_endPos.position, indicator.transform.position) > .1f)
		{
			Vector3 dir = _endPos.position - indicator.transform.position;
			dir.Normalize();                                    // normalization is obligatory
			indicator.transform.position += dir * indicatorSpeed * Time.deltaTime; // using deltaTime and speed is obligatory

			yield return null;
		}
		yield return 0;
	}

	private void SuccessfulBeatHit(Transform _transform)
	{
		if (indicatorCor != null) StopCoroutine(indicatorCor);
		//indicatorCor = StartCoroutine(MoveIndicator(currentCluster.NextNotePosition));
	}

	private void ChangeClusterCluster()
	{
		currentCluster.DeactivateCluster();
		if (clusterIndex < ClusterList.Count-1)
		{
			//increment
			clusterIndex++;
			currentCluster = ClusterList[clusterIndex];
			currentCluster.ActivateCluster();
		}
	}
}
