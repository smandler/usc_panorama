using UnityEngine;
using System.Collections;

public class infoCubeRPCs : getReal3D.MonoBehaviourWithRpc {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	///////////////////////////////////
	//                               //
	//  infoCube_broadcastSlideOpen  //
	//                               //
	///////////////////////////////////
	
	[getReal3D.RPC]
	public void rpc_infoCube_broadcastSlideOpen(int argCustomNetworkId, float argInfoCubeOpenActual)
	{
		GameObject infoCube = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(infoCube) { infoCube.GetComponent<infoCubeManager>().setSlideOpen_rpc(argInfoCubeOpenActual); }
	}
	
	///////////////////////////////////
	//                               //
	//  infoCube_broadcastSlideTurn  //
	//                               //
	///////////////////////////////////
	
	[getReal3D.RPC]
	public void rpc_infoCube_broadcastSlideTurn(int argCustomNetworkId, float argInfoCubeTurnActual)
	{
		GameObject infoCube = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(infoCube) { infoCube.GetComponent<infoCubeManager>().setSlideTurn_rpc(argInfoCubeTurnActual); }
	}
	
	///////////////////////////////////////
	//                                   //
	//  infoCube_broadcastSlideTextures  //
	//                                   //
	///////////////////////////////////////
	
	[getReal3D.RPC]
	public void rpc_infoCube_broadcastSlideTextures(int argCustomNetworkId, int argCurrentPanel, int argFrontSlideIndexToLoad, string argDirection)
	{
		GameObject infoCube = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if(infoCube) { infoCube.GetComponent<infoCubeManager>().setSlideTextures_rpc(argCurrentPanel, argFrontSlideIndexToLoad, argDirection); }
	}

	//////////////////////////////
	//                          //
	//  infoCube_broadcastMove  //
	//                          //
	//////////////////////////////

	[getReal3D.RPC]
    public void rpc_infoCube_broadcastMove(int argCustomNetworkId, Vector3 argNewPosition)
    {
		GameObject infoCube = this.GetComponent<controller>().sim.network.idRefs[argCustomNetworkId];
		if (infoCube) { infoCube.GetComponent<infoCubeManager>().setPosition_rpc(argNewPosition); }
	}

	//////////////////////////////
	//                          //
	//  infoCube_broadcastTest  //
	//                          //
	//////////////////////////////

	[getReal3D.RPC]
	public void rpc_infoCube_broadcastTest(string argText)
	{
		Debug.LogWarning ("broadcastTest successfully called");
	}
}
