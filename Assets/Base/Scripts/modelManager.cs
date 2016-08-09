using UnityEngine;
using System.Collections;

public class modelManager : getReal3D.MonoBehaviourWithRpc {
	
	// game object dependencies
	public GameObject controllerGO;
	
	// turntable game object
	public GameObject inspector;
	public GameObject turntable;
	
	public string turnMode = "N";
	public float  turnSpeedMultiplyBase = 5;
	public float  turnSpeedMultiplyTimes = 0;
	public float  turnSpeedMultiplyAmount = 2;
	public float  turnSpeed = 0;
	
	public float  turnTarget = 0.0f;
	public float  turnActual = 0.0f;
	public bool   turnChanged = true;
	
	// game objects to switch between
	public GameObject[] switchArray;
	public int switchCurrent = 0;
		
	public string pushedInputChar;
	
	// mode labels
	public int controlMode = 0;
	public bool controlModeChanged = true;
	public GameObject[] menuLabels;
	public GameObject[] menuValues;
	
	// wand helpers
	public bool wandCollide = false;
	public bool objZoom = false;
	public GameObject highlight;
	
	public Vector3 posStart;
	public Vector3 posFocus;
	
	public float posLerpSpeed = 1.05f;
	public Vector3 posLerpStart;
	
	public GameObject parentOriginal;
		
	/////////////
	//         //
	//  start  //
	//         //
	///////////// 
	
	// Use this for initialization
	void Start () {	
		for(int i=0; i<menuLabels.Length; i++){
			if(menuLabels[i] != null) { menuLabels[i].SetActive (false); }
			if(menuValues[i] != null) { menuValues[i].SetActive (false); }
		}
		posStart = transform.localPosition;
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
		
			Debug.LogWarning ("Object manager approached!\n");
			
			// Otherwise (head node in cave or just normal PC mode) go for it
			
			if(argOther.tag == "Player"){
						
				// push a docking request to Nav
				controllerGO.SendMessage ("dockToPublicInput", this.GetComponent<uniqueId>().customNetworkId);
				
				Debug.LogWarning ("Player has approached a managed object\n");
			}
		
		}
		
	}
	
	void OnTriggerExit(Collider argOther){
		
		if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
		{
			// if we're in the cave and this is NOT the head node, don't do anything...
			
		} else {
			
			// Otherwise (head node in cave or just normal PC mode) go for it
			
			if(argOther.tag == "Player"){
				
				// push an undock request to Nav
				controllerGO.SendMessage ("undockFromPublicInput", this.GetComponent<uniqueId>().customNetworkId);
				
				Debug.LogWarning ("Player has moved away from a sphere group\n");
			}

		}
		
	}
	
	///////////////////
	//               //
	//  pushedInput  //
	//               //
	///////////////////
	
	// this function only get's called on the head node if we're in cave mode
	// also, we have to dead drop the input to the update function because Unity is a spastic idiot
	
	void pushedInput(string argInput){
		pushedInputChar = argInput;
	}
	
	//////////////////////
	//                  //
	//  setWandCollide  //
	//                  //
	//////////////////////
	
	public void setWandCollide(bool argWandColliding){
		if(argWandColliding){
			wandCollide = true;
			controllerGO.SendMessage ("dockToWandInput", this.GetComponent<uniqueId>().customNetworkId);
			highlightModel(true);
		} else {
			wandCollide = false;
			controllerGO.SendMessage ("undockFromWandInput", this.GetComponent<uniqueId>().customNetworkId);
			highlightModel(false);
		}
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
		
		// respond to input events
		if(pushedInputChar != ""){
		
			//// ----- UP ----- ////
			
			if(pushedInputChar == "U"){
			
				if(controlMode < menuLabels.Length){
					controlMode += 1;
					controlModeChanged = true;
				}
				pushedInputChar = "";
			}
			
			//// ----- DOWN ----- ////
			
			if(pushedInputChar == "D"){
				
				if(controlMode > 0){
					controlMode -= 1;
					controlModeChanged = true;
				}
				pushedInputChar = "";
			}
			
			if(controlModeChanged == true){
			
				for(int i=0; i<menuLabels.Length; i++){
					if(i == controlMode-1){
						if(menuLabels[i] != null) { menuLabels[i].SetActive (true); }
						if(menuValues[i] != null) { menuValues[i].SetActive (true); }
					} else {
						if(menuLabels[i] != null) { menuLabels[i].SetActive (false); }
						if(menuValues[i] != null) { menuValues[i].SetActive (false); }
					}
				}
				
				controlModeChanged = false;
			}
			
			//// ----- UP ----- ////
			
			if(pushedInputChar == "U"){
				
				// do left stuff here
				Debug.LogWarning ("Sphere left!\n");
				
				// process input based on control mode
				if(controlMode == 0 || controlMode == 1){ turnSpeedMultiplyTimes -= 1; }
				if(controlMode == 2) {
					transform.localScale -= new Vector3(0.1f,0.1f,0.1f);
					menuValues[1].GetComponent<TextMesh>().text = transform.localScale.x.ToString ();
				}
				if(controlMode == 3) {
					Vector3 adjustPos = transform.localPosition;
					adjustPos.y -= 0.1f;
					transform.localPosition = adjustPos;
					menuValues[2].GetComponent<TextMesh>().text = transform.localPosition.y.ToString ();
				}
				
				pushedInputChar = "";
			}
			
			//// ----- DOWN ----- ////
			
			if(pushedInputChar == "D"){

				// do right stuff here
				Debug.LogWarning ("Sphere right!\n");
				
				// process input based on control mode
				if(controlMode == 0 || controlMode == 1){ turnSpeedMultiplyTimes += 1; }
				if(controlMode == 2) {
					transform.localScale += new Vector3(0.1f,0.1f,0.1f);
					menuValues[1].GetComponent<TextMesh>().text = transform.localScale.x.ToString ();
				}
				if(controlMode == 3) {
					Vector3 adjustPos = transform.localPosition;
					adjustPos.y += 0.1f;
					transform.localPosition = adjustPos;
					menuValues[2].GetComponent<TextMesh>().text = transform.localPosition.y.ToString ();
				}
								
				pushedInputChar = "";
			}
			
			//// ----- Activate ----- ////
			
			if(pushedInputChar == "S"){
				
				// do action stuff
				Debug.LogWarning ("Sphere action!\n");
				
				// cycle visibility through the switchArray
				if(switchCurrent < switchArray.Length-1){ switchCurrent++; } else { switchCurrent = 0; }
				
				for(int i= 0; i < switchArray.Length; i++){
					if(i == switchCurrent){ switchArray[i].SetActive (true); } else { switchArray[i].SetActive(false); }
				}
				
				pushedInputChar = "";
			}
			
			//// ----- LB pressed ----- ////
			if(pushedInputChar == "LB" && wandCollide == true){
				Debug.LogWarning ("LB pressed with object focused");
				
				if(objZoom){
					setZoom(false);
				} else {
					setZoom(true);
				}
				
				pushedInputChar = "";			
			}
			
			//// ----- RB pressed ----- ////
			if(pushedInputChar == "RB" && wandCollide == true){
				Debug.LogWarning ("RB pressed with object focused");	
				pushedInputChar = "";		
			}
			
			// clear out pushedInputChar
			pushedInputChar = "";
		}
		
		//// ----- manually handle turntable rotation ----- ////
		
		// set turnSpeed
		if(turnSpeedMultiplyTimes == 0){
			turnSpeed = 0;
		} else {
			if(turnSpeedMultiplyTimes == 1 || turnSpeedMultiplyTimes == -1){
				turnSpeed = turnSpeedMultiplyBase * turnSpeedMultiplyTimes;
			} else {
				turnSpeed = turnSpeedMultiplyBase * (turnSpeedMultiplyAmount * (turnSpeedMultiplyTimes - 1));
			}
		}
		
		// update turnTarget
		turnTarget += turnSpeed * Time.deltaTime;
		
		// if(turnActual < turnTarget - (turnSpeed * Time.deltaTime)) { turnActual += turnSpeed * Time.deltaTime; turnChanged = true; }
		// if(turnActual > turnTarget + (turnSpeed * Time.deltaTime)) { turnActual -= turnSpeed * Time.deltaTime; turnChanged = true; }
		// if(turnActual >= turnTarget - (turnSpeed * Time.deltaTime) && turnActual <= turnTarget + (turnSpeed * Time.deltaTime) && turnActual != turnTarget){ turnActual = turnTarget; turnChanged = true; }
			
		float turnModded = turnTarget % 360.0f;
			
		// pump turnActual through the singleton RPC machinery
		if(turnSpeed != 0){
			setTurn(turnModded);
			turnChanged = false; 
		}
		
		//// ----- manage object zoom ----- ////
		
		posLerpStart = new Vector3(posLerpStart.x / posLerpSpeed, posLerpStart.y, posLerpStart.z / posLerpSpeed);
		
		setLerp(posLerpStart);		
		
	}
	
	///////////////
	//           //
	//  setTurn  //
	//           //
	/////////////// 
	
	void setTurn(float argTurnActual){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_model_broadcastTurn", this.GetComponent<uniqueId>().customNetworkId, argTurnActual);
		} else {
			setTurn_rpc(argTurnActual);
		}
	}
	
	// must be triggered by singleton rpc call on NAV
	public void setTurn_rpc(float argTurnActual){
	
		Vector3 newRotationInspector;
		Vector3 newRotationTurntable;
	
		if(objZoom){
			float wandXrot = controllerGO.GetComponent<controller>().sim.input.wandPivot.transform.eulerAngles.x;
			newRotationInspector = new Vector3(wandXrot,0,0);
		} else {
			newRotationInspector = new Vector3(0,0,0);
		}
		
		newRotationTurntable = new Vector3(0,argTurnActual,0);
		
		inspector.transform.eulerAngles = newRotationInspector;
		turntable.transform.localEulerAngles = newRotationTurntable;
	}
	
	///////////////
	//           //
	//  setLerp  //
	//           //
	/////////////// 
	
	void setLerp(Vector3 argLerp){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_model_broadcastLerp", this.GetComponent<uniqueId>().customNetworkId, argLerp);
		} else {
			setLerp_rpc(argLerp);
		}
	}
	
	// must be triggered by singleton rpc call on NAV
	public void setLerp_rpc(Vector3 argLerp){
		transform.localPosition = argLerp;		
	}
	
	///////////////
	//           //
	//  setZoom  //
	//           //
	/////////////// 
	
	void setZoom(bool argZoom){
	
		Debug.LogWarning ("Calling setZoom_rpc");
	
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_model_broadcastZoom", this.GetComponent<uniqueId>().customNetworkId, argZoom);
		} else {
			setZoom_rpc(argZoom);
		}
	}
	
	// must be triggered by singleton rpc call on NAV
	public void setZoom_rpc(bool argZoom){
		
		if(argZoom){
			Debug.LogWarning ("setZoom_rpc called (argZoom = true)");
			objZoom = true;
			turntable.SetActive (false);
			// transform.parent = controllerGO.GetComponent<controller>().sim.camera.inspectionPoint.transform;
			// posLerpStart = transform.localPosition;
			turntable.SetActive (true);
		} else {
			Debug.LogWarning ("setZoom_rpc called (argZoom = false)");
			objZoom = false;	
			turntable.SetActive (false);
			// transform.parent = parentOriginal.transform;
			// posLerpStart = transform.localPosition;
			turntable.SetActive (true);
		}
		
	}
	
	//////////////////////
	//                  //
	//  highlightModel  //
	//                  //
	//////////////////////
	
	void highlightModel(bool argActive){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_model_broadcastHighlight", this.GetComponent<uniqueId>().customNetworkId, argActive);
		} else {
			highlightModel_rpc(argActive);
		}
	}
	
	// must be triggered by singleton rpc call on NAV
	public void highlightModel_rpc(bool argActive){
		highlight.SetActive (argActive);
	}
}























