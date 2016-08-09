using UnityEngine;
using System.Collections;

public class tourManager : MonoBehaviour {

    public GameObject controllerGO;

	public Spline route;
    public bool active = false;
    public bool playing = false;

    public float speedRX2 = -0.5f;
    public float speedRX1 = -0.1f;
    public float speedNil =  0.0f;
    public float speedFX1 =  0.1f;
    public float speedFX2 =  0.5f;
    public float speedBase = 0.1f;
    public AnimationCurve speedCurve;

    public AnimationCurve aheadCurve;

    public float scrubSpeed = 10;

	public float passedTime = 0.0f;
    public float loopedTime = 0.0f;
    public float aheadTime = 0.0f;

    public GameObject[] lookAts;

    public GameObject debugObject;

    public Vector3 camPos;
    public Quaternion camQuat;

    public Vector3 aheadPos;
    public Vector3 modPos;

    public int customNetworkId = -1;

	// Use this for initialization
	void Start ()
	{
        customNetworkId = this.GetComponent<uniqueId>().customNetworkId;
	}

    // ONLY called on head node
    public void Play()
    {
        if (controllerGO.GetComponent<controller>().sim.cameraMode == controller.cameraTypes.Cave && !getReal3D.Cluster.isMaster)
        {
            // if this is a render node in the cave, don't start the guide playing (only want the head node to calculate spline positions)
        }
        else
        {
            active  = true;
            playing = true;
        }
    }

    public void Stop()
    {
        active  = false;
        playing = false;
    }
	
	// Update is called once per frame
	void Update ()
	{
        // get speed for current point in time from the variableSpeed animation curve
        float speedCurveMod = speedCurve.Evaluate(loopedTime);

        // calculate passed time based on all of the tweak settings
        if (playing) { passedTime += (Time.deltaTime * speedBase * speedCurveMod) * scrubSpeed; }

        // also calculate a looped version of passed time
        loopedTime = (passedTime / scrubSpeed) % 1;

        // manage aheadTime (default lookAt pointer)
        aheadTime = loopedTime + aheadCurve.Evaluate(loopedTime);
        if(aheadTime > 1.0f) { aheadTime = 1.0f; }

        // if the tour is active, calculate and push the transform for the tour to the controller
        if (active)
        {
            // calculate the base position and orientation on the spline (based on looped time)
            camPos = route.GetPositionOnSpline(loopedTime);
            camQuat = route.GetOrientationOnSpline(loopedTime);

            // calculate a position ahead of the camera (to act as the default look at target)
            aheadPos = route.GetPositionOnSpline(aheadTime);
            modPos = aheadPos;

            // calculate dynamic look at position from blend of aheadPos and any in range look at modifiers
            for(int i=0; i<lookAts.Length; i++)
            {
                // figure out facing rotation from this position to current lookAts target
                Vector3 targetPos = lookAts[i].GetComponent<lookAtManager>().target.transform.position;
                Vector3 targetDeltaPos = targetPos - modPos;
                Vector3 targetDeltaPosBlended = targetDeltaPos * lookAts[i].GetComponent<lookAtManager>().influence;

                modPos += targetDeltaPosBlended;
            }

            // use debug object to show the end result of all the look at calculations - THIS IS WORKING
            debugObject.transform.position = modPos;

            // send the camera position and the (blended) lookAt position to the controller for distribution
            controllerGO.GetComponent<controller>().tourPushTransform(customNetworkId, camPos, modPos);
        }
	}

    // this is the transformer called by the RPC broadcast (or directly if on PC)
    public void SetTransform(Vector3 argPosition, Vector3 argLookAtPosition)
    {
        transform.position = argPosition;

        transform.LookAt(argLookAtPosition);
    }
    public void SetSpeed(int argSpeedSetting)
    {
        if (argSpeedSetting == -2) { speedBase = speedRX2; }
        if (argSpeedSetting == -1) { speedBase = speedRX1; }
        if (argSpeedSetting ==  0) { speedBase = speedNil; }
        if (argSpeedSetting ==  1) { speedBase = speedFX1; }
        if (argSpeedSetting ==  2) { speedBase = speedFX2; }
    }
}
