using UnityEngine;
using System.Collections;

public class lookAtManager : MonoBehaviour {

    public GameObject controllerGO;
    public GameObject guide;
    public GameObject target;
    public GameObject player;

    public bool active = false;

    public float colliderSize;
    public float playerDistance;
    public float incursionPercent;
    public AnimationCurve influenceCurve;

    public float influence=0;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (active)
        {
            playerDistance = Vector3.Distance(player.transform.position, this.transform.position);
            incursionPercent = 1 - (playerDistance / colliderSize);
            influence = influenceCurve.Evaluate(incursionPercent);
        }
        else
        {
            influence = 0;
        }


	}

    ///////////////////////////
    //                       //
    //  Proximity Detection  //
    //                       //
    ///////////////////////////

    void OnTriggerEnter(Collider argOther)
    {

        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
        {
            // if we're in the cave and this is NOT the head node, don't do anything...
        }
        else
        {
            // Otherwise (head node in cave or just normal PC mode) go for it

            if (argOther.tag == "Player")
            {
                Debug.LogWarning("Player has approached a look at manager\n");
                active = true;
                player = argOther.gameObject;
                colliderSize = Vector3.Distance(player.transform.position, this.transform.position);
            }
        }

    }

    void OnTriggerExit(Collider argOther)
    {

        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
        {
            // if we're in the cave and this is NOT the head node, don't do anything...
        }
        else
        {
            // Otherwise (head node in cave or just normal PC mode) go for it

            if (argOther.tag == "Player")
            {
                Debug.LogWarning("Player has moved away from a look at manager\n");
                active = false;
            }
        }

    }
}
