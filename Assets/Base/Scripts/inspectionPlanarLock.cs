using UnityEngine;
using System.Collections;

public class inspectionPlanarLock : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float correctedX = 360.0f - transform.parent.transform.localRotation.eulerAngles.x;
		transform.localEulerAngles = new Vector3 (correctedX, 0.0f, 0.0f);
	}
}
