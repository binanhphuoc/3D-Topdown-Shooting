using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject{
	public abstract class AidUISignal : MonoBehaviour {

		//protected GameObject player;
		private AidDisplayBar displayBar;
		//private Sprite image;
		public float timeDestroy;
		public float flashTime = 5f;
		public float timeEffect;
		public Color flashColor;
		private bool isEntered = false;

		/*
		public delegate void Delegate();
		protected Delegate triggerEnter;
		protected Delegate disableEffect;
		*/

		public virtual void triggerEnter(){}
		public virtual void disableEffect(){}
		public virtual void onAwake(){}

		//These methods are required
		public abstract void updateTimer();				// Update timer for each type of aid
		public abstract float sendTimer();
		public abstract AidEnum sendType ();

		// Use this for initialization
		void Awake() {
			//displayBar = GameObject.FindGameObjectWithTag ("AidTimer").GetComponent<AidDisplayBar>();

			//Debug.Log (displayBar);
			//player = GameObject.FindGameObjectWithTag ("Player");

			//This line is the problem
			//image = gameObject.GetComponent<Image> ().sprite;
			onAwake ();
		}


		void Update(){
			if (!isEntered) {
				if (timeDestroy <= 0)
					Destroy (gameObject);
				else
					timeDestroy -= Time.deltaTime;
			}
			else {
				if (sendTimer () <= 0) {
					// Do something with the character
					displayBar.disableTimer(sendType());
					disableEffect ();
					Destroy (gameObject);
				} else {
					updateTimer ();
					displayBar.setTimer (sendTimer () / timeEffect, sendType ());
				}
			}
		}

		protected GameObject player;

		// Update is called once per frame
		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player")) {

				player = other.gameObject;

				// Do something with the charater
				displayBar = GameObject.FindGameObjectWithTag ("AidTimer").GetComponent<AidDisplayBar>();
				player.GetComponent<PlayerHealth> ().flash (flashColor);
				triggerEnter();

				//player.GetComponent<PlayerMovement>().setInverse (true);
				//player.GetComponent<PlayerHealth> ().inverse ();
				//displayBar.setImage (image, sendType());
				isEntered = true;
				if (gameObject.GetComponent<MeshRenderer> ()) {
					gameObject.GetComponent<MeshRenderer> ().enabled = false;
				}
				gameObject.GetComponent<CapsuleCollider> ().enabled = false;
				for (int i = 0; i < gameObject.transform.childCount; i++) {
					gameObject.transform.GetChild (i).gameObject.SetActive (false);
				}
			}
		}
	}
}