using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameStateEnum
{
    MainMenu,
    Shooting,
    PendingResult,
    PostShot,
    Paused
}

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject canvasOverlayObj;
    [SerializeField] private GameObject mainMenuObj;
    [SerializeField] private GameObject postShotScreenObj;
    [SerializeField] private GameObject pauseScreenObj;
    [SerializeField] private GameObject pauseButtonObj;
    [SerializeField] private GameObject statsScreenObj;
    [SerializeField] private TextMeshProUGUI pendingTimerText;
    [SerializeField] private TextMeshProUGUI postShotStatusText;
    [SerializeField] private TextMeshProUGUI postShotTimerText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    private BallController ballController;
    private AudioController audioController;
    private CameraController cameraController;
    private GoalieController goalieController;

    public GameStateEnum gameState = GameStateEnum.MainMenu;
    private GameStateEnum gameStatePrePause;
    private int shotsScored = 0;
    private int shotsTaken = 0;

    private void Start()
    {
        ballController = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
        cameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();
        goalieController = GameObject.FindGameObjectWithTag("Goalie").GetComponent<GoalieController>();
    }

    public void StartGame()
    {
        gameState = GameStateEnum.Shooting;
        Time.timeScale = 1f;
        ballController.RespawnBall();
        goalieController.TriggerRespawn();
        canvasOverlayObj.SetActive(false);
        mainMenuObj.SetActive(false);
        statsScreenObj.SetActive(true);
        pauseButtonObj.SetActive(true);
        pendingTimerText.gameObject.SetActive(false);
        shotsScored = 0;
        shotsTaken = 0;
        accuracyText.text = "Accuracy: 0 out of 0 (0%)";
        audioController.PlayBackgroundMusic(false);
        cameraController.ToggleCamera(true);
    }

    public void TriggerPendingResult()
    {
        gameState = GameStateEnum.PendingResult;
        cameraController.EnableCameraIsFollowingBall();
        StartCoroutine(StartPendingTimer());
    }

    public void TriggerPostShotScreen(bool isGoal)
    {
        gameState = GameStateEnum.PostShot;
        if (isGoal)
        {
            shotsScored += 1;
        }
        shotsTaken += 1;
        accuracyText.text = "Accuracy: " + shotsScored + " out of " + shotsTaken + " (" + Mathf.RoundToInt(100 * shotsScored / shotsTaken) + "%)";
        pauseButtonObj.SetActive(false);
        canvasOverlayObj.SetActive(true);
        postShotScreenObj.SetActive(true);
        postShotStatusText.text = isGoal ? "GOOOOOOOOAL" : "Miss!";
        pendingTimerText.gameObject.SetActive(false);
        StartCoroutine(ResetForShot());
    }

    public void TriggerPause()
    {
        gameStatePrePause = gameState;
        gameState = GameStateEnum.Paused;
        pauseButtonObj.SetActive(false);
        canvasOverlayObj.SetActive(true);
        pauseScreenObj.SetActive(true);
        Time.timeScale = 0f;
        audioController.PauseBackgroundMusic();
        audioController.SyncVolumeSliders();
    }

    public void TriggerResume()
    {
        gameState = gameStatePrePause;
        pauseButtonObj.SetActive(true);
        canvasOverlayObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        Time.timeScale = 1f;
        audioController.UnPauseBackgroundMusic();
    }

    public void TriggerMainMenu()
    {
        gameState = GameStateEnum.MainMenu;
        canvasOverlayObj.SetActive(true);
        postShotScreenObj.SetActive(false);
        statsScreenObj.SetActive(false);
        pauseScreenObj.SetActive(false);
        mainMenuObj.SetActive(true);
        ballController.ResetBallForMainMenu();
        audioController.PlayBackgroundMusic(true);
        audioController.SyncVolumeSliders();
        cameraController.ToggleCamera(true);
        cameraController.DisableCameraIsFollowingBall();
    }

    private IEnumerator StartPendingTimer()
    {
        int timeLeft = 5;
        yield return new WaitForSeconds(1);
        while (gameState == GameStateEnum.PendingResult)
        {
            timeLeft -= 1;
            if (timeLeft <= 3)
            {
                pendingTimerText.text = timeLeft + "...";
                pendingTimerText.gameObject.SetActive(true);
            }
            if (timeLeft <= 0)
            {
                TriggerPostShotScreen(false);
                break;
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator ResetForShot()
    {
        int timeLeft = 3;
        postShotTimerText.SetText("Resetting in " + timeLeft + "...");
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            timeLeft -= 1;
            postShotTimerText.SetText("Resetting in " + timeLeft + "...");
        }
        gameState = GameStateEnum.Shooting;
        pauseButtonObj.SetActive(true);
        canvasOverlayObj.SetActive(false);
        postShotScreenObj.SetActive(false);
        ballController.RespawnBall();
        cameraController.DisableCameraIsFollowingBall();
        goalieController.TriggerRespawn();
    }
}
