using UnityEngine;
using System.Collections;

public class menuManager : MonoBehaviour {	
	
	// connection to the mothership
	public GameObject controllerGO;
	
	// input injection
	string pushedInputChar;
	
	// menu references
	public GameObject menuRoot;
	public GameObject menuMarker;
	public GameObject menuItems;
	Transform[] menuItemTransforms;
	public int menuSelectionIndex = 1;
	int menuItemCount;
	
	/////////////
	//         //
	//  Start  //
	//         //
	/////////////
	
	void Start () {
		menuItemCount = menuItems.transform.childCount;
		menuItemTransforms = menuItems.GetComponentsInChildren<Transform>();
	}
	
	///////////////////
	//               //
	//  pushedInput  //
	//               //
	///////////////////
	
	// We have to dead drop the input to the update function because... "unity"
	
	void pushedInput(string argInput){
		pushedInputChar = argInput;
	}
	
	//////////////
	//          //
	//  Update  //
	//          //
	//////////////
	
	void Update () {
		
		// process any relevant input		
		if(pushedInputChar != ""){
			
			//// ----- input : up ----- ////
			
			if(pushedInputChar == "U"){
				
				if(menuSelectionIndex > 1)
				{
					menuSelectionIndex -= 1;
					moveMarkerTo (menuSelectionIndex);
				}
				pushedInputChar = "";
			}
			
			//// ----- input : down ----- ////
			
			if(pushedInputChar == "D"){
				
				if(menuSelectionIndex < (menuItemCount))
				{		
					menuSelectionIndex += 1;
					moveMarkerTo (menuSelectionIndex);
				}
				pushedInputChar = "";
			}
			
			//// ----- input : select ----- ////
			
			if(pushedInputChar == "S"){
				menuItemTransforms[menuSelectionIndex].gameObject.SendMessage ("activateMenuOption");
				pushedInputChar = "";
			}
			
		}
	}
	
	////////////////////
	//                //
	//  moveMarkerTo  //
	//                //
	////////////////////
	
	void moveMarkerTo(int argTargetItemIndex){
		if(controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave){
			getReal3D.RpcManager.call("rpc_menu_broadcastMarkerMove", this.GetComponent<uniqueId>().customNetworkId, argTargetItemIndex);
		} else {
			moveMarkerTo_rpc(argTargetItemIndex);
		}
	}
	
	public void moveMarkerTo_rpc(int argTargetItemIndex){
		Vector3 newMarkerPosition = menuItemTransforms[argTargetItemIndex].localPosition;
		menuMarker.transform.localPosition = newMarkerPosition;
	}
}
