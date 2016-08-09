using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class uiManager : getReal3D.MonoBehaviourWithRpc {

	// game object dependencies
	public GameObject controllerGO;
	public GameObject debugText;
	
	// offsets
	public float cameraOffsetMultiplier = 1.0f;
	public float cameraOffsetDirection = 1.0f;

	/////////////
	//         //
	//  start  //
	//         //
	///////////// 
	
	// Use this for initialization
	void Start () {
	
	}

	//////////////
	//          //
	//  update  //
	//          //
	//////////////
	
	// Update is called once per frame
	void Update () {
	
		//////////////////////////
		//// input management ////
		//////////////////////////
		
		if(Input.GetKeyDown (KeyCode.F1)){ sendInput("F1"); }
		if(Input.GetKeyDown (KeyCode.F2)){ sendInput("F2"); }
		
		if(Input.GetKeyDown (KeyCode.Q)){ sendInput("Q"); }
		if(Input.GetKeyDown (KeyCode.E)){ sendInput("E"); }
		
		if(Input.GetKeyDown (KeyCode.R)){ sendInput("R"); }
		if(Input.GetKeyDown (KeyCode.T)){ sendInput("T"); }
		
		if(Input.GetKeyDown (KeyCode.F)){ sendInput("F"); }
		if(Input.GetKeyDown (KeyCode.G)){ sendInput("G"); }
		
		///////////////////////////
		//// offset management ////
		///////////////////////////
		
		// get the rotation of the camera in the world
		float cameraRotationY = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.eulerAngles.y;
		
		float offset;
		
		// calculate the offset
		if(cameraRotationY < 180){
			offset = (cameraRotationY * cameraOffsetMultiplier) * cameraOffsetDirection;
		} else {
			offset = ((cameraRotationY-360.0f) * cameraOffsetMultiplier) * cameraOffsetDirection;
		}
		
		Vector3 newLocation = new Vector3(offset,transform.localPosition.y,transform.localPosition.z);
		
		transform.localPosition = newLocation;
		
		debugText.GetComponent<Text>().text = cameraOffsetMultiplier+","+cameraOffsetDirection;
		
		
	}
	
	////////////////////
	//                //
	//  processInput  //
	//                //
	////////////////////
	
	void sendInput(string argInput){
		if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && getReal3D.Cluster.isMaster){
			getReal3D.RpcManager.call("rpc_UI_processInput", argInput);
		} else {
			processInput_rpc(argInput);
		}
	}
	
	[getReal3D.RPC]
	public void rpc_UI_processInput(string argInput) {
		processInput_rpc(argInput);
	}
	
	void processInput_rpc(string argInput) {
		
		if(argInput == "F1"){ cameraOffsetDirection =  1.0f; }
		if(argInput == "F2"){ cameraOffsetDirection = -1.0f; }
		
		if(argInput == "Q"){ cameraOffsetMultiplier *= 0.5f; }
		if(argInput == "E"){ cameraOffsetMultiplier *= 2.0f; }
		
		if(argInput == "R"){ cameraOffsetMultiplier *= 0.25f; }
		if(argInput == "T"){ cameraOffsetMultiplier *= 1.5f; }
		
		if(argInput == "F"){ cameraOffsetMultiplier *= 0.125f; }
		if(argInput == "G"){ cameraOffsetMultiplier *= 1.25f; }
		
	}
}






