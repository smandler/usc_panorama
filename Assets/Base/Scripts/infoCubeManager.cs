using UnityEngine;
using System.Collections;

public class infoCubeManager : getReal3D.MonoBehaviourWithRpc {
	
	// game object dependencies
	public GameObject controllerGO;
	public GameObject infoCube;
	public GameObject infoRing;

	// slide struct

	[System.Serializable]
	public struct slideStruct
	{
		public Texture slide;
		public GameObject toggleOn;
		public GameObject moveTo;
		public string action;
		public bool noTransition;
		public GameObject[] extraToggleOns;
	}

	// look at behaviour variables
	Vector3 evenHeight;
	
	// open shut animation variables
	public float openSpeed = 0.03f;	
	float openTarget = 0.0f;
	float openTween  = 0.0f;
	float openActual = 0.0f;
	
	// turn animation variables
	public float ringSpeed  = 0.1f;
	public float turnTarget = 0.0f;
	public float turnTween  = 0.0f;
	public float turnActual = 0.0f;

    public float moveToSpeed = 1.0f;
	
	int currentPanel = 0;
	int currentSlide = 0;
	public bool slideChangeRequested = false;

    public bool moveActive;
    public Vector3 moveFrom;
    public Vector3 moveTo;
    public Vector3 moveTransition;
    public float moveAmount = 0.0f;
    public float moveSpeed = 0.005f;
    public AnimationCurve moveCurve;

    // slide images and actions
    public Texture blankSlide;
    public GameObject toggleOffParent;

    public slideStruct[] slides;
	
	Texture slideToLoad;
	int slideIndexToLoad;
	
	bool turnChanged;
	bool openChanged;
	
	public string pushedInputChar;
	
	/////////////
	//         //
	//  start  //
	//         //
	///////////// 
	
	// Use this for initialization
	void Start () {	
		turnChanged = true;
		openChanged = true;

        moveFrom = slides[0].moveTo.transform.position;
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
		
			Debug.LogWarning ("Infocube approached!\n");
			
			// Otherwise (head node in cave or just normal PC mode) go for it
			
			if(argOther.tag == "Player"){
				
				// trigger the open sequence
				openTarget = 1.0f;
				setSlideTextures(currentPanel, currentSlide, "next");
				
				// push a docking request to Nav
				controllerGO.SendMessage ("dockToPublicInput", this.GetComponent<uniqueId>().customNetworkId);
				
				Debug.LogWarning ("Player has approached an infoCube\n");
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
				
				// trigger the close sequence
				openTarget = 0.0f;
				
				// push an undock request to Nav
				controllerGO.SendMessage ("undockFromPublicInput", this.GetComponent<uniqueId>().customNetworkId);
				
				Debug.LogWarning ("Player has moved away from an infoCube\n");
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
		
		// make a shorter alias for PC mode
		bool cameraModePC = false;
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.PC) { cameraModePC = true; }
		
		// respond to input events
		if(pushedInputChar != ""){
			
			// if the next slide button is being pushed
			if(((cameraModePC && pushedInputChar == "PC_next") || (!cameraModePC && pushedInputChar == "R")) && slideChangeRequested == false){
			// if(pushedInputChar == "R" && slideChangeRequested == false){
				
				// check that this is not the last slide
				if(currentSlide < slides.Length-1){
					
					setSlideTextures(currentPanel, currentSlide, "next");
					
					// set new target rotation
					turnTarget += 90.0f;
					
					// keep track of which panel is facing the player
					currentSlide += 1;
					currentPanel += 1;
					if(currentPanel == 4){ currentPanel = 0; }
					
					foreach(Transform child in toggleOffParent.transform){ child.gameObject.SetActive (false); }
                    slides[currentSlide].toggleOn.SetActive(true);
                    foreach (GameObject turnOn in slides[currentSlide].extraToggleOns){ turnOn.SetActive (true); }

                    // and now also setup the move to stuff
                    if (slides[currentSlide].moveTo != null)
                    {
                        moveFrom = transform.localPosition;
                        moveTo = slides[currentSlide].moveTo.transform.localPosition;
                        moveAmount = 0.0f;
                        moveActive = true;
                    } 

					slideChangeRequested = true;	
					
				} else {
					// Debug.Log ("You're already at the last slide!");
				}
				
				pushedInputChar = "";
			}
			
			// if the previous slide button is being pushed
			if(((cameraModePC && pushedInputChar == "PC_prev") || (!cameraModePC && pushedInputChar == "L")) && slideChangeRequested == false){
			// if(pushedInputChar == "L" && slideChangeRequested == false){
				
				// check that this is not the first slide
				if(currentSlide > 0){
					
					setSlideTextures(currentPanel, currentSlide, "next");
					
					// set new target rotation
					turnTarget -= 90.0f;
					
					// keep track of which panel is facing the player
					currentSlide -= 1;
					currentPanel -= 1;
					if(currentPanel == -1){ currentPanel = 3; }
					foreach(Transform child in toggleOffParent.transform){ child.gameObject.SetActive (false); }
                    slides[currentSlide].toggleOn.SetActive(true);
					foreach(GameObject turnOn in slides[currentSlide].extraToggleOns){ turnOn.SetActive (true); }

                    // move to stuff
                    if (slides[currentSlide].moveTo != null)
                    {
                        moveFrom = transform.localPosition;
                        moveTo = slides[currentSlide].moveTo.transform.localPosition;
                        moveAmount = 0.0f;
                        moveActive = true;
                    }

                    slideChangeRequested = true;
					
				} else {
					// Debug.Log ("You're already at the first slide!");
				}
				
				pushedInputChar = "";
			}
			
			// action requested for current slide
			if(pushedInputChar == "S" && slideChangeRequested == false){
				
				if(currentSlide > 0 && currentSlide < slides.Length){
					if(slides[currentSlide].action != "")
					{
						Debug.LogWarning ("Tour requested\n");
						controllerGO.GetComponent<controller>().notify ("Infocube", slides[currentSlide].action, this.gameObject);
					}
				}
				
				pushedInputChar = "";
			}
			
			// clear out pushedInputChar
			pushedInputChar = "";
		}
		
		// if neither are being pushed clear slideChangeRequested
		if(pushedInputChar == "" && turnActual == turnTarget){
			slideChangeRequested = false;
		}
		
		///////////////////////////////
		//// Set animation drivers ////
		///////////////////////////////
		
		// manually handle open/close animation //
		if(openActual < openTarget){ openActual += openSpeed * Time.deltaTime; openChanged = true; }
		if(openActual > openTarget){ openActual -= openSpeed * Time.deltaTime; openChanged = true; }
		if(openActual >= openTarget - (openSpeed * Time.deltaTime) && openActual <= openTarget + (openSpeed * Time.deltaTime) && openActual != openTarget){ openActual = openTarget; openChanged = true; }
		
		// pump openActual through the singleton RPC machinery
		if(openChanged){
			setSlideOpen(openActual);
			openChanged = false;
		}
		
		// manually handle ring rotation // 
		if(slides[currentSlide].noTransition == false){
		if(turnActual < turnTarget - (ringSpeed * Time.deltaTime)) { turnActual += ringSpeed * Time.deltaTime; turnChanged = true; }
		if(turnActual > turnTarget + (ringSpeed * Time.deltaTime)) { turnActual -= ringSpeed * Time.deltaTime; turnChanged = true; }
		if(turnActual >= turnTarget - (ringSpeed * Time.deltaTime) && turnActual <= turnTarget + (ringSpeed * Time.deltaTime) && turnActual != turnTarget){ turnActual = turnTarget; turnChanged = true; }
		} else {
			turnActual = turnTarget;
			turnChanged = true;
		}
		
		// pump turnActual through the singleton RPC machinery
		if(turnChanged){
			setSlideTurn(turnActual);
			turnChanged = false; 
		}
		
		//////////////////////////////////////////
		//// make the infobox face the player ////
		//////////////////////////////////////////
		
		evenHeight.x = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.x;
		evenHeight.y = transform.position.y;
		evenHeight.z = controllerGO.GetComponent<controller>().sim.camera.actualCamera.transform.position.z;
		transform.LookAt(evenHeight);

        /////////////////////////////////////////
        // move to the moveTo point if offered //
        /////////////////////////////////////////

        // public bool moveActive;
        // public Vector3 moveFrom;
        // public Vector3 moveTo;
        // public float moveAmount;
        // public float moveSpeed;
        // public AnimationCurve moveCurve;

        if (moveActive)
        {
            if(moveAmount < 1.0) { moveAmount += moveSpeed; }
            if(moveAmount > 1.0) { moveAmount = 1.0f; }
            moveTransition = Vector3.Lerp(moveFrom, moveTo, moveCurve.Evaluate(moveAmount));

            // transform.localPosition = moveTransition;
            setPosition(moveTransition);
        }
}
	
	////////////////////
	//                //
	//  setSlideOpen  //
	//                //
	//////////////////// 
	
	void setSlideOpen(float argOpenActual){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_infoCube_broadcastSlideOpen", this.GetComponent<uniqueId>().customNetworkId, argOpenActual);
		} else {
			setSlideOpen_rpc(argOpenActual);
		}
	}
	
	// must be triggered by singleton rpc call on NAV
	public void setSlideOpen_rpc(float argOpenActual){
		infoCube.GetComponent<Animation>()["Take 001"].time = argOpenActual;
		infoCube.GetComponent<Animation>()["Take 001"].speed = 0.0f;
		infoCube.GetComponent<Animation>().Play ("Take 001");
	}
	
	////////////////////
	//                //
	//  setSlideTurn  //
	//                //
	//////////////////// 
	
	void setSlideTurn(float argTurnActual){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_infoCube_broadcastSlideTurn", this.GetComponent<uniqueId>().customNetworkId, argTurnActual);
		} else {
			setSlideTurn_rpc(argTurnActual);
		}
	}	
	
	// must be triggered by singleton rpc call on NAV
	public void setSlideTurn_rpc(float argTurnActual){		
		infoCube.transform.localEulerAngles = new Vector3 (0.0f, argTurnActual, 0.0f);
	}

    ////////////////////////
    //                    //
    //  setSlidePosition  //
    //                    //
    //////////////////////// 

    void setPosition(Vector3 argNewPosition)
    {
        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("rpc_infoCube_broadcastMove", this.GetComponent<uniqueId>().customNetworkId, argNewPosition);
        }
        else
        {
            setPosition_rpc(argNewPosition);
        }
    }

    // must be triggered by singleton rpc call on NAV
    public void setPosition_rpc(Vector3 argNewPosition)
    {
        transform.localPosition = argNewPosition;
    }

    ////////////////////////
    //                    //
    //  setSlideTextures  //
    //                    //
    //////////////////////// 

    void setSlideTextures(int argCurrentPanel, int argFrontSlideIndexToLoad, string argDirection){
		
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){ 
			getReal3D.RpcManager.call("rpc_infoCube_broadcastSlideTextures", this.GetComponent<uniqueId>().customNetworkId, argCurrentPanel, argFrontSlideIndexToLoad, argDirection);
		} else {
			setSlideTextures_rpc(argCurrentPanel, argFrontSlideIndexToLoad, argDirection);
		}
	}	
	
	// must be triggered by singleton rpc call on NAV (if we're running in the cave)
	public void setSlideTextures_rpc(int argCurrentPanel, int argFrontSlideIndexToLoad, string argDirection){
		
		Debug.LogWarning ("[ Setting slide textures on "+this.name+" ]\n");
		Debug.LogWarning ("ArgCurrentPanel: "+argCurrentPanel+"\n");
		
		string targetSlideName = "";
		
		// load appropriate texture into the back panel AND REFRESH VISIBLE PANELS
		Material[] temp = infoRing.GetComponent<Renderer>().materials;
		for(int i=0; i<temp.Length; i++){
			
			// Debug.Log ("----- Examining slide material: "+temp[i].name+" -----");
			
			// ----- if current loop material is the one on the front panel... ----- //
			
			targetSlideName = "infoPanel"+(((argCurrentPanel)%4)+1)+" (Instance)";
			// Debug.Log ("Front panel: is it "+targetSlideName+"?");
			
			if(temp[i].name == targetSlideName){
				// Debug.Log (">>> Setting "+targetSlideName+" (at front) to slideIndex:"+argFrontSlideIndexToLoad+"\n");
				temp[i].mainTexture = slides[argFrontSlideIndexToLoad].slide;
			} else {
				// Debug.Log ("No, it is not");
			}
			
			// ----- if current loop material is the one on the right panel... ----- //
			
			targetSlideName = "infoPanel"+(((argCurrentPanel+1)%4)+1)+" (Instance)";
			// Debug.Log ("Right panel: is it "+targetSlideName+"?");
			
			if(temp[i].name == targetSlideName){
				if(argFrontSlideIndexToLoad+1 > slides.Length-1)
				{
					// Debug.Log (">>> Setting "+targetSlideName+" (on right) to blank");
					temp[i].mainTexture = blankSlide;
				} else {
					// Debug.Log (">>> Setting "+targetSlideName+" (on right) to slideIndex:"+(argFrontSlideIndexToLoad+1));
					temp[i].mainTexture = slides[argFrontSlideIndexToLoad+1].slide;
				}
			} else {
				// Debug.Log ("No, it is not");
			}
			
			// ----- if current loop material is the one on the left panel... ----- //
			
			if(argCurrentPanel-1 == -1)
			{
				targetSlideName = "infoPanel4 (Instance)";
			} else {
				targetSlideName = "infoPanel"+(((argCurrentPanel-1)%4)+1)+" (Instance)";
			}
			// Debug.Log ("Left panel: is it "+targetSlideName+"?");
			
			if(temp[i].name == targetSlideName){
				if(argFrontSlideIndexToLoad-1 >= 0)
				{
					// Debug.Log (">>> Setting "+targetSlideName+" (on left) to slideIndex:"+(argFrontSlideIndexToLoad-1));
					temp[i].mainTexture = slides[argFrontSlideIndexToLoad-1].slide;
				} else {
					// Debug.Log (">>> Setting "+targetSlideName+" (on left) to blank");
					temp[i].mainTexture = blankSlide;
				}
			} else {
				// Debug.Log ("No, it is not");
			}
			
			// ----- back panel (different depending on direction) ----- //
			
			targetSlideName = "infoPanel"+(((argCurrentPanel+2)%4)+1)+" (Instance)";
			// Debug.Log ("Back panel: is it "+targetSlideName+"?");
			
			if(argDirection == "next"){
				
				// if current loop material is the one on the left panel...
				if(temp[i].name == targetSlideName){
					if(argFrontSlideIndexToLoad+2 > slides.Length-1)
					{
						// Debug.Log (">>> Setting "+targetSlideName+" (at back) to blank");
						temp[i].mainTexture = blankSlide;
					} else {
						// Debug.Log (">>> Setting "+targetSlideName+" (at back) to slideIndex:"+(argFrontSlideIndexToLoad+2));
						temp[i].mainTexture = slides[argFrontSlideIndexToLoad+2].slide;
					}
				} else {
					// Debug.Log ("No, it is not");
				}
				
			}
			
			if(argDirection == "previous"){
				
				// if current loop material is the one on the left panel...
				if(temp[i].name == "infoPanel"+(((argCurrentPanel-2)%4)+1)+" (Instance)"){
					if(argFrontSlideIndexToLoad-2 > 0)
					{
						// Debug.Log (">>> Setting "+targetSlideName+" (at back) to slideIndex:"+(argFrontSlideIndexToLoad-2));
						temp[i].mainTexture = slides[argFrontSlideIndexToLoad-2].slide;
					} else {
						// Debug.Log (">>> Setting "+targetSlideName+" (at back) to blank");
						temp[i].mainTexture = blankSlide;
					}
				} else {
					// Debug.Log ("No, it is not");
				}
				
			}
			
		}
		infoRing.GetComponent<Renderer>().materials = temp;
		
	}


	
}























