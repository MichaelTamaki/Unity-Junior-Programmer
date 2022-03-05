using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private const float kickForce = 13f;
    private const float turnAngleSpeed = 20f;

    [SerializeField] private GameObject arrowObj;
    [SerializeField] private GameObject[] visibleIndicatorObjs;
    [SerializeField] private ParticleSystem goalExplosion;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private Rigidbody ballRb;
    private GameController gameController;
    private AudioController audioController;
    private GoalieController goalieController;
    private Vector3 shotRotationAngles = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        spawnPosition = gameObject.transform.position;
        spawnRotation = gameObject.transform.rotation;
        ballRb = GetComponent<Rigidbody>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
        goalieController = GameObject.FindGameObjectWithTag("Goalie").GetComponent<GoalieController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime * turnAngleSpeed;
        float verticalInput = Input.GetAxis("Vertical") * Time.deltaTime * turnAngleSpeed;
        if (gameController.gameState != GameStateEnum.Shooting)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ballRb.AddForce(Quaternion.Euler(shotRotationAngles) * Vector3.forward * kickForce, ForceMode.Impulse);
            ToggleShootingIndicator(false);
            gameController.TriggerPendingResult();
            audioController.PlayKickEffect();
            goalieController.TriggerRandomJump();
        }
        else
        {
            shotRotationAngles += horizontalInput * Vector3.up;
            shotRotationAngles -= verticalInput * Vector3.right;
            arrowObj.transform.rotation = Quaternion.Euler(shotRotationAngles);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameController.gameState != GameStateEnum.PendingResult)
        {
            return;
        }

        if (other.gameObject.CompareTag("Goal"))
        {
            gameController.TriggerPostShotScreen(true);
            audioController.PlayGoalEffect();
            goalExplosion.gameObject.SetActive(true);
            goalExplosion.gameObject.transform.position = gameObject.transform.position;
            goalExplosion.Play();
            goalieController.TriggerDeath();
        }
        else if (other.gameObject.CompareTag("Bounds"))
        {
            gameController.TriggerPostShotScreen(false);
            audioController.PlayMissEffect();
        }
    }

    private void ToggleShootingIndicator(bool isActive)
    {
        foreach (GameObject indicatorObj in visibleIndicatorObjs)
        {
            indicatorObj.SetActive(isActive);
        }
    }

    public void RespawnBall()
    {
        ballRb.velocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        gameObject.transform.position = spawnPosition;
        gameObject.transform.rotation = spawnRotation;
        shotRotationAngles = Vector3.zero;
        arrowObj.transform.rotation = Quaternion.Euler(shotRotationAngles);
        ToggleShootingIndicator(true);
    }

    public void ResetBallForMainMenu()
    {
        RespawnBall();
        ToggleShootingIndicator(false);
    }
}
