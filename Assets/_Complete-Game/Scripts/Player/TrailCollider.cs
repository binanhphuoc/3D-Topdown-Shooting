﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
	public class TrailCollider : MonoBehaviour {

		public GameObject thisMoster;
		TrailRenderer thisMonsterTrail;
		bool isInTrigger = false;
		public GameObject particles;

		// Use this for initialization
		void Start () {
			thisMonsterTrail = thisMoster.GetComponentInChildren<TrailRenderer> ();
			//timer = thisMonsterTrail.time;
			ObjectPooler.SharedInstance.destroyObject (gameObject, thisMonsterTrail.time - 0.2f);
			/*
			var main = gameObject.transform.GetChild (1).GetComponent<ParticleSystem> ().main;
			main.startColor = thisMonsterTrail.startColor;*/
		}
		
		// Update is called once per frame
		void Update () {
		}

		void OnTriggerStay(Collider other)
		{
			//Debug.Log (gameObject.activeInHierarchy);
			if (other.CompareTag ("Player") && gameObject.activeInHierarchy) {
				EnemyAttack ea = thisMoster.GetComponent<EnemyAttack> ();
				EnemyHealth eh = thisMoster.GetComponent<EnemyHealth> ();
				if (ea.getTimer() >= ea.timeBetweenAttacks && eh.currentHealth > 0) {
					// ... attack.
					//Debug.Log(trailCount);
					ea.Attack ();
					GameObject o = Instantiate (particles);
					o.transform.Translate (gameObject.transform.position - o.transform.position);
					var main = o.transform.GetChild (1).GetComponent<ParticleSystem> ().main;
					main.startColor = thisMonsterTrail.startColor;
					o.transform.GetChild (0).GetComponent<ParticleSystem> ().Play();
					o.transform.GetChild (1).GetComponent<ParticleSystem> ().Play();
					Destroy (o, 1f);
				}
			}
		
		}

		public bool inTrigger()
		{
			return isInTrigger;
		}
	}

}