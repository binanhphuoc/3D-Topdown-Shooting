using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
	public class Animation : MonoBehaviour {

		public float angleSpeed;
		public float updownSpeed;
		public float initialHeight = -0.5f;
		public float finalHeight = -0.1f;
		bool direction = true;			// up = true, down = false


		void Awake()
		{
			updownSpeed /= 10;
			//Debug.Log (transform.position);
		}

		void Update()
		{
			transform.Rotate(Vector3.up * Time.deltaTime * angleSpeed, Space.World);
			if (direction == true) {
				transform.Translate(Vector3.up * Time.deltaTime * updownSpeed, Space.World);
				if (transform.position.y >= finalHeight)
					direction = !direction;

			} else {
				transform.Translate(Vector3.down * Time.deltaTime * updownSpeed, Space.World);
				if (transform.position.y <= initialHeight)
					direction = !direction;
			}
		}
	}
}