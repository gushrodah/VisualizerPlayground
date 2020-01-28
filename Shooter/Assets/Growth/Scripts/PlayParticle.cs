using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticle : Actionable
{
	[SerializeField]
	private GameObject prefab;
	private LivingParticleController lp;

	public Transform attractor;

	public override void TriggerAction()
	{
		GameObject g = Instantiate(prefab, transform);
		lp = g.GetComponent<LivingParticleController>();
		lp.affector = attractor;

		//set destroy
		Destroy(g, g.GetComponent<ParticleSystem>().main.startLifetime.constantMax);
	}
}
