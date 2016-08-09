using UnityEngine;
using System.Collections;

public class modelHighlight : MonoBehaviour {

	public GameObject controllerGO;
	Vector3 evenHeight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		evenHeight.x = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.x;
		evenHeight.y = transform.position.y;
		evenHeight.z = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.z;
		transform.LookAt(evenHeight);
	}
}
