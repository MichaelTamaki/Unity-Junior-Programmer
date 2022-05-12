using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private GridController gridController;
    public BaseTetromino activeTetromino;
    public BaseTetromino ghostTetromino;
    private readonly System.Type[] tetrominoBag = { typeof(TetrominoJ), typeof(TetrominoL), typeof(TetrominoS), typeof(TetrominoZ), typeof(TetrominoT), typeof(TetrominoO), typeof(TetrominoI) };
    private int tetrominoBagIndex = 0;
    private float dropInterval = 0.5f; // How long it takes for the tetromino to automatically drop
    private readonly float dropIntervalFast = 0.05f; // Interval when down arrow is held
    private bool isFastDrop = false; // is down arrow held
    private float currentDropTime = 0f; // How long the tetromino has been dropping for
    private readonly float lockDelay = 0.5f; // How long it takes to land a piece https://tetris.fandom.com/wiki/Lock_delay
    private float currentLockTime = 0f; // How long the tetromino has been on the ground

    // Start is called before the first frame update
    void Start()
    {
        ShuffleTetrominoBag();
        gridController.InitializeGrid();
        SpawnNewTetromino();
    }

    private void Update()
    {
        if (gridController.IsTetrominoGrounded(activeTetromino))
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
                if (!gridController.DropTetrominoOneSpace(activeTetromino))
                {
                    throw new System.Exception("Tried dropping tetromino on space but instead is grounded");
                }
            }
        }
    }

    public void PlaceAndSpawnTetromino()
    {
        gridController.PlaceTetromino(activeTetromino);
        SpawnNewTetromino();
    }

    private void ShuffleTetrominoBag()
    {
        for (int i = tetrominoBag.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, tetrominoBag.Length);
            System.Type temp = tetrominoBag[i];
            tetrominoBag[i] = tetrominoBag[j];
            tetrominoBag[j] = temp;
        }
    }

    private void SpawnNewTetromino()
    {
        // Reset timers
        ResetDropTime();
        ResetLockTime();

        // Choose new tetromino
        System.Type newTetrominoType = tetrominoBag[tetrominoBagIndex++];
        if (tetrominoBagIndex >= tetrominoBag.Length)
        {
            ShuffleTetrominoBag();
            tetrominoBagIndex = 0;
        }

        // Reset Active Tetromino
        if (activeTetromino != null)
        {
            Destroy(activeTetromino.gameObject);
        }
        GameObject activeTetrominoObj = new GameObject("ActiveTetromino", newTetrominoType);
        activeTetromino = activeTetrominoObj.GetComponent<BaseTetromino>();
        activeTetrominoObj.transform.localPosition = new Vector3(activeTetromino.SpawnX, activeTetromino.SpawnY);
        activeTetrominoObj.transform.SetParent(gridController.transform);

        // Attempt dropping tetromino 1 space
        if (!gridController.DropTetrominoOneSpace(activeTetromino))
        {
            Debug.Log("Game over!");
        }

        // Reset Ghost Tetromino
        if (ghostTetromino != null)
        {
            Destroy(ghostTetromino.gameObject);
        }
        GameObject ghostTetrominoObj = new GameObject("GhostTetromino", newTetrominoType);
        ghostTetrominoObj.transform.SetParent(gridController.transform);
        ghostTetromino = ghostTetrominoObj.GetComponent<BaseTetromino>();
        ghostTetromino.isGhost = true;
        UpdateGhostTetromino();
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
        Vector3 tetrominoPosition = activeTetromino.transform.localPosition;
        tetrominoPosition.y = gridController.GetTetrominoDropY(activeTetromino);
        tetrominoPosition.z = 0.01f; // Temporary fix to put ghost behind the active
        ghostTetromino.transform.localPosition = tetrominoPosition;
        ghostTetromino.currentRotation = activeTetromino.currentRotation;
        ghostTetromino.Draw();
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
