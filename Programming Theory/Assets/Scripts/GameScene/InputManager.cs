using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridController grid;

    // When holding down left/right arrow, trigger shift automatically
    // - One "longer" pause
    // - "Shorter" pauses afterwards
    private bool isUseMoveDelayFirst = true;
    private readonly float moveDelayFirst = 0.15f;
    private readonly float moveDelayContinuous = 0.05f;
    private float currentMoveTime = 0f;
    private enum MoveDirection
    {
        Left = -1,
        Right = 1
    }
    private MoveDirection lastMoveDirection; // Just in case both left and right keys are held, use the last key pressed

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isPaused)
        {
            return;
        }

        // Horizontal movement -- immediate action after key down
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetMoveDirection(MoveDirection.Left);
            if (grid.ShiftTetrominoPosition(gameManager.tetrominoActive, (int) lastMoveDirection))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetMoveDirection(MoveDirection.Right);
            if (grid.ShiftTetrominoPosition(gameManager.tetrominoActive, (int) lastMoveDirection))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }

        // Horizontal movement -- delayed action after holding
        if (
            (
                Input.GetKey(KeyCode.LeftArrow)
                && lastMoveDirection == MoveDirection.Left
            )
            || (
                Input.GetKey(KeyCode.RightArrow)
                && lastMoveDirection == MoveDirection.Right
            )
        )
        {
            currentMoveTime += Time.deltaTime;

            if (isUseMoveDelayFirst)
            {
                if (currentMoveTime > moveDelayFirst)
                {
                    isUseMoveDelayFirst = false;
                    currentMoveTime = 0f;
                    if (grid.ShiftTetrominoPosition(gameManager.tetrominoActive, (int) lastMoveDirection))
                    {
                        gameManager.OnSuccessfulTetrominoMove();
                    }
                }
            }
            else if (currentMoveTime > moveDelayContinuous)
            {
                currentMoveTime = 0f;
                if (grid.ShiftTetrominoPosition(gameManager.tetrominoActive, (int) lastMoveDirection))
                {
                    gameManager.OnSuccessfulTetrominoMove();
                }
            }
        }

        // Rotation
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (grid.RotateTetromino(gameManager.tetrominoActive, true))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (grid.RotateTetromino(gameManager.tetrominoActive, false))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }

        // Soft drop modifiers
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gameManager.SetIsFastDrop(true);
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            gameManager.SetIsFastDrop(false);
        }

        // Hard drop
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.IsHardDropPossible())
        {
            gameManager.PlaceAndSpawnTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameManager.TriggerTetrominoHold();
        }
    }

    // Helper fn to reset variables for the delayed shift on key hold
    private void SetMoveDirection(MoveDirection moveDirection)
    {
        isUseMoveDelayFirst = true;
        currentMoveTime = 0f;
        lastMoveDirection = moveDirection;
    }
}
