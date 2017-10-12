using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject{

	public class ObjectPooler : MonoBehaviour {

		public static ObjectPooler SharedInstance;
		public Pool[] pool;
		public GameObject holder;

		void Awake()
		{
			SharedInstance = this;
			holder = new GameObject ();
			//Debug.Log(string.Format("poolLength {0}", pool.Length));
			for (int i = 0; i< pool.Length; i++)
			{
				for (int j = 0; j<pool[i].pooledAmount; j++)
				{
					GameObject obj = Instantiate(pool[i].pooledObject);
					obj.SetActive(false);
					obj.transform.SetParent(holder.transform);
					pool[i].add(obj);
				}

				//Debug.Log(pool[i].count());
			}
		}
		/*
		void Start()
		{
			Debug.Log(string.Format("poolLength {0}", pool.Length));
			for (int i = 0; i < pool.Length; i++)
			{
				for (int j = 0; j < pool[i].pooledAmount; j++)
				{
					GameObject obj = (GameObject)Instantiate(pool[i].pooledObject);
					obj.SetActive(false);
					obj.transform.SetParent(holder.transform);
					pool[i].add(obj);
				}
				Debug.Log(pool[i].count());
			}

		}*/

		public void setActive(GameObject obj, bool b)
		{
			//foreach (Behaviour childCompnent in obj.GetComponentsInChildren<Behaviour>())
			//childCompnent.enabled = b;
			if (obj == null)
				return;
			obj.SetActive(b);
			/*
			if (b == true)
			{
				PoolMonoBehaviour pmb = obj.GetComponent<PoolMonoBehaviour>();
				if (pmb != null)
					pmb.OnEnabled();
			}
			*/
		}

		/*
		public GameObject GetPooledObject(poolEntity p, Vector3 position, Quaternion rotation, GameObject parent = null) {
			//1
			int type = (int) p;
			for (int i = 0; i < pool[type].count(); i++) {
				//2
				if (!pool[type].active(i)) {
					//setActive(pool [type].obj (i),true);
					pool[type].obj(i).transform.position = position;
					pool[type].obj(i).transform.rotation = rotation;
					if (parent != null)
						pool[type].obj(i).transform.SetParent(parent.transform);
					return pool[type].obj(i);
				}
			}
			//3   
			if (pool[type].willGrow) {
				GameObject obj = (GameObject)Instantiate (pool[type].pooledObject);
                //setActive(obj,true);
				pool[type].add (obj);
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				if (parent != null)
					obj.transform.SetParent(parent.transform);
				return obj;
			}

			return null;
		}

		public GameObject GetPooledObject(poolEntity p)
		{
			//1
			int type = (int)p;
			//Debug.Log(string.Format("type {0}", type));
			for (int i = 0; i < pool[type].count(); i++)
			{
				//2
				if (!pool[type].active(i))
				{
					//setActive(pool [type].obj (i),true);
					pool[type].obj(i).transform.position = new Vector3();
					pool[type].obj(i).transform.rotation = new Quaternion();
					return pool[type].obj(i);
				}
			}
			//3   
			if (pool[type].willGrow)
			{
				GameObject obj = (GameObject)Instantiate(pool[type].pooledObject);
				//setActive(obj,true);
				obj.transform.position = new Vector3();
				obj.transform.rotation = new Quaternion();
				pool[type].add(obj);
				return obj;
			}

			return null;
		}

		*/

		public GameObject GetPooledObject(poolEntity p, Vector3 position, Quaternion rotation, GameObject parent = null)
		{
			//1
			int type = (int)p;

				//setActive(pool [type].obj (i),true);
			GameObject go = pool[type].pop();
			if (go != null)
			{
				go.transform.position = position;
				go.transform.rotation = rotation;
				if (parent != null)
					go.transform.SetParent(parent.transform);
				return go;
			}

			//3   
			if (pool[type].willGrow)
			{
				GameObject obj = (GameObject)Instantiate(pool[type].pooledObject);
				//setActive(obj,true);
				pool[type].add(obj);
				obj = pool[type].pop(obj);
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				if (parent != null)
					obj.transform.SetParent(parent.transform);
				return obj;
			}

			return null;
		}

		public GameObject GetPooledObject(poolEntity p)
		{
			//1
			int type = (int)p;

			//setActive(pool [type].obj (i),true);
			GameObject go = pool[type].pop();
			if (go != null)
			{
				return go;
			}

			//3   
			if (pool[type].willGrow)
			{
				GameObject obj = Instantiate(pool[type].pooledObject);
				//setActive(obj,true);
				pool[type].add(obj);
				obj = pool[type].pop(obj);

				return obj;
			}

			return null;
		}

		public void destroyObject(GameObject obj)
		{
			//if (time > 0)
			//	StartCoroutine(timer(time));
			/*for (int i = 0; i < obj.transform.childCount; i++) {
				obj.transform.GetChild(i).gameObject.SetActive(false);
			};*/
			//obj.transform.parent = null;

			int type = (int) obj.GetComponent<PoolTag>().type;
			pool[type].push(obj);
			obj.transform.SetParent(holder.transform);
			setActive(obj, false);
			//Debug.Log(obj.activeInHierarchy);

		}

		IEnumerator timer(float time)
		{
			yield return new WaitForSeconds(time);
		}

		public void destroyChildren(GameObject obj)
		{
			//Debug.Log(obj);
			while(obj.transform.childCount > 0)
			{
				//obj.transform.GetChild (i).gameObject.SetActive (false);
				//destroyObject (obj.transform.GetChild(i).gameObject);
				//obj.transform.SetParent(holder.transform);
				//setActive(obj
				Transform o = obj.transform.GetChild(0);
				setActive(o.gameObject, false);
				//Debug.Log(o.gameObject.activeInHierarchy);
				o.transform.SetParent(holder.transform);
			}
			//Debug.Log(obj.transform.childCount);
			//obj.transform.DetachChildren ();

		}

	}

}