using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class CarMovement : MonoBehaviour {

    public enum DriveMode { Front, Rear, All };

    [Header("Car Tuning")]
    [Tooltip("Choose which wheels force is applied to.")]
    public DriveMode driveMode = DriveMode.Rear;
    [Tooltip("Wheel RPM at which the maximum motor force is applied.")]
    [Range(10f, 800f)]
    public float idealRPM = 50f;
    [Tooltip("Maximum wheel RPM. Determines top speed.")]
    [Range(400f, 2000f)]
    public float maxRPM = 800f;
    [Tooltip("Maximum forward force applied to wheels.")]
    [Range(1000f, 8000f)]
    public float forwardForce = 5000f;
    [Tooltip("Maximum reverse force applied to wheels.")]
    [Range(1000f, 8000f)]
    public float reverseForce = 4000f;
    [Tooltip("Maximum braking force applied to all wheels.")]
    [Range(1000f, 5000f)]
    public float brakeForce = 2000f;
    [Tooltip("Handbrake force applied to rear wheels")]
    [Range(1000f, 10000f)]
    public float handbrakeForce = 8000f;
    [Tooltip("Angle front wheels can turn to.")]
    [Range(1, 45)]
    public int turnAngle = 25;
    [Tooltip("Anti-roll bar strength, determines amount of body roll.")]
    [Range(100f, 10000f)]
    public float antiRoll = 1000f;
    [Tooltip("Rear wheel modifier for handbrakes to make it have less grip. Should be less than 1.")]
    [Range(0f, 1f)]
    public float rearSlipMultiplier = 0.5f;
    //[Tooltip("Amount of time to wait when braking to apply accelerator.")]
    //[Range(0f, 1f)]
    //public float brakeWaitTime = 0.5f;

    [Header("Wheel Colliders")]
    public WheelCollider wheelColFR;
    public WheelCollider wheelColFL;
    public WheelCollider wheelColRR;
    public WheelCollider wheelColRL;

    [Header("Car Center Of Gravity")]
    [Tooltip("Attach empty GameObject, is used to set centre of mass.")]
    public Transform centerOfGravity;

    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private Rigidbody rb;
    private WheelFrictionCurve normalRearCurve;
    private WheelFrictionCurve handbrakeCurve;
    private float zVel;
    //private CharacterController controller;
    //private float countdown;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfGravity.localPosition;

        normalRearCurve = wheelColRR.sidewaysFriction;
        handbrakeCurve = wheelColRR.sidewaysFriction;
        handbrakeCurve.stiffness = normalRearCurve.stiffness * 0.75f;

        //controller = GetComponent<CharacterController>();

    }

    public float Speed()
    {
        return wheelColRR.radius * Mathf.PI * Rpm() * 60f / 1000f;
    }

    public float Rpm()
    {
        return (wheelColRR.rpm + wheelColRL.rpm) / 2;
    }

    // Update is called once per frame
    void FixedUpdate () {

        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);

        zVel = (transform.InverseTransformDirection(rb.velocity).z) * 3.6f;
        //zVel = rb.velocity.magnitude * 3.6f;
        //zVel = controller.velocity.magnitude;

        float torque = state.Triggers.Right * forwardForce;
        float steeringAngle = state.ThumbSticks.Left.X * turnAngle;
        float braking = state.Triggers.Left * brakeForce;
        float reverseTorque = state.Triggers.Left * reverseForce;
        float reverseBraking = state.Triggers.Right * brakeForce;


        if (Rpm() < idealRPM)
        {
            torque = Mathf.Lerp(torque / 10f, torque, Rpm() / idealRPM);
            reverseTorque = Mathf.Lerp(reverseTorque / 10f, reverseTorque, Rpm() / idealRPM);
        }
            
        else{
            torque = Mathf.Lerp(torque, 0, (Rpm() - idealRPM) / (maxRPM - idealRPM));
            reverseTorque = Mathf.Lerp(reverseTorque, 0, (Rpm() - idealRPM) / (maxRPM - idealRPM));
        }

        

        wheelColFL.steerAngle = steeringAngle;
        wheelColFR.steerAngle = steeringAngle;

        //wheelColRL.steerAngle = -steeringAngle;
        //wheelColRR.steerAngle = -steeringAngle;




        if (zVel >= 0f)
        {
            wheelColFR.motorTorque = driveMode == DriveMode.Rear ? 0 : torque;
            wheelColFL.motorTorque = driveMode == DriveMode.Rear ? 0 : torque;
            wheelColRR.motorTorque = driveMode == DriveMode.Front ? 0 : torque;
            wheelColRL.motorTorque = driveMode == DriveMode.Front ? 0 : torque;

            wheelColRL.brakeTorque = braking;
            wheelColRR.brakeTorque = braking;
            wheelColFL.brakeTorque = braking;
            wheelColFR.brakeTorque = braking;

        }
        else if (zVel < 0f)
        {
            wheelColFR.motorTorque = driveMode == DriveMode.Rear ? 0 : -reverseTorque;
            wheelColFL.motorTorque = driveMode == DriveMode.Rear ? 0 : -reverseTorque;
            wheelColRR.motorTorque = driveMode == DriveMode.Front ? 0 : -reverseTorque;
            wheelColRL.motorTorque = driveMode == DriveMode.Front ? 0 : -reverseTorque;

            wheelColRL.brakeTorque = reverseBraking;
            wheelColRR.brakeTorque = reverseBraking;
            wheelColFL.brakeTorque = reverseBraking;
            wheelColFR.brakeTorque = reverseBraking;
        }
        //else
        //{
        //    wheelColFR.motorTorque = driveMode == DriveMode.Rear ? 0 : torque;
        //    wheelColFL.motorTorque = driveMode == DriveMode.Rear ? 0 : torque;
        //    wheelColRR.motorTorque = driveMode == DriveMode.Front ? 0 : torque;
        //    wheelColRL.motorTorque = driveMode == DriveMode.Front ? 0 : torque;

        //    wheelColFR.motorTorque += driveMode == DriveMode.Rear ? 0 : -reverseTorque;
        //    wheelColFL.motorTorque += driveMode == DriveMode.Rear ? 0 : -reverseTorque;
        //    wheelColRR.motorTorque += driveMode == DriveMode.Front ? 0 : -reverseTorque;
        //    wheelColRL.motorTorque += driveMode == DriveMode.Front ? 0 : -reverseTorque;
        //}


        if (state.Buttons.B == ButtonState.Pressed)
        {
            wheelColRL.brakeTorque += handbrakeForce;
            wheelColRR.brakeTorque += handbrakeForce;
            wheelColRR.sidewaysFriction = handbrakeCurve;
            wheelColRL.sidewaysFriction = handbrakeCurve;

        }
        else
        {
            wheelColRL.brakeTorque += 0;
            wheelColRR.brakeTorque += 0;
            wheelColFL.brakeTorque += 0;
            wheelColFR.brakeTorque += 0;
            wheelColRR.sidewaysFriction = normalRearCurve;
            wheelColRL.sidewaysFriction = normalRearCurve;
        }


        DoRollBar(wheelColFR, wheelColFL);
        DoRollBar(wheelColRR, wheelColRL);

        

    }

    void DoRollBar(WheelCollider WheelL, WheelCollider WheelR)
    {
        WheelHit hit;
        float travelL = 1.0f;
        float travelR = 1.0f;

        bool groundedL = WheelL.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-WheelL.transform.InverseTransformPoint(hit.point).y - WheelL.radius) / WheelL.suspensionDistance;

        bool groundedR = WheelR.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-WheelR.transform.InverseTransformPoint(hit.point).y - WheelR.radius) / WheelR.suspensionDistance;

        float antiRollForce = (travelL - travelR) * antiRoll;

        if (groundedL)
            rb.AddForceAtPosition(WheelL.transform.up * -antiRollForce,
                                         WheelL.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(WheelR.transform.up * antiRollForce,
                                         WheelR.transform.position);
    }
}
