using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CUI_ZChangeOnHover : getReal3D.MonoBehaviourWithRpc, IPointerEnterHandler, IPointerExitHandler {
    public GameObject controllerGO;

	public float restZ = 0;
	public float OnHoverZ = -50;

	bool Zoomed = false;

	// Update is called once per frame
	void Update () {
	
		(transform as RectTransform).anchoredPosition3D = (transform as RectTransform).anchoredPosition3D.ModifyZ(Mathf.Clamp((Zoomed ? 
			(transform as RectTransform).anchoredPosition3D.z + Time.deltaTime * (OnHoverZ - restZ) * 6 :
			(transform as RectTransform).anchoredPosition3D.z - Time.deltaTime * (OnHoverZ - restZ) * 6), OnHoverZ, restZ));

	}

	public void  OnPointerEnter (PointerEventData eventData){
        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("rpc_CurvedUI_broadcastOnPointerEnter", this.GetComponent<uniqueId>().customNetworkId);
        }
        else
        {
            OnPointerEnter_rpc();
        }
    }
    public void OnPointerEnter_rpc()
    {
		Zoomed = true;
	
	}

	public void  OnPointerExit (PointerEventData eventData){
        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave)
        {
            getReal3D.RpcManager.call("rpc_CurvedUI_broadcastOnPointerExit", this.GetComponent<uniqueId>().customNetworkId);
        }
        else
        {
            OnPointerExit_rpc();
        }
    }
    public void OnPointerExit_rpc()
    {
		Zoomed = false;
	}
}
