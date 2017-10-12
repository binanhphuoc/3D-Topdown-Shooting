using UnityEngine;
using System.Collections.Generic;

namespace CompleteProject
{
	public class AidManager : MonoBehaviour
	{
		public PlayerHealth playerHealth;       // Reference to the player's heatlh.
		public GameObject spawnObject;                // The enemy prefab to be spawned.
		public float spawnTime = 3f;            // How long between each spawn.
		Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

		public GameObject spawnSystem;
		public int pointCount;

		void Start ()
		{
			spawnPoints = new Transform [pointCount];
			
			for (int runs = 0; runs < pointCount; runs++)
			{
				spawnPoints[runs] = spawnSystem.transform.GetChild (runs).GetComponent<Transform> ();
			}
					 
			// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
			InvokeRepeating ("Spawn", spawnTime, spawnTime);
		}


		void Spawn ()
		{
			// If the player has no health left...
			if(playerHealth.currentHealth <= 0f)
			{
				// ... exit the function.
				return;
			}

			// Find a random index between zero and one less than the number of spawn points.
			//int spawnPointIndex = Random.Range (0, pointCount);
			int spawnPointIndex = Random.Range(1, 3);



			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			//Instantiate (spawnObject, spawnPoints[spawnPointIndex].position, spawnObject.transform.rotation);
			GameObject go = ObjectPooler.SharedInstance.GetPooledObject(spawnObject.GetComponent<PoolTag>().type, spawnPoints[spawnPointIndex].position, spawnObject.transform.rotation);
			ObjectPooler.SharedInstance.setActive(go, true);
		}
	}
}