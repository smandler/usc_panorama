using UnityEngine;
using System.Collections;

public class menuActionNotify : MonoBehaviour {

	public GameObject controllerGO;
	public string menu;
	public string message;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// activate menu option
	void activateMenuOption(){
		controllerGO.GetComponent<controller>().notify(menu, message, this.gameObject);
	}
}
