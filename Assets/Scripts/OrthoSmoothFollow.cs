using UnityEngine;
using System.Collections;

public class OrthoSmoothFollow : MonoBehaviour
{

    public Transform target;
    public Transform car;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    public Vector3 cameraRelative;
    private Vector3 initialPos;

    private void Awake()
    {
        initialPos = transform.position;
        cameraRelative = car.InverseTransformPoint(initialPos);
        //cameraRelative = initialPos - transform.position;
        Debug.Log(cameraRelative);
    }

    void FixedUpdate()
    {
        target.eulerAngles = new Vector3 (0, 0, 0);
        //cameraRelative = target.InverseTransformPoint(transform.position);
        //Vector3 goalPos = target.position;
        //goalPos.y = transform.position.z;
        //cameraRelative = target.InverseTransformPoint(initialPos);
        Debug.Log(target.TransformPoint(cameraRelative));
        transform.position = Vector3.SmoothDamp(transform.position, target.TransformPoint(cameraRelative), ref velocity, smoothTime);
    }
}