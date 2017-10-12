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

		float timer;

		void Awake()
		{
			player = GameObject.FindGameObjectWithTag ("Player");
			playerHealth = player.GetComponent <PlayerHealth> ();
/*			renderer = GetComponent<MeshRenderer> ();
			col = GetComponent<Collider> ();*/
		}

		void OnEnable()
		{
			timer = existTime;
		}

		void Update()
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
				ObjectPooler.SharedInstance.destroyObject(gameObject);
		}

		void OnTriggerEnter(Collider other)
		{
			if (!gameObject.activeInHierarchy)
				return;
			if (playerHealth.currentHealth >= playerHealth.startingHealth)
				return;
			if (other.gameObject == player) {
				playerHealth.recover (healthGain);
			/*	renderer.enabled = false;
				col.enabled = false;*/
				ObjectPooler.SharedInstance.destroyObject (gameObject);
			}
		}

	}
}