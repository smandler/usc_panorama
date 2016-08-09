using UnityEngine;
using System.Collections;

public class billboardManager : MonoBehaviour {

	public GameObject controllerGO;

    public bool scaleOn = true;

    Vector3 evenHeight;

    public float distToCamera;
    public float modifiedScale;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		evenHeight.x = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.x;
		evenHeight.y = transform.position.y;
		evenHeight.z = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.z;
		transform.LookAt(evenHeight);

        distToCamera = Vector3.Distance(controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position, this.transform.position);

        float modifiedScale = distToCamera / 7000;

        Vector3 tempScale = new Vector3(modifiedScale, modifiedScale, modifiedScale);
        if (scaleOn) { this.transform.localScale = tempScale; }

	}
}
