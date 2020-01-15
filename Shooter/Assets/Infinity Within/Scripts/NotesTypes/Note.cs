using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NoteState
{
	POS,NEG,NEUTRAL, DONE
}

// two states : neg  or pos 
[RequireComponent(typeof(ParticleSystem))]
public class Note : MonoBehaviour {

	#region Properties
	[Header("Attributes")]
	// TODO: change to auto update speed to get to hand on next note
	public float absorbSpeed;
	public NoteState state = NoteState.NEG;

	[Header("Visuals")]
	public ParticleSystem indicatorParts;
	// aborbed particles to hand
	public ParticleSystem NeutralParts, negParts, PosParts;

	[Space(10)]
	public GameObject absorbParts;
	// missed particles
	public GameObject missedPrefab;

	#region private 
	//collider
	private SphereCollider col;
	// controls all child particles
	private ParticleSystem parentPart;
	#endregion
	#endregion

	#region monos
	private void OnEnable()
	{
		MusicManager.OnLeadIn += Activate;
	}

	private void OnDisable()
	{
		MusicManager.OnLeadIn -= Activate;
	}

	void Start()
	{
		parentPart = GetComponent<ParticleSystem>();
		col = GetComponent<SphereCollider>();
	}
	#endregion

	#region public methods

	public virtual void ChangeState(NoteState _state)
	{
		if (_state == NoteState.NEUTRAL)
		{
			if (negParts != null)
			{
				negParts.Stop();
				PosParts.gameObject.SetActive(true);
			}
			PosParts.gameObject.SetActive(true);
			if (PosParts.isStopped) PosParts.Play();
			PosParts.Play();
			if (NeutralParts != null)
			{
				NeutralParts.Stop();
			}
		}
		else if (_state == NoteState.POS)
		{
			negParts.Stop();
			PosParts.Stop();
			NeutralParts.gameObject.SetActive(true);
			NeutralParts.Play();
		}
		else if (_state == NoteState.DONE)
		{
			negParts.Stop();
			PosParts.Stop();
			NeutralParts.Stop();
		}
	}

	/// <summary>
	/// checks to see if absorbed on beat
	/// </summary>
	/// <param name="_endPos"></param>
	public void CheckCanAbsorb(Transform _endPos)
	{
		if (MusicManager.Instance.IsNoteHittable())
		{
			Absorb(_endPos);
		}
		else
		{
			Debug.Log("missed");
			Miss();
		}
	}


	//start of indicator
	public void Activate()
	{
		if (state == NoteState.NEG)
			indicatorParts.Play();
	}
	#endregion

	#region private methods

	private void Absorb(Transform _endPos)
	{
		StartCoroutine(ToPos(_endPos));

		ChangeState(NoteState.NEUTRAL);

		// disable col so cant hit
		GetComponent<SphereCollider>().enabled = false;
		indicatorParts.gameObject.SetActive(false);
		if (negParts != null)
		{
			negParts.gameObject.SetActive(false);
		}
	}

	private void Miss()
	{
		GameObject g = Instantiate(missedPrefab, transform.position, Quaternion.identity);
		StartCoroutine(DespawnNote(3, g));
	}

	private IEnumerator DespawnNote(int _duration, GameObject _g)
	{
		yield return new WaitForSeconds(_duration);
		Destroy(_g);
	}

	//TOD
	// move transfer particle to hand
	//change to Coroutine 
	private IEnumerator ToPos(Transform _endPos)
	{
		// instantiate neutral particle
		GameObject g = Instantiate(absorbParts, transform.position, Quaternion.identity);
		//TODO: change speed to hit exactly on next beat
		while (Vector3.Distance(_endPos.position, g.transform.position) > .1f)
		{
			Vector3 dir = _endPos.position - g.transform.position;
			dir.Normalize();                                    // normalization is obligatory
			g.transform.position += dir * absorbSpeed * Time.deltaTime; // using deltaTime and speed is obligatory

			yield return null;
		}
		Destroy(g);
		yield return 0;
	}

	#endregion
}
