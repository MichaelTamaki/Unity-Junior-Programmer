using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridController gridController;
    public BaseTetromino tetrominoNext;
    public BaseTetromino tetrominoActive;
    public BaseTetromino tetrominoGhost;
    // Keep track of tetromino queue with two bags. This is because we show a queue, and the index may be the last item in the bag.
    // So we must pre-generate the next bag.
    private readonly System.Type[] tetrominoBagNext = { typeof(TetrominoJ), typeof(TetrominoL), typeof(TetrominoS), typeof(TetrominoZ), typeof(TetrominoT), typeof(TetrominoO), typeof(TetrominoI) };
    private readonly System.Type[] tetrominoBagActive = new System.Type[7];
    private int tetrominoBagIndex = 0;
    // Timing variables
    private float dropInterval = 0.5f; // How long it takes for the tetromino to automatically drop
    private readonly float dropIntervalFast = 0.05f; // Interval when down arrow is held
    private bool isFastDrop = false; // is down arrow held
    private float currentDropTime = 0f; // How long the tetromino has been dropping for
    private readonly float lockDelay = 0.5f; // How long it takes to land a piece https://tetris.fandom.com/wiki/Lock_delay
    private float currentLockTime = 0f; // How long the tetromino has been on the ground

    // Start is called before the first frame update
    void Start()
    {
        GenerateNewTetrominoBag();
        gridController.InitializeGrid();
        SpawnNewTetromino();
    }

    private void Update()
    {
        if (gridController.IsTetrominoGrounded(tetrominoActive))
        {
            // Do not place tetromino until user has not moved for half a second
            // https://tetris.fandom.com/wiki/Lock_delay
            ResetDropTime();
            currentLockTime += Time.deltaTime;
            if (currentLockTime > lockDelay)
            {
                PlaceAndSpawnTetromino();
            }
        }
        else
        {
            // Soft drop tetromino
            ResetLockTime();
            currentDropTime += Time.deltaTime;
            if (currentDropTime > (isFastDrop ? dropIntervalFast : dropInterval))
            {
                ResetDropTime();
                if (!gridController.DropTetrominoOneSpace(tetrominoActive))
                {
                    throw new System.Exception("Tried dropping tetromino on space but instead is grounded");
                }
            }
        }
    }

    public void PlaceAndSpawnTetromino()
    {
        gridController.PlaceTetromino(tetrominoActive);
        SpawnNewTetromino();
    }

    private void GenerateNewTetrominoBag()
    {
        // Copy next tetromino bag into active tetromino bag
        for (int i = 0; i < tetrominoBagNext.Length; i++)
        {
            tetrominoBagActive[i] = tetrominoBagNext[i];
        }

        // Shuffle next tetromino bag
        for (int j = tetrominoBagNext.Length - 1; j > 0; j--)
        {
            int k = Random.Range(0, tetrominoBagNext.Length);
            System.Type temp = tetrominoBagNext[j];
            tetrominoBagNext[j] = tetrominoBagNext[k];
            tetrominoBagNext[k] = temp;
        }
    }

    private void SpawnNewTetromino()
    {
        // Reset timers
        ResetDropTime();
        ResetLockTime();

        // Choose new tetromino
        System.Type newTetrominoType = tetrominoBagActive[tetrominoBagIndex++];
        if (tetrominoBagIndex >= tetrominoBagActive.Length)
        {
            GenerateNewTetrominoBag();
            tetrominoBagIndex = 0;
        }

        // Create new next tetromino
        GameObject tetrominoNextObj = new GameObject("Tetromino", newTetrominoType);
        tetrominoNextObj.transform.SetParent(gridController.transform);

        // On the first time this is run, there is no next tetromino so we must run
        // SpawnNewTetromino again to properly set the active / ghost tetromino
        if (tetrominoNext == null)
        {
            tetrominoNext = tetrominoNextObj.GetComponent<BaseTetromino>();
            SpawnNewTetromino();
            return;
        }

        // Clone the old next tetromino to active tetromino
        if (tetrominoActive != null)
        {
            Destroy(tetrominoActive.gameObject);
        }
        GameObject tetrominoActiveObj = Instantiate(tetrominoNext.gameObject, gridController.transform);
        tetrominoActive = tetrominoActiveObj.GetComponent<BaseTetromino>();
        tetrominoActiveObj.transform.localPosition = new Vector3(tetrominoActive.SpawnX, tetrominoActive.SpawnY);

        // Attempt dropping tetromino 1 space
        if (!gridController.DropTetrominoOneSpace(tetrominoActive))
        {
            Debug.Log("Game over!");
        }

        // Set the old next tetromino to ghost tetromino
        if (tetrominoGhost != null)
        {
            Destroy(tetrominoGhost.gameObject);
        }
        GameObject tetrominoGhostObj = tetrominoNext.gameObject;
        tetrominoGhost = tetrominoGhostObj.GetComponent<BaseTetromino>();
        tetrominoGhost.isGhost = true;
        UpdateGhostTetromino();

        // Set the new next tetromino
        tetrominoNext = tetrominoNextObj.GetComponent<BaseTetromino>();
        tetrominoNextObj.transform.localPosition = new Vector3(13, 18);
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
    }
}
