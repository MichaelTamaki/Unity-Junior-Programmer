using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridController gridController;
    [SerializeField] private UIManager uiManager;
    private BaseTetromino tetrominoNext;
    public BaseTetromino tetrominoActive;
    private BaseTetromino tetrominoGhost;
    private BaseTetromino tetrominoHold;
    public bool isPaused = false; // Prevents input while a menu is active
    private bool isHoldTriggered = false; // Don't allow hold to be triggered twice in a row before dropping a tetromino
    // Keep track of tetromino queue with two bags. This is because we show a queue, and the index may be the last item in the bag.
    // So we must pre-generate the next bag.
    private readonly System.Type[] tetrominoBagNext = { typeof(TetrominoJ), typeof(TetrominoL), typeof(TetrominoS), typeof(TetrominoZ), typeof(TetrominoT), typeof(TetrominoO), typeof(TetrominoI) };
    private readonly System.Type[] tetrominoBagActive = new System.Type[7];
    private int tetrominoBagIndex = 0;
    // Game variables
    private int linesCleared = 0;
    private int speedLevel = 1;
    private readonly int speedLevelLinesClearedInterval = 4; // Every x cleared lines, increase the speed level
    // Timing variables
    private readonly float[] speedLevelDropIntervals = { 1.0f, 1.0f, 0.5f, 0.35f, 0.2f, 0.15f, 0.1f, 0.08f, 0.05f, 0.035f, 0.025f, 0.015f, 0.01f, 0.005f, 0.001f }; // How long it takes for the tetromino to automatically drop, based on speedLevel
    private readonly float dropIntervalFast = 0.04f; // Interval when down arrow is held
    private bool isFastDrop = false; // is down arrow held
    private float currentDropTime = 0f; // How long the tetromino has been dropping for
    private readonly float lockDelay = 0.5f; // How long it takes to land a piece https://tetris.fandom.com/wiki/Lock_delay
    private float currentLockTime = 0f; // How long the tetromino has been on the ground
    private float lastSoftDropTime = 0f; // Keep track of the last timestamp where a soft drop occurred
    private readonly float softDropGracePeriod = 0.3f; // Time between the last soft drop and the next time a hard drop can occur

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewTetrominoBag(); // Shuffle tetrominoBagNext
        GenerateNewTetrominoBag(); // Shuffled bag is set to active bag
        gridController.InitializeGrid();
        SpawnNewTetromino(true);
        UpdateGameUi();
    }

    private void Update()
    {
        if (isPaused)
        {
            return;
        }

        if (!gridController.IsTetrominoGrounded(tetrominoActive))
        {
            ResetLockTime();
        }

        // Soft drop tetromino
        currentDropTime += Time.deltaTime;
        float currentDropInterval = speedLevelDropIntervals[speedLevel];
        if (isFastDrop)
        {
            // Use currentDropInterval if it is faster than dropIntervalFast
            currentDropInterval = Mathf.Min(dropIntervalFast, currentDropInterval);
        }
        // In case currentDropInterval is smaller than delta time, be able to handle multiple drops per Update call
        while (currentDropTime > currentDropInterval && !gridController.IsTetrominoGrounded(tetrominoActive))
        {
            currentDropTime -= currentDropInterval;
            if (!gridController.DropTetrominoOneSpace(tetrominoActive))
            {
                // This should never occur
                Debug.LogError("Attempted to drop a tetromino even though it is grounded");
            }
        }

        if (gridController.IsTetrominoGrounded(tetrominoActive))
        {
            // Do not place tetromino until user has not moved for half a second
            // https://tetris.fandom.com/wiki/Lock_delay
            ResetDropTime();
            currentLockTime += Time.deltaTime;
            if (currentLockTime > lockDelay)
            {
                PlaceAndSpawnTetromino();
                lastSoftDropTime = Time.time;
            }
        }
    }

    public void PlaceAndSpawnTetromino()
    {
        linesCleared += gridController.PlaceTetromino(tetrominoActive);
        while (linesCleared > speedLevelLinesClearedInterval * speedLevel && speedLevel + 1 < speedLevelDropIntervals.Length)
        {
            speedLevel++;
        }
        UpdateGameUi();
        SpawnNewTetromino(true);
        isHoldTriggered = false; // Allow hold to be triggered again
    }

    private void GenerateNewTetrominoBag()
    {
        // Copy next tetromino bag into active tetromino bag
        for (int i = 0; i < tetrominoBagNext.Length; i++)
        {
            tetrominoBagActive[i] = tetrominoBagNext[i];
        }

        // Shuffle next tetromino bag to "generate" a new bag
        for (int j = tetrominoBagNext.Length - 1; j > 0; j--)
        {
            int k = Random.Range(0, tetrominoBagNext.Length);
            System.Type temp = tetrominoBagNext[j];
            tetrominoBagNext[j] = tetrominoBagNext[k];
            tetrominoBagNext[k] = temp;
        }
    }

    // Sets the active and ghost tetromino
    // This does not update the next/hold tetromino, so this should be done by the consumer
    private void SetActiveAndGhostTetromino(BaseTetromino newTetromino, bool destroyTetrominoActive)
    {
        // Reset timers
        ResetDropTime();
        ResetLockTime();

        // Clone the new tetromino to active
        if (destroyTetrominoActive && tetrominoActive != null)
        {
            Destroy(tetrominoActive.gameObject);
        }
        GameObject tetrominoActiveObj = Instantiate(newTetromino.gameObject, gridController.transform);
        tetrominoActive = tetrominoActiveObj.GetComponent<BaseTetromino>();
        tetrominoActiveObj.transform.localPosition = new Vector3(tetrominoActive.SpawnX, tetrominoActive.SpawnY);

        // Attempt dropping active tetromino 1 space
        if (!gridController.DropTetrominoOneSpace(tetrominoActive))
        {
            TriggerUiScreenAndPause(UIManager.UIScreen.FailScreen);
        }

        // Set the new tetromino to ghost
        if (tetrominoGhost != null)
        {
            Destroy(tetrominoGhost.gameObject);
        }
        GameObject tetrominoGhostObj = newTetromino.gameObject;
        tetrominoGhost = tetrominoGhostObj.GetComponent<BaseTetromino>();
        tetrominoGhost.isGhost = true;
        UpdateGhostTetromino();
    }

    // Spawns a new tetromino from the bag. Use this when a tetromino is dropped.
    private void SpawnNewTetromino(bool destroyTetrominoActive)
    {
        // Choose new next tetromino
        System.Type newTetrominoNextType = tetrominoBagActive[tetrominoBagIndex++];
        if (tetrominoBagIndex >= tetrominoBagActive.Length)
        {
            GenerateNewTetrominoBag();
            tetrominoBagIndex = 0;
        }

        // Create new next tetromino
        GameObject newTetrominoNextObj = new GameObject("Tetromino", newTetrominoNextType);
        newTetrominoNextObj.transform.SetParent(gridController.transform);

        // On the first time this is run, there is no next tetromino so we must run
        // SpawnNewTetromino again to properly set the active / ghost tetromino
        if (tetrominoNext == null)
        {
            tetrominoNext = newTetrominoNextObj.GetComponent<BaseTetromino>();
            SpawnNewTetromino(destroyTetrominoActive);
            return;
        }

        // Set the old next tetromino to the active/ghost tetromino
        SetActiveAndGhostTetromino(tetrominoNext, destroyTetrominoActive);

        // Set the new next tetromino
        tetrominoNext = newTetrominoNextObj.GetComponent<BaseTetromino>();
        newTetrominoNextObj.transform.localPosition = new Vector3(13, 18);
    }

    public void TriggerTetrominoHold()
    {
        // Ensure hold can't happen twice in a row before dropping a tetromino
        if (isHoldTriggered)
        {
            return;
        }

        // Swap active and hold tetromino
        BaseTetromino temp = tetrominoActive;
        if (tetrominoHold == null)
        {
            SpawnNewTetromino(false);
        } else
        {
            SetActiveAndGhostTetromino(tetrominoHold, false);
        }
        tetrominoHold = temp;

        // Reset hold tetromino rotation, and set position
        tetrominoHold.ResetRotationAndDraw();
        tetrominoHold.transform.localPosition = new Vector3(-5, 18);

        isHoldTriggered = true;
    }

    private void ResetDropTime()
    {
        currentDropTime = 0f;
    }

    private void ResetLockTime()
    {
        currentLockTime = 0f;
    }

    private void UpdateGhostTetromino()
    {
        Vector3 tetrominoPosition = tetrominoActive.transform.localPosition;
        tetrominoPosition.y = gridController.GetTetrominoDropY(tetrominoActive);
        tetrominoPosition.z = 0.01f; // Temporary fix to put ghost behind the active
        tetrominoGhost.transform.localPosition = tetrominoPosition;
        tetrominoGhost.currentRotation = tetrominoActive.currentRotation;
        tetrominoGhost.Draw();
    }

    // To be used upon successful shifting or rotation
    public void OnSuccessfulTetrominoMove()
    {
        ResetLockTime();
        UpdateGhostTetromino();
    }

    public void SetIsFastDrop(bool isFast)
    {
        isFastDrop = isFast;
        ResetDropTime(); // Prevent case where isFastDrop is set to true when currentDropTime is high. This causes a tetromino to drop multiple times unexpectedly.
    }

    private void TriggerUiScreenAndPause(UIManager.UIScreen uiScreen)
    {
        isPaused = true;
        Time.timeScale = 0.0f;
        uiManager.ToggleUiScreen(uiScreen, true);
    }

    //private void HideUiScreenAndResume(UIManager.UIScreen uiScreen)
    //{
    //    Time.timeScale = 1.0f;
    //    uiManager.ToggleUiScreen(uiScreen, false);
    //}

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenuScene");
    }

    private void UpdateGameUi()
    {
        uiManager.UpdateGameUiText(linesCleared, speedLevel);
    }

    // Prevent accidental hard drop right after a soft drop happens (may occur when trying to hard drop, but happens right at the end of the lock time)
    public bool IsHardDropPossible()
    {
        return Time.time - lastSoftDropTime > softDropGracePeriod;
    }
}
