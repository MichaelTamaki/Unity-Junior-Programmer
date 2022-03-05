using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxTorque = 400f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject hoodCamera;
    private bool isMainCam = true;
    private Rigidbody playerRb;
    [SerializeField] private GameObject centerOfMassObj;
    [SerializeField] private CarAxle[] carAxles;
    [SerializeField] private TextMeshProUGUI speedometer;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerRb.centerOfMass = centerOfMassObj.transform.localPosition;
        setCameraActive();
    }

    private void FixedUpdate()
    {
        float torque = maxTorque * Input.GetAxis("Vertical");
        float steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
        foreach (CarAxle carAxle in carAxles)
        {
            if (carAxle.isSteering)
            {
                carAxle.leftWheel.steerAngle = steerAngle;
                carAxle.rightWheel.steerAngle = steerAngle;
            }
            if (carAxle.isMotor)
            {
                carAxle.leftWheel.motorTorque = torque;
                carAxle.rightWheel.motorTorque = torque;
            }
            ApplyLocalPositionToVisuals(carAxle.leftWheel);
            ApplyLocalPositionToVisuals(carAxle.rightWheel);
        }
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isMainCam = !isMainCam;
            setCameraActive();
        }

        int speed = Mathf.RoundToInt(playerRb.velocity.magnitude * 2.237f);
        speedometer.text = "Speed: " + speed + "mph";
    }

    private void setCameraActive()
    {
        mainCamera.gameObject.SetActive(isMainCam);
        mainCamera.SetActive(isMainCam);
        hoodCamera.gameObject.SetActive(!isMainCam);
        hoodCamera.SetActive(!isMainCam);
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
}

[System.Serializable]
public class CarAxle
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool isSteering;
    public bool isMotor;
}

