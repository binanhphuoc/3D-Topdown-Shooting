using UnityEngine;
using System.Collections;
//using DarkTonic.PoolBoss;

namespace CompleteProject
{
    public class EnemyAttack : MonoBehaviour
    {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.


        Animator anim;                              // Reference to the animator component.
        GameObject player;                          // Reference to the player GameObject.
        PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.
		//int trailCount = 0;							// Current number of trail colliders in contact with player
		GameObject trailOrigin;
		public GameObject originPrefab;
		public Transform trailPrefab;				// Prefab to spawn trail

        void Awake ()
        {
            // Setting up the references.
            player = GameObject.FindGameObjectWithTag ("Player");
            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent<EnemyHealth>();
            anim = GetComponent <Animator> ();

        }

		void OnEnable()
		{
			trailOrigin = ObjectPooler.SharedInstance.GetPooledObject(poolEntity.ORIGIN);
			Debug.Log(trailOrigin);
			ObjectPooler.SharedInstance.setActive(trailOrigin, true);
            InvokeRepeating("trailSpawn", 0.5f, 0.1f);
		}

		void OnDisable()
		{
			CancelInvoke();
		}

		void trailSpawn()
		{
			//Transform newTrail = PoolBoss.Spawn(trailPrefab, gameObject.transform.position, gameObject.transform.rotation, trailOrigin.transform);
			GameObject newTrail = ObjectPooler.SharedInstance.GetPooledObject(poolEntity.TRAIL,
																			  gameObject.transform.position,
			                                                                  gameObject.transform.rotation, trailOrigin);
			newTrail.gameObject.GetComponent<TrailCollider> ().thisMoster = gameObject;
			ObjectPooler.SharedInstance.setActive(newTrail, true);
		}

        void OnTriggerEnter (Collider other)
        {
            // If the entering collider is the player...
            if(other.gameObject == player)
            {
                // ... the player is in range.
                playerInRange = true;
            }
        }


        void OnTriggerExit (Collider other)
        {
            // If the exiting collider is the player...
            if(other.gameObject == player)
            {
                // ... the player is no longer in range.
                playerInRange = false;
            }
        }


		void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...
			if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0)
            {
                // ... attack.
				//Debug.Log(trailCount);
                Attack ();
            }

            // If the player has zero or less health...
            if(playerHealth.currentHealth <= 0)
            {
                // ... tell the animator the player is dead.
                anim.SetTrigger ("PlayerDead");
            }
        }


        public void Attack ()
        {
            // Reset the timer.
            timer = 0f;

            // If the player has health to lose...
            if(playerHealth.currentHealth > 0)
            {
                // ... damage the player.
                playerHealth.TakeDamage (attackDamage);
				//playerHealth.TakeDamage (0);
            }
        }

	/*	public void changeTrailCount(int delta)
		{
			trailCount += delta;
		}*/

		public void destroyOrigin()
		{
			CancelInvoke ();
			//Debug.Log (trailOrigin.transform.childCount);
			//Debug.Log(trailOrigin.transform.childCount);
			//Debug.Log(trailOrigin);
			ObjectPooler.SharedInstance.destroyChildren(trailOrigin);
			//trailOrigin.transform.DetachChildren();
			//Debug.Log(trailOrigin.transform.childCount);
			ObjectPooler.SharedInstance.destroyObject (trailOrigin);
		}

		public float getTimer()
		{
			return timer;
		}

    }
}