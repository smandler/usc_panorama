using UnityEngine;
using System.Collections;

public class modelWandCollision : MonoBehaviour {

	public GameObject controllerGO;
	public GameObject modelManager;
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	///////////////////////////
	//                       //
	//  Proximity Detection  //
	//                       //
	///////////////////////////
	
	void OnTriggerEnter(Collider argOther){
		
		if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
		{ 
			// if we're in the cave and this is NOT the head node, don't do anything...
			
		} else {
			
			if(argOther.tag == "wandCollide"){
				modelManager.GetComponent<modelManager>().setWandCollide(true);
			}
		}
		
	}
	
	void OnTriggerExit(Collider argOther){
		
		if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
		{
			// if we're in the cave and this is NOT the head node, don't do anything...
			
		} else {
			
			if(argOther.tag == "wandCollide"){
				modelManager.GetComponent<modelManager>().setWandCollide(false);
			}
		}
		
	}
}
