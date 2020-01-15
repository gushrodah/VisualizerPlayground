using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class NoteCluster : MonoBehaviour {

	// range to spawn notes
	public Vector3 maxRange;
	// set at beginning for auto populating env with notes
	public int noteCount;
	// TODO: make private - manually reacting notes for now
	public List<Note> noteList;
	public GameObject notePrefab;

	// spawn prefab after done or spawned prefab on individual note
	public bool spawnAfterCluster;
	// finished prefab to spawn when cluster is finished
	public GameObject completedPrefab;

	public NoteManager manager;
	
	public int index = 0;

	public bool IsComplete { get { return (index == -1 || index >= noteList.Count);}}

	public Transform NextNotePosition { get { return noteList[index].transform; } }

	private void Start()
	{

		Init();
	}

	// instantiate and add all notes to list
	private void Init()
	{
		noteList = new List<Note>();
		for (int i = 0; i < noteCount; i++)
		{
			// TODO: set note to random position
			Vector3 newPos = new Vector3(UnityEngine.Random.Range(-maxRange.x, maxRange.x), UnityEngine.Random.Range(-maxRange.y, maxRange.y), UnityEngine.Random.Range(-maxRange.z, maxRange.z));
			newPos += transform.position;
			GameObject tempNote = Instantiate(notePrefab, newPos, Quaternion.identity, transform);
			Note note = tempNote.GetComponent<Note>();
			noteList.Add(note);
		}
	}

	// sub to events
	public void ActivateCluster()
	{
		// set events
		manager.OnBeatHit += SuccessfulBeatHit;
		manager.OnLeadInEvent += ActivateCurrentNote;
	}
	// unsub events
	public void DeactivateCluster()
	{
		// set events
		manager.OnBeatHit -= SuccessfulBeatHit;
		manager.OnLeadInEvent -= ActivateCurrentNote;
	}

	private void SuccessfulBeatHit(Transform _transform)
	{
		if (noteList.Count >= 0)
		{
			// set note to launch at hand
			noteList[index].CheckCanAbsorb(_transform);

			// remove that note from list
			//noteList.RemoveAt(index);

			Increment();
		}
	}

	// plays the indicator
	private void ActivateCurrentNote()
	{
		noteList[index].Activate();
	}

	// increment current note index
	private void Increment()
	{
		index++;
		if (index >= noteList.Count)
		{
			index = -1;

			// dont disbale notes if !spawnAfterCluster
			if (spawnAfterCluster)
			{
				GameObject g = Instantiate(completedPrefab, transform.position, Quaternion.identity);
				ParticleSystem ps = g.GetComponent<ParticleSystem>();
				if (ps != null) { ps.Play(); }
				DisableAllNotes();
			}
			// cluster complete
			manager.OnClusterComplete.Invoke();
		}
	}

	private void DisableAllNotes()
	{
		foreach (var note in noteList)
		{
			note.ChangeState(NoteState.DONE);
		}
	}
}
