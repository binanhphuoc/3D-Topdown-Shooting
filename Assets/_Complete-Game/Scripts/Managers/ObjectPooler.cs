using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject{

	public enum poolEntity{TRAIL, MONSTER};

	[System.Serializable]
	public class Pool
	{
		public GameObject pooledObject;
		public int pooledAmount;
		public bool willGrow;
		private List<GameObject> pooledList;

		public Pool()
		{
			pooledList = new List<GameObject>();
		}

		public void add(GameObject obj)
		{
			pooledList.Add (obj);
		}

		public int count()
		{
			return pooledList.Count;
		}

		public bool active(int pos)
		{
			return pooledList [pos].activeInHierarchy;
		}

		public GameObject obj(int pos)
		{
			return pooledList [pos];
		}
			
	}

	public class ObjectPooler : MonoBehaviour {

		public static ObjectPooler SharedInstance;
		public Pool[] pool;
		GameObject holder;

		void Awake()
		{
			SharedInstance = this;
			holder = new GameObject ();
		}

		void Start()
		{
			for (int i = 0; i < pool.Length; i++) {
				for (int j = 0; j < pool[i].pooledAmount; j++) {
					GameObject obj = (GameObject)Instantiate(pool[i].pooledObject);
					obj.SetActive(false); 
					obj.transform.SetParent(holder.transform);
					pool[i].add(obj);
				}
			}

		}

		public GameObject GetPooledObject(poolEntity p) {
			//1
			int type = (int) p;
			for (int i = 0; i < pool[type].count(); i++) {
				//2
				if (!pool[type].active(i)) {
					pool [type].obj (i).SetActive (true);
					return pool[type].obj(i);
				}
			}
			//3   
			if (pool[type].willGrow) {
				GameObject obj = (GameObject)Instantiate (pool[type].pooledObject);
				pool[type].add (obj);
				return obj;
			}

			return null;
		}

		public IEnumerator destroyObject(GameObject obj, float time = 0f)
		{
			if (time > 0)
				yield return new WaitForSeconds (time);

			/*for (int i = 0; i < obj.transform.childCount; i++) {
				obj.transform.GetChild(i).gameObject.SetActive(false);
			};*/
			obj.transform.parent = null;
			obj.transform.SetParent(holder.transform);
			obj.SetActive (false);
		}

		public void destroyChildren(GameObject obj)
		{
			for (int i = 0; i < obj.transform.childCount; i++) {
				//obj.transform.GetChild (i).gameObject.SetActive (false);
				//destroyObject (obj.transform.GetChild (i).gameObject);
				destroyObject(obj.transform.GetChild(i).gameObject);
			}
			//obj.transform.DetachChildren ();
		}

	}

}