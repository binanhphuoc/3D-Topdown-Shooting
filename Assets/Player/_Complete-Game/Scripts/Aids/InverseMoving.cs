using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject{

	public class InverseMoving : AidUISignal {

		//private GameObject player;


		override public void onAwake()
		{
			// Write your code here
		}

		// Called when a player entering the trigger
		// Variable 'player' (GameObject) refers to the player entering the trigger
		override public void triggerEnter()
		{
			// Do not remove this line
			timer = timeEffect;
			if (current != null && current != this.gameObject)
				ObjectPooler.SharedInstance.destroyObject(current);
			current = this.gameObject;

			// Write your code here
			player.GetComponent<PlayerMovement>().setInverse (true);
		}

		// Called when timer is out
		// Variable 'player' (GameObject) refers to the player entering the trigger
		override public void disableEffect()
		{
			// Write your code here
			player.GetComponent<PlayerMovement>().setInverse (false);
		}

		// Boilerplate section
		// Do NOT modify

		// Please specify the type here
		public AidEnum type = AidEnum.PoisonousMushroom;

		static float timer;
		static GameObject current;
		override public void updateTimer()
		{
			timer -= Time.deltaTime;
			//Debug.Log (timer);
		}

		override public float sendTimer()
		{
			return timer;
		}

		override public AidEnum sendType()
		{
			return type;
		}

	}

}