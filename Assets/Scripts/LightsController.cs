using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class LightsController : MonoBehaviour {

    public Light BrakeL, BrakeR, HeadlightL, HeadlightR;
    public WheelCollider frontWheel;
    private float initialBLIntensity;
    private bool highBeams;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    public float lowBeamAngle = 30.0f;
    private Vector3 lowBeams;

    // Use this for initialization
    void Start () {
        initialBLIntensity = BrakeL.intensity;
        highBeams = false;

        lowBeams = new Vector3(lowBeamAngle, 0, 0);
        HeadlightL.transform.localEulerAngles = lowBeams;
        HeadlightR.transform.localEulerAngles = lowBeams;
    }
	
	// Update is called once per frame
	void Update () {

        prevState = state;
        state = GamePad.GetState(playerIndex);

        if (frontWheel.brakeTorque > 0)
        {
            BrakeL.intensity = initialBLIntensity * 2;
            BrakeR.intensity = initialBLIntensity * 2;
        }
        else
        {
            BrakeL.intensity = initialBLIntensity;
            BrakeR.intensity = initialBLIntensity;
        }

        if(prevState.Buttons.LeftStick == ButtonState.Released && state.Buttons.LeftStick == ButtonState.Pressed)
        {
            highBeams = !highBeams;

            if (highBeams == true)
            {
                HeadlightL.transform.localEulerAngles = Vector3.zero;
                HeadlightR.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                HeadlightL.transform.localEulerAngles = lowBeams;
                HeadlightR.transform.localEulerAngles = lowBeams;
            }
        }

        
    }
}
