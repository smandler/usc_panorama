using UnityEngine;
using System.Collections;

public class CurvedUI_RPCs : getReal3D.MonoBehaviourWithRpc {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    ////////////////////////////////////////////
    //                                        //
    //  rpc_CurvedUI_broadcastOnPointerEnter  //
    //                                        //
    ////////////////////////////////////////////

    [getReal3D.RPC]
	public void rpc_CurvedUI_broadcastOnPointerEnter(int argCustomNetworkId)
	{
		GameObject menuItem = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(menuItem) { menuItem.GetComponent<CUI_ZChangeOnHover>().OnPointerEnter_rpc(); }
	}

    ///////////////////////////////////////////
    //                                       //
    //  rpc_CurvedUI_broadcastOnPointerExit  //
    //                                       //
    ///////////////////////////////////////////

    [getReal3D.RPC]
    public void rpc_CurvedUI_broadcastOnPointerExit(int argCustomNetworkId)
    {
        GameObject menuItem = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
        if (menuItem) { menuItem.GetComponent<CUI_ZChangeOnHover>().OnPointerExit_rpc(); }
    }
}
