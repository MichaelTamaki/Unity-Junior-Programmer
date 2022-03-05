using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject focalPointObj;
    public float rotateSpeed = 75.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(focalPointObj.transform.position, Vector3.up, rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));    
    }
}
