using UnityEngine;
using System.Collections;
using DarkTonic.PoolBoss;

namespace DarkTonic.PoolBoss {
	/// <summary>
	/// This class is used to configure a Timed Despawner
	/// </summary>
	// ReSharper disable once CheckNamespace
	[AddComponentMenu("Dark Tonic/Pool Boss/Timed Despawner")]
	public class TimedDespawner : MonoBehaviour {
	    public float LifeSeconds = 5;
	    public bool StartTimerOnSpawn = true;

	    private Transform _trans;
	    private YieldInstruction _timerDelay;

	    // ReSharper disable once UnusedMember.Local
	    void Awake() {
	        _trans = transform;
	        _timerDelay = new WaitForSeconds(LifeSeconds);
	        AwakeOrSpawn();
	    }

	    // ReSharper disable once UnusedMember.Local
	    void OnSpawned() { // used by Core GameKit Pooling & also Pool Manager Pooling!
	        AwakeOrSpawn();
	    }

	    void AwakeOrSpawn() {
	        if (StartTimerOnSpawn) {
	            StartTimer();
	        }
	    }

	    /// <summary>
	    /// Call this method to start the Timer if it's not set to start automatically.
	    /// </summary>
	    public void StartTimer() {
	        StartCoroutine(WaitUntilTimeUp());
	    }

	    private IEnumerator WaitUntilTimeUp() {
	        yield return _timerDelay;

	        PoolBoss.Despawn(_trans);
	    }
	}
}