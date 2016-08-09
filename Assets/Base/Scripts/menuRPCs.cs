using UnityEngine;
using System.Collections;

public class menuRPCs : getReal3D.MonoBehaviourWithRpc {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	////////////////////////////////
	//                            //
	//  menu_broadcastMarkerMove  //
	//                            //
	////////////////////////////////
	
	[getReal3D.RPC]
	public void rpc_menu_broadcastMarkerMove(int argCustomNetworkId, int argTargetItemIndex)
	{
		GameObject menu = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(menu) { menu.GetComponent<menuManager>().moveMarkerTo_rpc(argTargetItemIndex); }
	}
}
