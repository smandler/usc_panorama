using UnityEngine;
using System.Collections;

public class modelRPCs : getReal3D.MonoBehaviourWithRpc {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	///////////////////////////
	//                       //
	//  model_broadcastTurn  //
	//                       //
	///////////////////////////
	
	[getReal3D.RPC]
	public void rpc_model_broadcastTurn(int argCustomNetworkId, float argModelTurnActual)
	{
		GameObject modelTurntable = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(modelTurntable) { modelTurntable.GetComponent<modelManager>().setTurn_rpc(argModelTurnActual); }
	}
	
	///////////////////////////
	//                       //
	//  model_broadcastLerp  //
	//                       //
	///////////////////////////
	
	[getReal3D.RPC]
	public void rpc_model_broadcastLerp(int argCustomNetworkId, Vector3 argLerp)
	{
		GameObject model = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(model) { model.GetComponent<modelManager>().setLerp_rpc(argLerp); }
	}
	
	///////////////////////////
	//                       //
	//  model_broadcastZoom  //
	//                       //
	///////////////////////////
	
	[getReal3D.RPC]
	public void rpc_model_broadcastZoom(int argCustomNetworkId, bool argZoom)
	{
		GameObject model = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(model) { model.GetComponent<modelManager>().setZoom_rpc(argZoom); }
	}
	
	
	////////////////////////////////////
	//                                //
	//  rpc_model_broadcastHighlight  //
	//                                //
	////////////////////////////////////
	
	[getReal3D.RPC]
	public void rpc_model_broadcastHighlight(int argCustomNetworkId, bool argActive)
	{
		GameObject modelManager = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(modelManager) { modelManager.GetComponent<modelManager>().highlightModel_rpc(argActive); }
	}
}
