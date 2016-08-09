using UnityEngine;
using System.Collections;

// this class reports the rotation of the camera in the scene
// I made it to test whether or not we could determine the rotational differences between the cameras in the cave

public class cameraTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		this.GetComponent<TextMesh>().text = Mathf.RoundToInt (transform.parent.transform.eulerAngles.x) + "," + Mathf.RoundToInt (transform.parent.transform.eulerAngles.y) + "," + Mathf.RoundToInt (transform.parent.transform.eulerAngles.z);
	
	}
}
