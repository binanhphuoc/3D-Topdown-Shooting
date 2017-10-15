#define MOBILE_INPUT

using UnityEngine;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;
using UnityEngine.EventSystems;

namespace CompleteProject
{
	
    public class PlayerShooting : MonoBehaviour
    {
		public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.
		private int basicDamage;
		private bool onPower = false;

        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
		public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

		public Image joystick;
		public JoystickController ac;					// Aim and Shoot Controller

        void Awake ()
        {
			basicDamage = damagePerShot;

			// Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
			//faceLight = GetComponentInChildren<Light> ();
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
				//Debug.Log(Input.mousePosition);
                // ... shoot the gun.
                Shoot ();
            }

#else

			if(ac.isActive() && timer >= timeBetweenBullets && Time.timeScale != 0)
			{
			//Debug.Log(Input.mousePosition);
			// ... shoot the gun.
				Shoot ();
			}

            // If there is input on the shoot direction stick and it's time to fire...
        /*  if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                Shoot();
            }*/


#endif
			/*
			int nbTouches = Input.touchCount;
			Debug.Log (nbTouches);
			if(nbTouches > 0)
			{
				for (int i = 0; i < nbTouches; i++)
				{
					Touch touch = Input.GetTouch(i);

					TouchPhase phase = touch.phase;

					switch(phase)
					{
					case TouchPhase.Began:
						if (timer >= timeBetweenBullets && Time.timeScale != 0 && touch.position.x > Screen.width/2) {
							Shoot ();
						}
						Debug.Log (touch.position);
						break;
					case TouchPhase.Moved:
						if (timer >= timeBetweenBullets && Time.timeScale != 0 && touch.position.x > Screen.width/2) {
							Shoot ();
						}
						break;
					case TouchPhase.Stationary:
						if (timer >= timeBetweenBullets && Time.timeScale != 0 && touch.position.x > Screen.width/2) {
							Shoot ();
						}
						break;
					case TouchPhase.Ended:
						print("Touch index " + touch.fingerId + " ended at position " + touch.position);
						break;
					case TouchPhase.Canceled:
						print("Touch index " + touch.fingerId + " cancelled");
						break;

					}
				}
			}
		*/
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }

		private bool clickOnJoystick(Vector3 input)
		{
			RectTransform rt = joystick.rectTransform;
			if (input.x < rt.position.x || input.x > rt.position.x + rt.rect.width) {
				return false;
			}
			if (input.y < rt.position.y || input.y > rt.position.y + rt.rect.height) {
				return false;
			}
			return true;
		}

        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
		{
			// Reset the timer.
			timer = 0f;

			// Play the gun shot audioclip.
			gunAudio.Play ();

			// Enable the lights.
			gunLight.enabled = true;
			faceLight.enabled = true;

			// Stop the particles from playing if they were, then start the particles.
			gunParticles.Stop ();
			gunParticles.Play ();

			// Enable the line renderer and set it's first position to be the end of the gun.
			gunLine.enabled = true;
			gunLine.SetPosition (0, transform.position);

			// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
			shootRay.origin = transform.position;
			shootRay.direction = transform.forward;

			Vector3 finalPoint = new Vector3 ();
			finalPoint.Set (0, 0, 0);

			float currentRange = range;


			if (onPower) {
				// Perform the raycast against gameobjects on the shootable layer and if it hits something...
				while ((range > 0) && (Physics.SphereCast (shootRay.origin, 1f, shootRay.direction, out shootHit, currentRange, shootableMask))) {	            
					//if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
					// Try and find an EnemyHealth script on the gameobject hit.
					EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();

					// If the EnemyHealth component exist...
					if (enemyHealth != null) {
						// ... the enemy should take damage.
						enemyHealth.TakeDamage (damagePerShot, shootHit.point);
					}


					// Set the second position of the line renderer to the point the raycast hit.
					//gunLine.SetPosition (1, shootHit.point);
					finalPoint = shootHit.point + shootHit.normal.normalized + shootRay.direction.normalized;
					//gunLine.SetPosition (1, finalPoint);
					currentRange -= (finalPoint - shootRay.origin).magnitude;
					shootRay.origin = finalPoint;
					//gunLine.SetPosition (0, finalPoint);

				}
				gunLine.SetPosition (1, transform.position + shootRay.direction * range);
			} else {
				if (Physics.Raycast (shootRay, out shootHit, range, shootableMask)) {
					// Try and find an EnemyHealth script on the gameobject hit.
					EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();

					// If the EnemyHealth component exist...
					if (enemyHealth != null) {
						// ... the enemy should take damage.
						enemyHealth.TakeDamage (damagePerShot, shootHit.point);
					}

					// Set the second position of the line renderer to the point the raycast hit.
					gunLine.SetPosition (1, shootHit.point);
				
				}
				else
					gunLine.SetPosition (1, transform.position + shootRay.direction * range);
			}

        }

		public int getBasicDamage()
		{
			return basicDamage;
		}

		public void setOnPower(bool b)
		{
			onPower = b;
		}

		public void setGunLineWidth(float w)
		{
			gunLine.widthMultiplier = w;
		}
    }
}