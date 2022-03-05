using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject mainCameraObj;
    [SerializeField] private GameObject aimCameraObj;
    [SerializeField] private GameObject ballObj;
    private GameController gameController;
    private bool isFollowingBall = false;
    private Quaternion mainCamRotation;
    private Quaternion aimCameraRotation;

    private void Start()
    {
        mainCamRotation = mainCameraObj.transform.rotation;
        aimCameraRotation = aimCameraObj.transform.rotation;
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        ToggleCamera(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && gameController.gameState == GameStateEnum.Shooting)
        {
            ToggleCamera();
        }
    }

    private void LateUpdate()
    {
        if (isFollowingBall)
        {
            Vector3 ballPos = ballObj.transform.position;
            foreach (GameObject camObj in new GameObject[] { mainCameraObj, aimCameraObj })
            {
                Vector3 targetDirection = ballPos - camObj.transform.position;
                Vector3 intermediateDirection = Vector3.RotateTowards(camObj.transform.forward, targetDirection, 0.8f * Time.deltaTime, 0.0f);
                camObj.transform.rotation = Quaternion.LookRotation(intermediateDirection);
            }
        }
    }

    public void ToggleCamera()
    {
        bool isMain = !mainCameraObj.activeSelf;
        ToggleCamera(isMain);
    }

    public void ToggleCamera(bool isMain)
    {
        mainCameraObj.SetActive(isMain);
        aimCameraObj.SetActive(!isMain);
    }

    public void EnableCameraIsFollowingBall()
    {
        isFollowingBall = true;
    }
    public void DisableCameraIsFollowingBall()
    {
        isFollowingBall = false;
        mainCameraObj.transform.rotation = mainCamRotation;
        aimCameraObj.transform.rotation = aimCameraRotation;
    }

}
