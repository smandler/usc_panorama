using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// note the modified monobehaviour to support RPC!

public class controller : getReal3D.MonoBehaviourWithRpc {
    public PresentationControl pr = new PresentationControl();

    /////////////////////
    // Mode management //
    /////////////////////

    [System.Serializable]
	public enum modeTypes
	{
		Manual,  // allow the user to walk around manually
		Menu,   // interact with a SYSTEM menu
		Tour   // animate the camera along prepared paths
	}
	
	////////////////////////////
	// Environment management //
	////////////////////////////
	
	[System.Serializable]
	public struct environmentStruct
	{
		public int current;
		public GameObject[] options;
	}
	
	////////////////////////////
	// Content set management //
	////////////////////////////
	
	[System.Serializable]
	public struct contentSetStruct
	{
		public int current;
		public GameObject[] options;
	}

    // system menus are menus that are the same regardless of where you view them from (i.e. program settings / options)
    // location based content menus (along with all the other types of interactions) are managed in a different data structure

    /////////////////////
    // Menu management //
    /////////////////////

    //// Simple Menu ////

    // playback elements //

    [System.Serializable]
    public struct SM_playbackStruct
    {
        public GameObject root;
        public Texture[] slides;
    }

    // main menu elements

    [System.Serializable]
    public struct SM_mainOptionsStruct
    {
        public string command;
        public string argument;
        public Texture slide;
    }

    [System.Serializable]
    public struct SM_mainPageStruct
    {
        public string name;
        public SM_mainOptionsStruct[] option;
    }

    [System.Serializable]
    public struct SM_mainBaseStruct
    {
        public GameObject root;
        public int SM_column;
        public int SM_row;
        public SM_mainPageStruct[] page;
    }

    // info panel elements

    [System.Serializable]
    public struct SM_infoPanels
    {
        public GameObject mesh;
        public Texture[] slides;
    }

    // simple menu base struct

    [System.Serializable]
    public struct SM_baseStruct
    {
        public GameObject root;
        public SM_playbackStruct playback;
        public SM_mainBaseStruct main;
        public SM_infoPanels[] panels; 
    }

    [System.Serializable]
    public struct GridUiStruct
    {
        public GameObject root;
        public Vector3 pos;
        public Vector3 rot;
        public Vector3 siz;
        public float transformMultiplier;
        public float posAdjust;
        public float rotAdjust;
        public float sizAdjust;
    }

    // old EWB menu

    [System.Serializable]
    public struct menuStruct
    {
        public GameObject root;
        public GameObject handle;
        public GameObject marker;
        public GameObject items;
    }

    [System.Serializable]
	public struct systemMenusStruct
	{
        public SM_baseStruct simpleMenu;
		public GridUiStruct GridUI;
	}

	/////////////////////
	// Tour management //
	/////////////////////

	[System.Serializable]
	public struct tourStruct
	{
		public GameObject GuidesHomeGO;
		public GameObject GuideStarter;

        public GameObject GuideCurrent;
        public int GuideCurrentId;

        public int GuideCurrentSpeed;
		public bool tourStarted;
		public bool tourFinished;
        public modeTypes returnMode;
        public GameObject playbackIndicator;
        public Texture[] playbackStates;
	}

	///////////////////////
	// Camera management //
	///////////////////////
	
	// The three supported platforms
	[System.Serializable]
	public enum cameraTypes { ACL, CS, Cave, PC, Immerse }
	
	// Camera sub objects
	[System.Serializable]
	public struct cameraSubObjectsStruct
	{
		public GameObject root;
		public GameObject fpc;
		public GameObject actualCamera;
		public GameObject fpcParent;
		public GameObject inspectionPoint;
		public GameObject screenGrid;
        public GameObject canvasSocket;
	}
	
	// Camera presets struct
	[System.Serializable]
	public struct cameraPresetsStruct
	{
		public cameraSubObjectsStruct ACL;
		public cameraSubObjectsStruct CS;
		public cameraSubObjectsStruct Cave;
		
		public cameraSubObjectsStruct PC;
		
		public cameraSubObjectsStruct Immerse;
	}

	bool lockTransforms = true;
	bool unlockTransforms = false;

	////////////////////////
	// Network Management //
	////////////////////////
	
	[System.Serializable]
	public struct networkStruct
	{
		public GameObject[] idRefs;		
	}
	
	//////////////////////
	// Input Management //
	//////////////////////
	
	[System.Serializable]
	public enum inputModes
	{
		Public, // when any object (within a certain distance is receiving input)
		Private // when only one object is receiving input (i.e. a modal dialog)
	}
	
	// main input struct	
	[System.Serializable]
	public struct inputStruct
	{
		public inputModes mode;
		public int dockPrivate;
		public int[] dockPublic;
		public int[] dockWand;
		public bool  dockWandMultiple;
		public GameObject wandPivot;
		public bool  processed;
		public string PCkeyMode;
	}
	
	//////////////////////////////////
	// Sim Management (root struct) //
	//////////////////////////////////
	
	[System.Serializable]
	public struct simStruct
	{
		public cameraTypes cameraMode;
		public modeTypes playerMode; 
		public environmentStruct environments;
		public contentSetStruct contentSets;
		public networkStruct network;
		public systemMenusStruct menus;
		public tourStruct tours;
		public inputStruct input;
		public cameraSubObjectsStruct camera;
		public cameraPresetsStruct cameraPresets;
	}
	
	public simStruct sim;
	 
	///////////////
	//           //
	//   Start   //
	//           //
	///////////////
	
	void Start () {
	
		// initialize input
		sim.input.mode = inputModes.Public;
		sim.input.dockPublic = new int[10];
		for(int i=0; i<sim.input.dockPublic.Length; i++){ sim.input.dockPublic[i] = -1; }
		sim.input.dockWand = new int[10];
		for(int i=0; i<sim.input.dockWand.Length; i++){ sim.input.dockWand[i] = -1; }

		// initialize the multiplatform agnosticator
		if (sim.cameraMode == cameraTypes.PC){
		
			sim.camera.root = sim.cameraPresets.PC.root; 
			sim.camera.fpc = sim.cameraPresets.PC.fpc; 
			sim.camera.actualCamera = sim.cameraPresets.PC.actualCamera; 
			sim.camera.fpcParent = sim.cameraPresets.PC.fpcParent; 
			sim.camera.inspectionPoint = sim.cameraPresets.PC.inspectionPoint;
			sim.camera.screenGrid = sim.cameraPresets.PC.screenGrid; 
            sim.camera.canvasSocket = sim.cameraPresets.PC.canvasSocket;
			
			sim.cameraPresets.PC.root.SetActive (true);
			sim.cameraPresets.Immerse.root.SetActive (false);
			sim.cameraPresets.Cave.root.SetActive (false);
		}
		if (sim.cameraMode == cameraTypes.Immerse){
		
			sim.camera.root = sim.cameraPresets.Immerse.root; 
			sim.camera.fpc = sim.cameraPresets.Immerse.fpc; 
			sim.camera.actualCamera = sim.cameraPresets.Immerse.actualCamera; 
			sim.camera.fpcParent = sim.cameraPresets.Immerse.fpcParent; 
			sim.camera.inspectionPoint = sim.cameraPresets.Immerse.inspectionPoint; 
			sim.camera.screenGrid = sim.cameraPresets.Immerse.screenGrid; 
            sim.camera.canvasSocket = sim.cameraPresets.Immerse.canvasSocket;
			
			sim.cameraPresets.PC.root.SetActive (false);
			sim.cameraPresets.Immerse.root.SetActive (true);
			sim.cameraPresets.Cave.root.SetActive (false);
		}
		if (sim.cameraMode == cameraTypes.Cave){
		
			sim.camera.root = sim.cameraPresets.Cave.root; 
			sim.camera.fpc = sim.cameraPresets.Cave.fpc; 
			sim.camera.actualCamera = sim.cameraPresets.Cave.actualCamera; 
			sim.camera.fpcParent = sim.cameraPresets.Cave.fpcParent; 
			sim.camera.inspectionPoint = sim.cameraPresets.Cave.inspectionPoint; 
			sim.camera.screenGrid = sim.cameraPresets.Cave.screenGrid; 
            sim.camera.canvasSocket = sim.cameraPresets.Cave.canvasSocket;
			
			sim.cameraPresets.PC.root.SetActive (false);
			sim.cameraPresets.Immerse.root.SetActive (false);
			sim.cameraPresets.Cave.root.SetActive (true);
		}
		
		// hide all the screen grids
		foreach(Transform screenGrid in sim.menus.GridUI.root.transform){
			screenGrid.gameObject.SetActive (false);
		}
		
        sim.camera.screenGrid.SetActive(true); // <--- will want to turn off for builds

		// attach the relevant screengrid to the relevant camera and turn it on
        sim.menus.GridUI.root.transform.parent = sim.camera.fpc.transform;
        sim.menus.GridUI.root.transform.localPosition = new Vector3(0,0,0);
        sim.menus.GridUI.root.gameObject.SetActive (true);

        if (sim.menus.simpleMenu.root) {

            // move simple menu to the relevant canvasSocket (determined by camera selection)
            sim.menus.simpleMenu.root.transform.parent = sim.camera.canvasSocket.transform;
            sim.menus.simpleMenu.root.transform.localPosition = new Vector3(0, 0, 0);
            sim.menus.simpleMenu.root.gameObject.SetActive(true);

            Material[] temp = sim.menus.simpleMenu.main.root.GetComponent<Renderer>().materials;
            temp[0].mainTexture = sim.menus.simpleMenu.main.page[0].option[0].slide;

        }

        //// Initialize tour ////
        // tourStart(networkIdFromName(sim.tours.GuideStarter.name));
	}
	
	////////////////
	//            //
	//   Update   //
	//            //
	////////////////
	
	void Update () {
	
		// Stupid cave split fix
		if (sim.cameraMode == cameraTypes.Cave)
		{
			if(getReal3D.Cluster.isMaster) {
				getReal3D.RpcManager.call("fixPlayerTransform", sim.camera.fpc.transform.position, sim.camera.fpc.transform.rotation);
			}			
		}

		// ------------------------ [ multi platform input management ] ------------------------ //
		
		string newInput = "";
		if (sim.cameraMode == cameraTypes.Cave && getReal3D.Cluster.isMaster)
		{
			// generic input capturing
			if(getReal3D.Input.valuators[5] ==  1) { newInput = "U"; } // Primary up
			if(getReal3D.Input.valuators[5] == -1) { newInput = "D"; } // Primary down
			if(getReal3D.Input.valuators[4] == -1) { newInput = "L"; } // Primary left
			if(getReal3D.Input.valuators[4] ==  1) { newInput = "R"; } // Primary right
			
			if(getReal3D.Input.buttons[4] == 1)    { newInput = "S"; }   // Primary select (left bumper on gamepad controller)
			if(getReal3D.Input.buttons[5] == 1)    { newInput = "ESC"; } // Primary escape (right bumper on gamepad controller)
			
			if(getReal3D.Input.buttons[6] == 1)    { newInput = "LB"; } // Left trigger on gamepad controller
			if(getReal3D.Input.buttons[7] == 1)    { newInput = "RB"; } // Right trigger on gamepad controller
			
			// switch content shortcuts
			if(Input.GetKeyDown(KeyCode.B)) { newInput = "B";  } // Switch background content
			if(Input.GetKeyDown(KeyCode.C)) { newInput = "C";  } // Switch foreground content
			
			// screen grid tweaking shortcuts
			if(Input.GetKeyDown (KeyCode.I)) { newInput = "I"; } // Secondary up
			if(Input.GetKeyDown (KeyCode.J)) { newInput = "J"; } // Secondary down
			if(Input.GetKeyDown (KeyCode.K)) { newInput = "K"; } // Secondary left
			if(Input.GetKeyDown (KeyCode.L)) { newInput = "Lkey"; } // Secondary right
			
			if(Input.GetKeyDown (KeyCode.U)) { newInput = "U"; } // Secondary forward
			if(Input.GetKeyDown (KeyCode.O)) { newInput = "O"; } // Secondary backward
			
			if(Input.GetKeyDown (KeyCode.N)) { newInput = "N"; } // Secondary scale up
			if(Input.GetKeyDown (KeyCode.M)) { newInput = "M"; } // Secondary scale down
			
			if(Input.GetKeyDown (KeyCode.Z)) { newInput = "Z"; } // Secondary rotate counterclockwise
			if(Input.GetKeyDown (KeyCode.X)) { newInput = "X"; } // Secondary rotate clockwise
			
			if(Input.GetKeyDown (KeyCode.Comma)) { newInput = ","; } // half transform modifier
			if(Input.GetKeyDown (KeyCode.Period)) { newInput = "."; } // double transform modifier
			
		} else {
		
			// generic input capturing
			if(Input.GetKeyDown (KeyCode.LeftControl)) { newInput = "CTRL"; } // Navigate / Interact mode switch
			
			if(Input.GetKeyDown(KeyCode.W)) { newInput = "U";  } // Primary up
			if(Input.GetKeyDown(KeyCode.S)) { newInput = "D";  } // Primary down
			if(Input.GetKeyDown(KeyCode.A)) { newInput = "L";  } // Primary left
			if(Input.GetKeyDown(KeyCode.D)) { newInput = "R";  } // Primary right
			
			if(Input.GetKeyDown(KeyCode.Space))   { newInput = "S"; }   // Primary select
			if(Input.GetKeyDown (KeyCode.Escape)) { newInput = "ESC"; } // Primary escape
			
			// switch content shortcuts
			if(Input.GetKeyDown(KeyCode.B)) { newInput = "B";  } // Switch background content
			if(Input.GetKeyDown(KeyCode.C)) { newInput = "C";  } // Switch foreground content
			
			// screen grid tweaking shortcuts
			if(Input.GetKeyDown (KeyCode.I)) { newInput = "I"; } // Secondary up
			if(Input.GetKeyDown (KeyCode.J)) { newInput = "J"; } // Secondary down
			if(Input.GetKeyDown (KeyCode.K)) { newInput = "K"; } // Secondary left
			if(Input.GetKeyDown (KeyCode.L)) { newInput = "Lkey"; } // Secondary right
			
			if(Input.GetKeyDown (KeyCode.U)) { newInput = "U"; } // Secondary forward
			if(Input.GetKeyDown (KeyCode.O)) { newInput = "O"; } // Secondary backward
			
			if(Input.GetKeyDown (KeyCode.N)) { newInput = "N"; } // Secondary scale up
			if(Input.GetKeyDown (KeyCode.M)) { newInput = "M"; } // Secondary scale down
			
			if(Input.GetKeyDown (KeyCode.Z)) { newInput = "Z"; } // Secondary rotate counterclockwise
			if(Input.GetKeyDown (KeyCode.X)) { newInput = "X"; } // Secondary rotate clockwise
			
			if(Input.GetKeyDown (KeyCode.Comma)) { newInput = ","; } // half transform modifier
			if(Input.GetKeyDown (KeyCode.Period)) { newInput = "."; } // double transform modifier
		}
		
		// reset input one shot if nothing is being pressed
		if(newInput == ""){ 
			sim.input.processed = false;
		}
		
		// but if something has been pressed we need to process it
		if (newInput != "") {
			
			// first check that the input hasn't already been processed in a previous update
			if (sim.input.processed == false) {
				
				// log input
				Debug.LogWarning ("Input registered: " + newInput + "\n");
                if (newInput == "D")
                {
                    if (sim.playerMode == modeTypes.Manual)
                    {
                        Debug.Log("Should be down");
                    }

                    if (sim.playerMode == modeTypes.Menu)
                    {
                        Debug.Log("Should be down");
                    }

                    if (sim.playerMode == modeTypes.Tour)
                    {
                        /*
                        if (sim.tours.GuideCurrentSpeed > -2)
                        {
                            sim.tours.GuideCurrentSpeed -= 1;
                            setPlaybackIndicator(sim.tours.GuideCurrentSpeed);
                            tourAdjustSpeed(sim.tours.GuideCurrentId, sim.tours.GuideCurrentSpeed);
                        }
                        */
                    }

                    // mark one shot as processed to prevent multiple triggerings of the associated event
                    sim.input.processed = true;
                }

                if (newInput == "U")
                {
                    if(sim.playerMode == modeTypes.Manual)
                    {

                    }

                    if(sim.playerMode == modeTypes.Menu)
                    {

                    }

                    if (sim.playerMode == modeTypes.Tour)
                    {
                        /*
                        if (sim.tours.GuideCurrentSpeed < 2)
                        {
                            sim.tours.GuideCurrentSpeed += 1;
                            setPlaybackIndicator(sim.tours.GuideCurrentSpeed);
                            tourAdjustSpeed(sim.tours.GuideCurrentId, sim.tours.GuideCurrentSpeed);
                        }
                        */
                    }

                    // terrainFlipbookIndex++;
                    // if (terrainFlipbookIndex > terrainFlipbook.Length) { terrainFlipbookIndex = 0; }
                    // terrainTarget.terrainData.SetDetailLayer()

                    // mark one shot as processed to prevent multiple triggerings of the associated event
                    sim.input.processed = true;
                }
				
				//// ---------- public broadcast of global inputs ---------- ////
				
				// toggle the keyboard control mode for PC (wasd: move or manipulate?)
				if(newInput == "CTRL"){
					
					// only process this if we're in PC mode
					if(sim.cameraMode == cameraTypes.PC){
						
						// toggle PCkeyMode between interact and navigate
						if(sim.input.PCkeyMode == "" || sim.input.PCkeyMode == "navigate"){
							sim.input.PCkeyMode = "interact";
						} else {
							if(sim.input.PCkeyMode == "interact"){
								sim.input.PCkeyMode = "navigate";
							}
						}
						
						// lock navigation if in interact mode
						if(sim.input.PCkeyMode == "interact"){
							sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockMovement = true;
							sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockJump = true;
						}
						
						// unlock navigation if in navigate mode
						if(sim.input.PCkeyMode == "navigate"){
							sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockMovement = false;
							sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockJump = false;
						}
											
					}
					
					// mark one shot as processed to prevent multiple triggerings of the associated event
					sim.input.processed = true;		
				}
				
				// toggle the control menu on and off
				
				if (newInput == "ESC") {
					
					// lock in the one shot (this stops ESC going out for public or private broadcast below)
					sim.input.processed = true;
					
					// make sure we're not at the start menu
					
                    Debug.LogWarning("Esc pushed");
					
						// toggle the control menu on and off
                    if(sim.playerMode != modeTypes.Menu)
                    {
                        // if we're coming from tour mode, pause the tour
                        if (sim.playerMode == modeTypes.Tour){ sim.tours.GuideCurrent.GetComponent<tourManager>().SetSpeed(0); }
							
							// pauseEverything ();
                        sim.tours.returnMode = sim.playerMode;
							
                        sim.playerMode = modeTypes.Menu;

                        toggleMenu(true);
							
						} else {
						
                        // turn off the menu
							
							// unPauseEverything ();
                        toggleMenu(false);
							
                        sim.playerMode = sim.tours.returnMode;
							

                        if (sim.playerMode == modeTypes.Tour){ sim.tours.GuideCurrent.GetComponent<tourManager>().SetSpeed(sim.tours.GuideCurrentSpeed); }
					}
					
				}
				
				// toggle between the backgrounds
				
				if(newInput == "B"){
					
					sim.environments.current++;
					if (sim.environments.current == sim.environments.options.Length){ sim.environments.current = 0; }
					
					changeEnvironment(sim.environments.current);
					
					// mark one shot as processed to prevent multiple triggerings of the associated event
					sim.input.processed = true;		
				}
				
				// toggle between the content sets
				
				if(newInput == "C"){
					
					sim.contentSets.current++;
					if (sim.contentSets.current == sim.contentSets.options.Length){ sim.contentSets.current = 0; }
					
					changeContentSet(sim.contentSets.current);
					
					// mark one shot as processed to prevent multiple triggerings of the associated event
					sim.input.processed = true;		
				}
				
				// handle secondary directional input (used to tweak position of screenGrid)
				
				// up
				if(newInput == "I"){
					sim.menus.GridUI.pos.y += (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();

                    //load north scene
                    pr.LoadNewScene(1);

                    sim.input.processed = true;
				}	
				
				// down
				if(newInput == "K"){
					sim.menus.GridUI.pos.y -= (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();

                    //load south scene
                    pr.LoadNewScene(2);

                    sim.input.processed = true;
				}
				
				// left
				if(newInput == "J"){
					sim.menus.GridUI.pos.x -= (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();

                    //load west scene
                    pr.LoadNewScene(3);

                    sim.input.processed = true;
                }
				
				// right
				if(newInput == "Lkey"){
					sim.menus.GridUI.pos.x += (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();

                    //load east scene
                    pr.LoadNewScene(4);

                    sim.input.processed = true;
				}
				
				// forward
				if(newInput == "U"){
					sim.menus.GridUI.pos.z += (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}	
				
				// back
				if(newInput == "O"){
					sim.menus.GridUI.pos.z -= (sim.menus.GridUI.posAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}
				
				// scale up
				if(newInput == "N"){
					// sim.menus.GridUI_siz.x += (sim.menus.GridUI_sizAdjust * sim.menus.GridUI_transformMultiplier);
					sim.menus.GridUI.siz.y += (sim.menus.GridUI.sizAdjust * sim.menus.GridUI.transformMultiplier);
					// sim.menus.GridUI_siz.z += (sim.menus.GridUI_sizAdjust * sim.menus.GridUI_transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}	
				
				// scale down
				if(newInput == "M"){
					// sim.menus.GridUI_siz.x -= (sim.menus.GridUI_sizAdjust * sim.menus.GridUI_transformMultiplier);
					sim.menus.GridUI.siz.y -= (sim.menus.GridUI.sizAdjust * sim.menus.GridUI.transformMultiplier);
					// sim.menus.GridUI_siz.z -= (sim.menus.GridUI_sizAdjust * sim.menus.GridUI_transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}
				
				// rotate CCW
				if(newInput == "Z"){
					sim.menus.GridUI.rot.y -= (sim.menus.GridUI.rotAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}
				
				// rotate CW
				if(newInput == "X"){
					sim.menus.GridUI.rot.y += (sim.menus.GridUI.rotAdjust * sim.menus.GridUI.transformMultiplier);
					broadcastGridUI_transforms();
					sim.input.processed = true;
				}
				
				// halve transform multiplier	
				if(newInput == ","){
					sim.menus.GridUI.transformMultiplier *= 0.5f;
					sim.input.processed = true;
				}
				
				// double transform multiplier	
				if(newInput == "."){
					sim.menus.GridUI.transformMultiplier *= 2.0f;
					sim.input.processed = true;
				}
				
				//// ---------- public broadcast of input to docked objects ---------- ////
				
				// if we're in public input mode send the input to all inputDocked game objects
				if (sim.input.mode == inputModes.Public) {
					foreach (int dockedObjectId in sim.input.dockPublic) {
						if (dockedObjectId != -1) {
							// but if we're in PC mode, only send the input if we're in the special PC interact mode
							if(sim.cameraMode != cameraTypes.PC || sim.input.PCkeyMode == "interact"){
								pushInputToGameObject (dockedObjectId, newInput);
							}
						}
					}
				}
				
				//// ---------- public broadcast of input to WAND docked objects ---------- ////
				
				// if we're in public input mode send the input to all WAND inputDocked game objects
				if (sim.input.mode == inputModes.Public) {
					foreach (int dockedObjectId in sim.input.dockWand) {
						if (dockedObjectId != -1) {
							// but if we're in PC mode, only send the input if we're in the special PC interact mode
							if(sim.cameraMode != cameraTypes.PC || sim.input.PCkeyMode == "interact"){
								pushInputToGameObject (dockedObjectId, newInput);
							}
						}
					}
				}
				
				//// ---------- private broadcast of input to non modal objects ---------- ////
				
				// if we're in private input mode and there is an input, send it to the selected game object
				if (sim.input.mode == inputModes.Private) {
					if (sim.input.dockPrivate != -1) {
						pushInputToGameObject (sim.input.dockPrivate, newInput);
					}
				}

				//// ---------- mark input as processed ---------- ////
				
				// mark one shot as processed to prevent multiple triggerings of the associated event
				sim.input.processed = true;		
			}
		}
		
		////////////////////////////////
		//// screen grid management ////
		////////////////////////////////
		
        /*
		Vector3 updatePos = new Vector3(sim.menus.GridUI_pos.x,sim.menus.GridUI_pos.y,sim.menus.GridUI_pos.z);
		sim.camera.screenGrid.transform.localPosition = updatePos;
		
		Vector3 updateRot = new Vector3(sim.menus.GridUI_rot.x,sim.menus.GridUI_rot.y,sim.menus.GridUI_rot.z);
		sim.camera.screenGrid.transform.localEulerAngles = updateRot;
		
		Vector3 updateSiz = new Vector3(sim.menus.GridUI_siz.x,sim.menus.GridUI_siz.y,sim.menus.GridUI_siz.z);
		sim.camera.screenGrid.transform.localScale = updateSiz;
        */
	}

    ///////////////////////
    //                   //
    //  Tour Management  //
    //                   //
    ///////////////////////

    /// ---------- tourStart ---------- ///

    void tourStart(int argGuide)
    {
        if (sim.cameraMode == cameraTypes.Cave && !getReal3D.Cluster.isMaster)
        {
            // if we're running in the cave and this is a render node, don't process the startTour request
        }
        else
        {
            // if we're here, it means we're either in the cave on the head node or just running as a PC build
            Debug.LogWarning("Tour Start called, argGuide = " + argGuide);

            // set up the tour (ALL NODES)
            tourInitialize(argGuide);

            // put player on guide 
            cameraPiggyBack(argGuide, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), lockTransforms);

            // tell the guide to start calculating spline transforms
            sim.tours.GuideCurrent.GetComponent<tourManager>().SetSpeed(1);
            sim.tours.GuideCurrentSpeed = 1;
            sim.tours.GuideCurrent.GetComponent<tourManager>().Play();
        }
    }

    /// ---------- tourInitialize ---------- ///

    void tourInitialize(int argGuideId)
    {
        if (sim.cameraMode == cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("tourInitialize_rpc", argGuideId);
        }
        else
        {
            tourInitialize_rpc(argGuideId);
        }
    }

    [getReal3D.RPC]
    void tourInitialize_rpc(int argGuideId)
    {
        // set sim mode (suspends normal interaction)
        sim.playerMode = modeTypes.Tour;

        // set currentGuide
        sim.tours.GuideCurrent = sim.network.idRefs[argGuideId];
        sim.tours.GuideCurrentId = argGuideId;
    }

    /// ---------- tourPushTransform ---------- ///

    public void tourPushTransform(int argGuideId, Vector3 argPosition, Vector3 argLookAtPosition)
    {
        if (sim.cameraMode == cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("tourPushTransform_rpc", argGuideId, argPosition, argLookAtPosition);
        }
        else
        {
            tourPushTransform_rpc(argGuideId, argPosition, argLookAtPosition);
        }
    }

    [getReal3D.RPC]
    void tourPushTransform_rpc(int argGuideId, Vector3 argPosition, Vector3 argLookAtPosition)
    {
        // double check that this instance of the sim is in tour mode (safety feature)
        if (sim.playerMode == modeTypes.Tour)
        {
            sim.network.idRefs[argGuideId].GetComponent<tourManager>().SetTransform(argPosition, argLookAtPosition);
        }
    }
    public void tourAdjustSpeed(int argGuideId, int argSpeed)
    {
        if (sim.cameraMode == cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("tourAdjustSpeed_rpc", argGuideId, argSpeed);
        }
        else
        {
            tourAdjustSpeed_rpc(argGuideId, argSpeed);
        }
    }
    [getReal3D.RPC]
    void tourAdjustSpeed_rpc(int argGuideId, int argSpeed)
    {
        if (sim.playerMode == modeTypes.Tour)
        {
            sim.tours.GuideCurrent.GetComponent<tourManager>().SetSpeed(argSpeed);
        }
    }

    //////////////
    //          //
    //  Notify  //
    //          //
    //////////////

    public void notify(string argSender, string argMessage, GameObject argSenderGO)
	{
		Debug.LogWarning ("Nav notification! : sender="+argSender+", message="+argMessage+"\n");

		//// ------ old tour code ------ ////
		
		if (argSender == "Infocube") {

			// --- Tour request --- //

			if(argMessage.Length >= 4 && argMessage.Substring (0,4) == "Tour"){
				Debug.LogWarning ("Searching for requested tour: "+argMessage+"\n");

				// activate tour...
			}
			
			if(argMessage == "End"){
				
				// and go back to the main menu
				sim.playerMode = modeTypes.Menu;
				cameraPiggyBack (networkIdFromName ("MainMenu"), new Vector3 (0, 0, 0), new Quaternion (0, 0, 0, 0), lockTransforms);
			}
			
		}
	}
	
	//////////////////////////
	//                      //
	//  fixPlayerTransform  //
	//                      //
	//////////////////////////
	
	[getReal3D.RPC]
	void fixPlayerTransform(Vector3 argNewPosition, Quaternion argNewRotation) {
		if(!getReal3D.Cluster.isMaster) {
			sim.camera.fpc.transform.position = argNewPosition;
			sim.camera.fpc.transform.rotation = argNewRotation;
		}
	}
	
	//////////////////////////////
	//                          //
	//  assignUniqueNetworkIds  //
	//                          //
	//////////////////////////////
	
	// IMPORTANT : This script is called from the custom inspector on the controller node IN EDITOR
	// This is so that the ids it generates are saved into the scene and baked into the builds
	// (thereby insuring the same id per object across the caves render node unity instances)
	
	// Also, ids are ONLY applied to objects that have the uniqueId component attached to them.
	
	public void assignUniqueNetworkIds()
	{
		// find all gameobjects with uniqueId components on them (WARNING: doesn't detect non active objects)
		uniqueId[] assignTargets = FindObjectsOfType (typeof(uniqueId)) as uniqueId[];
		
		// recreate the id ref registry to support the number of unique id components found
		sim.network.idRefs = new GameObject[assignTargets.Length];
		
		for (int i=0; i<assignTargets.Length; i++) {
			
			// push the index to the uniqueId component for use as a unique id
			assignTargets[i].customNetworkId = i;
			
			// and then also store a reference to the gameobject in the id ref registry
			sim.network.idRefs[i] = assignTargets[i].gameObject;
			
			// and a little LogWarning just to let us know
			Debug.LogWarning (i + ":" + sim.network.idRefs[i].name+"\n");
		}
	}
	
	/////////////////////////
	//                     //
	//  networkIdFromName  //
	//                     //
	/////////////////////////
	
	int networkIdFromName(string argName){
		
		int matchingId = -2; // not found (-1 is none)
		for(int i=0; i<sim.network.idRefs.Length; i++) {
			if(sim.network.idRefs[i].name == argName) { matchingId = i; }
		}
		return matchingId;
	}
	
	////////////////////////
	//                    //
	//  dockToPublicInput //
	//                    //
	////////////////////////
	
	void dockToPublicInput(int argRequestersId){
		
		Debug.LogWarning ("Input dock request from "+sim.network.idRefs[argRequestersId].name);
		bool idFound = false;
		
		// search through the public input dock to see if the dock requester is already docked
		for(int i=0; i<sim.input.dockPublic.Length; i++)
		{
			if(sim.input.dockPublic[i] == argRequestersId){ idFound = true; }
		}
		
		// if it's not already docked, dock it
		if(!idFound)
		{
			// look for an empty slot to dock (a reference to) the object into
			bool idPlaced = false;
			for(int i=0; i<sim.input.dockPublic.Length; i++)
			{
				// -1 is an empty dock space, if the object hasn't been already placed, put it here
				if(sim.input.dockPublic[i] == -1 && !idPlaced)
				{
					sim.input.dockPublic[i] = argRequestersId;
					idPlaced = true;
				}
			}

			// Fix for PC mode when near interactive objects : disable strafe and jump
			if(sim.cameraMode == cameraTypes.PC){
			
				// NO! Just choose better inputs! //
			
				// sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockSideways = true;
				sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockJump = true;
			}

		}
		
	}
	
	////////////////////////////
	//                        //
	//  undockFromPublicInput //
	//                        //
	////////////////////////////
	
	void undockFromPublicInput(int argRequestersId){
		
		for(int i=0; i<sim.input.dockPublic.Length; i++)
		{
			if(sim.input.dockPublic[i] == argRequestersId)
			{
				sim.input.dockPublic[i] = -1;

				// Fix for PC mode when moving away from interactive objects : re-enable strafe and jump
				if(sim.cameraMode == cameraTypes.PC){
				
					// NO! Just choose better inputs! //
				
					// sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockSideways = false;
					sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().m_LockJump = false;
				}
			}
		}
		
	}
	
	////////////////////////
	//                    //
	//  dockToWandInput //
	//                    //
	////////////////////////
	
	void dockToWandInput(int argRequestersId){
		
		Debug.LogWarning ("Wand dock request from "+sim.network.idRefs[argRequestersId].name);
		bool idFound = false;
		
		// search through the wand input dock to see if the dock requester is already docked
		for(int i=0; i<sim.input.dockWand.Length; i++)
		{
			if(sim.input.dockWand[i] == argRequestersId){ idFound = true; }
		}
		
		// if it's not already docked, dock it
		if(!idFound)
		{
			// look for an empty slot to dock (a reference to) the object into
			bool idPlaced = false;
			for(int i=0; i<sim.input.dockWand.Length; i++)
			{
				// -1 is an empty dock space, if the object hasn't been already placed, put it here
				if(sim.input.dockWand[i] == -1 && !idPlaced)
				{
					sim.input.dockWand[i] = argRequestersId;
					idPlaced = true;
				}
			}
						
		}
		
	}
	
	////////////////////////////
	//                        //
	//  undockFromWandInput //
	//                        //
	////////////////////////////
	
	void undockFromWandInput(int argRequestersId){
		
		for(int i=0; i<sim.input.dockWand.Length; i++)
		{
			if(sim.input.dockWand[i] == argRequestersId)
			{
				sim.input.dockWand[i] = -1;
			}
		}
		
	}
	
	////////////////////////////
	//                        //
	//  pushInputToGameObject //
	//                        //
	////////////////////////////
	
	void pushInputToGameObject(int argTargetObjectId, string argInput){
		
		if(sim.cameraMode == cameraTypes.Cave){
			getReal3D.RpcManager.call("pushInputToGameObject_rpc", argTargetObjectId, argInput);
		} else {
			pushInputToGameObject_rpc(argTargetObjectId, argInput);
		}
		
	}
	
	[getReal3D.RPC]
	void pushInputToGameObject_rpc(int argTargetObjectId, string argInput){
		
		// find the specified gameobject
		GameObject targetObject = sim.network.idRefs[argTargetObjectId];
		
		// send the input content
		targetObject.SendMessage("pushedInput",argInput);
		
	}
	
	/////////////////////////
	//                     //
	//  setLockTransforms  //
	//                     //
	/////////////////////////
	
	void setLockTransforms(bool argOn){
		
		// ...if using the cave camera
		if(sim.cameraMode == cameraTypes.Cave){
			if(argOn){
				Debug.LogWarning ("Freezing move paramaters\n");
				sim.camera.fpc.GetComponent<CsCharacterMotor>().enabled = false;
				sim.camera.fpc.GetComponent<getRealWalkthruController>().enabled = false;
				// sim.camera.fpc.GetComponent<getRealJoyLook>().enabled = false;
			} else {
				Debug.LogWarning ("Unfreezing move paramaters\n");
				sim.camera.fpc.GetComponent<CsCharacterMotor>().enabled = true;
				sim.camera.fpc.GetComponent<getRealWalkthruController>().enabled = true;
				// sim.camera.fpc.GetComponent<getRealJoyLook>().enabled = true;
			}
		}
		
		// ...if using the PC camera
		if(sim.cameraMode == cameraTypes.PC){
			if(argOn){
				Debug.LogWarning ("Freezing move paramaters\n");
				sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;

			} else {
				Debug.LogWarning ("Unfreezing move paramaters\n");
				sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			}
		}
		
		// ...if using the Immerse camera
		if(sim.cameraMode == cameraTypes.Immerse){
			if(argOn){
				Debug.LogWarning ("Freezing move paramaters\n");
				sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
				
			} else {
				Debug.LogWarning ("Unfreezing move paramaters\n");
				sim.camera.fpc.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			}
		}
	}

	/////////////////////////
	//                     //
	//  positionAnimation  //
	//                     //
	/////////////////////////
	
	void positionAnimation(int argTargetObjectId, float argSpeed, float argTimePosition){
		if(sim.cameraMode == cameraTypes.Cave){
			getReal3D.RpcManager.call("positionAnimation_rpc", argTargetObjectId, argSpeed, argTimePosition);
		} else {
			positionAnimation_rpc(argTargetObjectId, argSpeed, argTimePosition);
		}
	}	
	
	[getReal3D.RPC]
	void positionAnimation_rpc(int argTargetObjectId, float argSpeed, float argTimePosition){
		
		// derive gameobject references from the supplied Network View IDs
		GameObject targetObject = sim.network.idRefs[argTargetObjectId];
		
		// trigger requested animation
		targetObject.GetComponent<Animator>().speed = argSpeed;
		targetObject.GetComponent<Animator>().Play (0,0,argTimePosition);
		
	}

	///////////////////////
	//                   //
	//  cameraPiggyBack  //
	//                   //
	///////////////////////
	
	void cameraPiggyBack(int argTargetParentId, Vector3 argNewLocalPosition, Quaternion argNewLocalRotation, bool argLockTransforms)
	{
		if(sim.cameraMode == cameraTypes.Cave){
			getReal3D.RpcManager.call ("cameraPiggyBack_rpc", argTargetParentId, argNewLocalPosition, argNewLocalRotation, argLockTransforms);
		} else {
			cameraPiggyBack_rpc(argTargetParentId, argNewLocalPosition, argNewLocalRotation, argLockTransforms); 
		}
	}
	
	[getReal3D.RPC]
	void cameraPiggyBack_rpc(int argTargetParentId, Vector3 argNewLocalPosition, Quaternion argNewLocalRotation, bool argLockTransforms) { 
		
		GameObject targetParent = sim.network.idRefs[argTargetParentId];
		
        /*

		// derive gameobject references from the supplied targetParent_name
		if (sim.cameraMode == cameraTypes.Cave) {
			targetParent = GameObject.Find (sim.network.idRefs[argTargetParentId].name + "_GR3Doffset");
			if(!targetParent){ Debug.LogError ("No GR3doffset on requested camera! ("+sim.network.idRefs[argTargetParentId].name+")\n"); }
		} else {
			targetParent = GameObject.Find (sim.network.idRefs[argTargetParentId].name + "_PCoffset");
			if(!targetParent){ Debug.LogError ("No PCoffset on requested camera! ("+sim.network.idRefs[argTargetParentId].name+")\n"); }
		}
		
        */

		// lock/unlock camera transforms...
		setLockTransforms(argLockTransforms);
		
		// reparent camera
		sim.camera.fpc.transform.parent = targetParent.transform;
		sim.camera.fpc.transform.localPosition = argNewLocalPosition;
		sim.camera.fpc.transform.localRotation = argNewLocalRotation;
	}

    /////////////////////////
    //                     //
    //  changeEnvironment  //
    //                     //
    /////////////////////////

    void changeEnvironment(int argEnvironmentIndex){
		if(sim.cameraMode == cameraTypes.Cave){
			getReal3D.RpcManager.call ("changeEnvironment_rpc", argEnvironmentIndex);
		} else {
			changeEnvironment_rpc(argEnvironmentIndex); 
		}
	}
	
	[getReal3D.RPC]
	void changeEnvironment_rpc(int argEnvironmentIndex){
		
		for(int i=0; i<sim.environments.options.Length; i++){
			if(i == argEnvironmentIndex){
				sim.environments.options[i].SetActive (true);
				RenderSettings.skybox = sim.environments.options[i].GetComponent<environmentOptions>().skybox;
			} else {
				sim.environments.options[i].SetActive (false);
			}
		}
	}
	
    //////////////////
    //              //
    //  toggleMenu  //
    //              //
    //////////////////

    void toggleMenu(bool argTurnMenuOn)
    {
        if (sim.cameraMode == cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("toggleMenu_rpc", argTurnMenuOn);
        }
        else
        {
            toggleMenu_rpc(argTurnMenuOn);
        }
    }

    [getReal3D.RPC]
    void toggleMenu_rpc(bool argTurnMenuOn)
    {
        if (sim.menus.simpleMenu.main.root)
        {
            sim.menus.simpleMenu.main.root.SetActive(argTurnMenuOn);
        }

        Debug.LogWarning("Menu on: " + argTurnMenuOn + "\n");

    }


    ////////////////////////////
    //                        //
    //  setPlaybackIndicator  //
    //                        //
    ////////////////////////////

    void setPlaybackIndicator(int argSpeed)
    {
        if (sim.cameraMode == cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("setPlaybackIndicator_rpc", argSpeed);
        }
        else
        {
            setPlaybackIndicator_rpc(argSpeed);
        }
    }

    [getReal3D.RPC]
    void setPlaybackIndicator_rpc(int argSpeed)
    {
        Material[] temp = sim.tours.playbackIndicator.GetComponent<Renderer>().materials;

        if (argSpeed == -2) { temp[0].mainTexture = sim.tours.playbackStates[0]; }
        if (argSpeed == -1) { temp[0].mainTexture = sim.tours.playbackStates[1]; }
        if (argSpeed ==  0) { temp[0].mainTexture = sim.tours.playbackStates[2]; }
        if (argSpeed ==  1) { temp[0].mainTexture = sim.tours.playbackStates[3]; }
        if (argSpeed ==  2) { temp[0].mainTexture = sim.tours.playbackStates[4]; }

    }
	////////////////////////
	//                    //
	//  changeContentSet  //
	//                    //
	////////////////////////
	
	void changeContentSet(int argContentSetIndex){
		if(sim.cameraMode == cameraTypes.Cave){
			getReal3D.RpcManager.call ("changeContentSet_rpc", argContentSetIndex);
		} else {
			changeContentSet_rpc(argContentSetIndex); 
		}
	}
	
	[getReal3D.RPC]
	void changeContentSet_rpc(int argContentSetIndex){
		
		for(int i=0; i<sim.contentSets.options.Length; i++){
			if(i == argContentSetIndex){
				sim.contentSets.options[i].SetActive (true);
			} else {
				sim.contentSets.options[i].SetActive (false);
			}
		}
	}
	
	//////////////////////////////////
	//                              //
	//  broadcastGridUI_transforms  //
	//                              //
	//////////////////////////////////
	
	void broadcastGridUI_transforms(){
		if(sim.cameraMode == cameraTypes.Cave){
			if(getReal3D.Cluster.isMaster){
				getReal3D.RpcManager.call (
					"broadcastGridUI_transforms_rpc", 
					sim.menus.GridUI.pos.x, sim.menus.GridUI.pos.y, sim.menus.GridUI.pos.z, 
					sim.menus.GridUI.rot.x, sim.menus.GridUI.rot.y, sim.menus.GridUI.rot.z, 
					sim.menus.GridUI.siz.x, sim.menus.GridUI.siz.y, sim.menus.GridUI.siz.z
				);
			}
		}
	}
	
	[getReal3D.RPC]
	void broadcastGridUI_transforms_rpc(float argPosX, float argPosY, float argPosZ, float argRotX, float argRotY, float argRotZ, float argSizX, float argSizY, float argSizZ){
		if(!getReal3D.Cluster.isMaster){
		
			sim.menus.GridUI.pos.x = argPosX;
			sim.menus.GridUI.pos.y = argPosY;
			sim.menus.GridUI.pos.z = argPosZ;
			
			Vector3 newRotation = new Vector3(argRotX, argRotY, argRotZ);
			sim.menus.GridUI.rot = newRotation;
			
			sim.menus.GridUI.siz.x = argSizX;
			sim.menus.GridUI.siz.y = argSizY;
			sim.menus.GridUI.siz.z = argSizZ;
		}
	}
	
}
