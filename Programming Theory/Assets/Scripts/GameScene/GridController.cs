using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    static readonly int HeightPlayable = 20;
    static readonly int HeightTotal = HeightPlayable * 2;
    // Note that index 0 is the row that is rendered at the bottom
    private List<GridRow> gridRows;

    public void InitializeGrid()
    {
        gridRows = new List<GridRow>();
        for (int y = 0; y < HeightTotal; y++)
        {
            GameObject rowObj = new GameObject("Grid Row", typeof(GridRow));
            rowObj.transform.SetParent(gameObject.transform, false);
            rowObj.transform.localPosition = new Vector3(0, y);
            GridRow gridRow = rowObj.GetComponent<GridRow>();
            gridRow.InitializeBlocks();
            gridRows.Add(gridRow);
        }
    }

    private bool IsValidTetrominoPosition(BaseTetromino tetromino, int xBase, int yBase)
    {
        foreach (int[] indices in tetromino.GetBlockStructureEnumerator())
        {
            int y = yBase + indices[0];
            int x = xBase + indices[1];
            if (x < 0 || x >= GridRow.blockWidth || y < 0 || y >= HeightTotal || gridRows[y].IsBlockActive(x))
            {
                return false;
            }
        }
        return true;
    }

    public bool ShiftTetrominoPosition(BaseTetromino tetromino, int xChange)
    {
        if (IsValidTetrominoPosition(tetromino, (int) tetromino.transform.position.x + xChange, (int) tetromino.transform.position.y))
        {
            tetromino.transform.Translate(new Vector3(xChange, 0));
            return true;
        }
        return false;
    }

    public bool RotateTetromino(BaseTetromino tetromino, bool isClockwise)
    {
        // Using Super Rotation System to check for all possible rotation positions
        int[,] kickTranslations = tetromino.Rotate(isClockwise);
        for (int kickIndex = 0; kickIndex < kickTranslations.GetLength(0); kickIndex++)
        {
            if (IsValidTetrominoPosition(tetromino, (int) tetromino.transform.position.x + kickTranslations[kickIndex, 0], (int) tetromino.transform.position.y + kickTranslations[kickIndex, 1]))
            {
                tetromino.transform.Translate(new Vector3(kickTranslations[kickIndex, 0], kickTranslations[kickIndex, 1]));
                tetromino.Draw();
                return true;
            }
        }
        tetromino.Rotate(!isClockwise);
        return false;
    }

    public bool IsTetrominoGrounded(BaseTetromino tetromino)
    {
        return !IsValidTetrominoPosition(tetromino, (int)tetromino.transform.position.x, (int)tetromino.transform.position.y - 1);
    }

    public bool DropTetrominoOneSpace(BaseTetromino tetromino)
    {
        if (!IsTetrominoGrounded(tetromino))
        {
            tetromino.transform.Translate(new Vector3(0, -1));
            return true;
        }
        return false;
    }

    public int GetTetrominoDropY(BaseTetromino tetromino)
    {
        int dropRowIndex = int.MaxValue;
        // Goes down to -2 because it is possible for a tetromino's bottom rows to be completely empty
        for (int rowIndex = (int) tetromino.transform.position.y; rowIndex >= -2; rowIndex--)
        {
            if (IsValidTetrominoPosition(tetromino, (int) tetromino.transform.position.x, rowIndex))
            {
                dropRowIndex = rowIndex;
            }
            else
            {
                break;
            }
        }
        if (dropRowIndex == int.MaxValue)
        {
            throw new System.Exception("GetTetrominoDropY: unable to find valid position");
        }
        else
        {
            return dropRowIndex;
        }
    }

    public void PlaceTetromino(BaseTetromino tetromino)
    {
        // Update grid rows and catch filled rows
        int dropY = GetTetrominoDropY(tetromino);
        int dropX = (int) tetromino.transform.position.x;
        List<GridRow> filledRows = new List<GridRow>();
        foreach (int[] indices in tetromino.GetBlockStructureEnumerator())
        {
            int y = dropY + indices[0];
            int x = dropX + indices[1];
            if (gridRows[y].PlaceBlock(x, tetromino.blockColor))
            {
                filledRows.Add(gridRows[y]);
            }
        }

        // Remove and re-add to the top filled grid rows
        foreach (GridRow filledRow in filledRows)
        {
            gridRows.Remove(filledRow);
            gridRows.Add(filledRow);
            filledRow.ResetBlocks();
        }

        // Shift all grid rows to correct y position
        for (int y = 0; y < gridRows.Count; y++)
        {
            gridRows[y].gameObject.transform.localPosition = new Vector3(0, y);
        }
    }
}
