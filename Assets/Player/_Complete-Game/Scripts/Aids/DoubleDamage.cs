using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
	public class DoubleDamage : AidUISignal {

		public int multiplier;

		override public void onAwake()
		{
			// Write your code here
		}
		
		// Called when a player entering the trigger
		// Variable 'player' (GameObject) refers to the player entering the trigger
		override public void triggerEnter()
		{
			// Boilerplate
			// Do not remove this line
			timer = timeEffect;
			if (current != null && current != this.gameObject)
				ObjectPooler.SharedInstance.destroyObject(current);
			current = this.gameObject;

			// Your code goes here
			PlayerShooting ps = player.transform.GetChild (2).gameObject.GetComponent<PlayerShooting> ();
			// Write your code here
			if (ps.damagePerShot == ps.getBasicDamage ()) {
				ps.damagePerShot *= multiplier;
			}
			ps.setOnPower (true);
			ps.setGunLineWidth(15f);
		}

		// Called when timer is out
		// Variable 'player' (GameObject) refers to the player entering the trigger
		override public void disableEffect()
		{
			PlayerShooting ps = player.transform.GetChild (2).gameObject.GetComponent<PlayerShooting> ();

			ps.setOnPower (false);

			// Write your code here
			int basicDam = ps.getBasicDamage ();
			ps.setGunLineWidth(1f);

			if (ps.damagePerShot != basicDam)
				ps.damagePerShot = basicDam;
			//Debug.Log ("disable");
		}
			
		// Required variables and methods
		// These following methods is called once after each frame

		public AidEnum type = AidEnum.SuperBullet;

		// Boilerplate section
		// Do NOT modify
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