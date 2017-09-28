using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CompleteProject{

	public class AidDisplayBar : MonoBehaviour {

		private GameObject[] aidItems;				// Contain image for real-time display
		private GameObject player;
		public int[] itemList;				// Mark item data in accordance with enum
		public Sprite[] imageList;
		private int[] mapback;
		private int numberOfItems;
		private int activeCount = 0;

		// Use this for initialization

		void Start () {

			numberOfItems = gameObject.transform.childCount;

			itemList = new int[numberOfItems];
			mapback = new int[numberOfItems];

			player = GameObject.FindGameObjectWithTag ("Player");

			aidItems = new GameObject[numberOfItems];

			for (int i = 0; i < numberOfItems; i++) {
				aidItems[i] = gameObject.transform.GetChild (i).gameObject;
				aidItems[i].SetActive(false);

				//itemList[i].isActive = false;
				//itemList[i].timer = 0f;
				itemList[i] = -1;
				activeCount = 0;
				mapback [i] = -1;
			}

		}
		
		// Update is called once per frame
		void Update () {
			if (player.gameObject.GetComponent<PlayerHealth> ().currentHealth <= 0) {
				gameObject.SetActive (false);
			}

			//updateDisplay ();
		}

		public void setTimer(float timerFraction, AidEnum type){


			if (itemList [(int)type] == -1) {
				itemList [(int)type] = activeCount;
				mapback [activeCount] = (int)type;


				//i = itemList [i].position;
				aidItems [activeCount].SetActive (true);
				aidItems [activeCount].transform.GetChild (0).gameObject.GetComponent<Image> ().sprite = imageList [(int)type];

				GameObject timerDisplay = aidItems [activeCount].transform.GetChild (1).gameObject;
		
				timerDisplay.GetComponent<Image> ().rectTransform.sizeDelta = 
					new Vector2 (100, (1 - timerFraction) * 100);

				activeCount++;
			} else {
				GameObject timerDisplay = aidItems [itemList[(int)type]].transform.GetChild (1).gameObject;
	
				timerDisplay.GetComponent<Image> ().rectTransform.sizeDelta = 
					new Vector2 (100, (1 - timerFraction) * 100);
			}

			//Debug.Log (itemList [i].position);

		}
			
		public void disableTimer(AidEnum type)
		{
			/*for (int j = 0 ; j < numberOfItems; j++)
				Debug.Log (itemList[j]);
			Debug.Log ("end");*/

			int i = (int)type;
			int position = itemList [i];
			itemList [i] = -1;

			if (position == -1) {
				
				return;
			}
			Debug.Log (position);
			mapback [position] = -1;


			for (i = position; i < activeCount-1; i++) {
				//Debug.Log (i);
				aidItems [i].SetActive (true);
				Image image = aidItems [i].transform.GetChild (0).gameObject.GetComponent<Image> ();


				image.sprite = aidItems [i+1].transform.GetChild (0).gameObject.GetComponent<Image> ().sprite;

				//Image timerDisplay = aidItems [i].transform.GetChild (1).gameObject.GetComponent<Image> ();

				mapback [i] = mapback [i + 1];
				itemList [mapback [i+1]]--;


			}

			activeCount--;

			aidItems [activeCount].SetActive (false);
				
		}
	}
}
