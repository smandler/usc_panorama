using UnityEngine;
using System.Collections;

public class Follow_Lat_Pos : MonoBehaviour {


    public GameObject Arrow_Position;

	// Use this for initialization
	void Start ()
    
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {

        Vector3 NewPosition = new Vector3(this.transform.position.x, Arrow_Position.transform.position.y, this.transform.position.z);

        this.transform.position = NewPosition;

	}
}
