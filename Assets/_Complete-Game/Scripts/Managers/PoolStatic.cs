using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompleteProject
{
	public enum poolEntity { TRAIL, ORIGIN, ZOMBUNNY, ZOMBEAR, HELLEPHANT, HEART, INVERSE, SUPERGUN, FLAME };

	[System.Serializable]
	public class Pool
	{
		public GameObject pooledObject;
		public int pooledAmount;
		public bool willGrow;
		//private List<GameObject> all;
		private List<GameObject> disabled;

		public Pool()
		{
			//all = new List<GameObject>();
			disabled = new List<GameObject>();
		}

		public void add(GameObject obj)
		{
			//all.Add(obj);
			disabled.Add(obj);
		}

		/*
		public int count()
		{
			return pooledList.Count;
		}
		*/

		/*
		public bool active(int pos)
		{
			return pooledList [pos].activeInHierarchy;
		}
		*/

		/*
		public GameObject obj(int pos)
		{
			return pooledList [pos];
		}
		*/

		public void push(GameObject go)
		{
			disabled.Add(go);
		}

		public GameObject pop(GameObject go = null)
		{
			if (go == null)
			{
				if (disabled.Count == 0)
					return null;
				go = disabled[0];
				disabled.RemoveAt(0);
				return go;
			}
			else
			{
				disabled.Remove(go);
				return go;
			}
		}

	}

}
