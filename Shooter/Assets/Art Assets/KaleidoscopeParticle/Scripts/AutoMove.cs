// -----------------------------------------------------------------------
//  <copyright file="AutoMove.cs" company="DAIKI">
//      Copyright (c) DAIKI. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace KaleidoscopeParticle
{
    using UnityEngine;
    using System.Collections;

    public class AutoMove : MonoBehaviour
    {
        public Vector3 moveSpeed;
        public Vector3 rotSpeed;
        public float lessSpeed = 0.0f;

		public bool randomize;
		public Vector3 rotMin;
		public Vector3 rotMax;

		private void Start()
		{
			if (randomize)
			{
				rotSpeed = new Vector3(Random.Range(rotMin.x, rotMax.x), Random.Range(rotMin.y, rotMax.y), Random.Range(rotMin.z, rotMax.z));
			}
		}

		// Update is called once per frame
		void Update()
        {
            transform.localPosition += moveSpeed * Time.deltaTime;
            transform.localEulerAngles += rotSpeed * Time.deltaTime;

            moveSpeed -= moveSpeed * lessSpeed * Time.deltaTime;
            rotSpeed -= rotSpeed * lessSpeed * Time.deltaTime;
            //moveSpeed *= (1f-lessSpeed);
            //rotSpeed *= (1f-lessSpeed);
        }
    }
}
