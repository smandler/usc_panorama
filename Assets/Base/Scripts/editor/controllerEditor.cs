using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(controller))]
public class controllerEditor : Editor {
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		
		// get a handle on the target script
		controller controllerScript = (controller)target;

		// Make a button to setup the camera system
		if (GUILayout.Button ("Assign unique network ids")) { controllerScript.assignUniqueNetworkIds(); }
	}
}
