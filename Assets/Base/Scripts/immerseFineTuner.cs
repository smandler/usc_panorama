using UnityEngine;
using System.Collections;

public class immerseFineTuner : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown (KeyCode.R)){
			transform.Translate(Vector3.forward * Time.deltaTime / 5);
		}
		
		if(Input.GetKeyDown (KeyCode.F)){
			transform.Translate(Vector3.back * Time.deltaTime / 5);
		}
		
		if(Input.GetKeyDown (KeyCode.Q)){
			transform.localScale += new Vector3(0.01F,0,0);
		}
		
		if(Input.GetKeyDown (KeyCode.E)){
			transform.localScale += new Vector3(-0.01F,0,0);
		}
		
	}
}