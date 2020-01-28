using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullEvent : MonoBehaviour
{
	public float pullPower;

	[SerializeField]
	private GameObject target;
	private Rigidbody rb;
		
    void Start()
    {
		if(target.GetComponent<Rigidbody>() != null)
			rb = target.GetComponent<Rigidbody>();
    }

	// pull target to this transform
	public void Pull()
	{
		var heading = target.transform.position - transform.position;
		var distance = heading.magnitude;
		var direction = heading / distance;	// normalized
		//target.transform.position
		rb.AddForce(direction * pullPower);
	}
}
