using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text speedText;

    private float zVel;
    private Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        zVel = transform.InverseTransformDirection(rb.velocity).z;

        if (speedText != null)
            speedText.text = "Speed: " + (zVel * 3.6f).ToString("f0") + "km/h";
    }
}
