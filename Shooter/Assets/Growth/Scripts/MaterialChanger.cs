using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
	[SerializeField]
	private Material startMaterial, newMaterial;
	[SerializeField]
	private MeshRenderer meshRend;

	private float changeRate = .5f;

	private void Start()
	{
		startMaterial = meshRend.material;
	}

	private void OnEnable()
	{
		GetComponent<OnCollisionEvent>().OnCollision += ChangeMaterial;
	}

	private void ChangeMaterial()
	{
		meshRend.material = newMaterial;
		StartCoroutine(ChangeColorOverTime());
	}

	IEnumerator ChangeColorOverTime()
	{
		float maxEmission = 10;
		while (maxEmission > 0)
		{
			// todo : get materials value and set it lower

			// decrement
			maxEmission -= changeRate;

			yield return null;
		}
		yield return 0;
	}
}
