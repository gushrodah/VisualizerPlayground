using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : Actionable
{
	//[SerializeField]
	//private Material newMaterial;

	private Material newMaterial;

	private Material curMaterial
	{
		get { return GetComponent<MeshRenderer>().material; }
		set { GetComponent<MeshRenderer>().material = value; }
	}
	private MeshRenderer meshRend;

	private const float maxEmission = 10;
	private float changeRate = .08f;

	private void Start()
	{
		if (GetComponent<MeshRenderer>() != null)
		{
			newMaterial = new Material(curMaterial);
		}
		// get rid of script if no mesh renderer
		else
		{
			Debug.Log(name + " has no meshrend: DESTROYING");
			Destroy(this);
		}
	}

	public override void TriggerAction()
	{
		// set values
		curMaterial = newMaterial;
		curMaterial.SetColor("_EmissionColor", Color.green * 5);
		curMaterial.EnableKeyword("_EMISSION");

		Debug.Log("start");
		StartCoroutine(ChangeColorOverTime());
	}

	IEnumerator ChangeColorOverTime()
	{
		float curEmission = maxEmission;
		while (curEmission > 0)
		{
			curMaterial.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.green, curEmission / maxEmission));

			// decrement
			curEmission -= changeRate;

			yield return null;
		}
		Debug.Log("end");
		yield return 0;
	}
}
