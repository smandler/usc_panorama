using UnityEngine;
using System.Collections;

public class controllerNotify : MonoBehaviour {

	public controller controllerGO;
	public string sender;

	// send message
	void SendMessage(string argMessage) {
		Debug.LogWarning ("sending message: " + argMessage + "\n");
		controllerGO.notify (sender, argMessage, this.gameObject);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
