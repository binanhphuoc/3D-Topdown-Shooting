using UnityEngine;

namespace CompleteProject
{
    public class EnemyManager : MonoBehaviour
    {
        public PlayerHealth playerHealth;       // Reference to the player's heatlh.
		//public GameObject enemy;                // The enemy prefab to be spawned.
		public poolEntity entity;
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
		float timer;

        void Start ()
        {
			// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
			InvokeRepeating ("Spawn", spawnTime, spawnTime);
			//timer = spawnTime;
        }

		/*
		void Update()
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				Spawn();
				timer = spawnTime;
			}
		}
		*/

        void Spawn ()
        {
            // If the player has no health left...
            if(playerHealth.currentHealth <= 0f)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            //Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
			GameObject go = ObjectPooler.SharedInstance.GetPooledObject(entity, 
			                                                            spawnPoints[spawnPointIndex].position, 
			                                                            spawnPoints[spawnPointIndex].rotation);
			if (go == null)
				return;
			ObjectPooler.SharedInstance.setActive(go, true);

			/*
			GameObject obj = ObjectPooler.SharedInstance.GetPooledObject(poolEntity.MONSTER);
			Vector3 v = spawnPoints [spawnPointIndex].position;
			obj.transform.position = v;
			Quaternion q = spawnPoints [spawnPointIndex].rotation;
			obj.transform.rotation = q;
			*/
        }
    }
}