using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GridController grid;

    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (grid.ShiftTetrominoPosition(gameManager.activeTetromino, -1))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (grid.ShiftTetrominoPosition(gameManager.activeTetromino, 1))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }

        // Rotation
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (grid.RotateTetromino(gameManager.activeTetromino, true))
            {
                gameManager.OnSuccessfulTetrominoMove();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (grid.RotateTetromino(gameManager.activeTetromino, false))
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.PlaceAndSpawnTetromino();
        }
    }
}
