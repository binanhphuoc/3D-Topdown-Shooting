using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
	public class HealthRecover : MonoBehaviour {

		public int healthGain;
		public float existTime;

		GameObject player;

	/*	MeshRenderer renderer;
		Collider col;*/

		PlayerHealth playerHealth;

		void Awake()
		{
			player = GameObject.FindGameObjectWithTag ("Player");
			playerHealth = player.GetComponent <PlayerHealth> ();
/*			renderer = GetComponent<MeshRenderer> ();
			col = GetComponent<Collider> ();*/
			Destroy (gameObject, existTime);
		}

		void Update()
		{
			
		}

		void OnTriggerEnter(Collider other)
		{
			if (playerHealth.currentHealth >= playerHealth.startingHealth)
				return;
			if (other.gameObject == player) {
				playerHealth.recover (healthGain);
			/*	renderer.enabled = false;
				col.enabled = false;*/
				Destroy (gameObject);
			}
		}

	}
}