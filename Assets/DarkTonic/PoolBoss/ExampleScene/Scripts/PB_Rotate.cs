/*! \cond PRIVATE */
using UnityEngine;
using System.Collections;

public class PB_Rotate : MonoBehaviour {
	// Update is called once per frame
	void Update () {
		this.transform.Rotate(Vector3.up * 200 * Time.deltaTime);	
	}
}
/*! \endcond */