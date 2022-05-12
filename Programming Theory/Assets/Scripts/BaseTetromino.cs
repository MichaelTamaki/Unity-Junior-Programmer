using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTetromino : MonoBehaviour
{
    // See Tetris Guideline's "Super Rotation System" https://tetris.wiki/Super_Rotation_System
    // Follows section "How Guideline SRS Really Works" of how The Tetris Company (TTC) actually
    // implemented wall kicks.

    public enum RotationState
    {
        Zero,
        R,
        Two,
        L
    }

    // BlockStructureStates defines blocks that are active in a coordinate system
    // Note that the y axis is in reverse (index = 0 is rendered as the bottom row)
    protected abstract bool[,,] BlockStructureStates { get; }

    // WallKickData defines the types of translations that are attempted for Super
    // Rotation System
    private static readonly int[,,] _wallKickData =
    {
        {
            { 0, 0 },
            { 0, 0 },
            { 0, 0 },
            { 0, 0 },
            { 0, 0 }
        },
        {
            { 0, 0 },
            { 1, 0 },
            { 1, -1 },
            { 0, 2 },
            { 1, 2 }
        },
        {
            { 0, 0 },
            { 0, 0 },
            { 0, 0 },
            { 0, 0 },
            { 0, 0 }
        },
        {
            { 0, 0 },
            { -1, 0 },
            { -1, -1 },
            { 0, 2 },
            { -1, 2 }
        },
    };
    protected virtual int[,,] WallKickData { get { return _wallKickData; } }

    // Spawn position
    // Assumes the middle of the field is 4 and the tetromino is 3 blocks wide
    public virtual int SpawnX { get { return 3; } }
    // According to Tetris Guideline tetrominos should spawn rows 22 and 23.
    // The "I" tetromino is just row 22.
    // Note that this is 0 indexed and for most tetrominos the first drawn row is empty
    public virtual int SpawnY { get { return 20; } }


    public RotationState currentRotation = RotationState.Zero;
    public abstract Color blockColor { get; }
    public bool isGhost = false;
    private GameObject[,] blockObjs;

    private void Start()
    {
        Draw();
    }

    public void Draw()
    {
        if (blockObjs == null)
        {
            blockObjs = new GameObject[BlockStructureStates.GetLength(1), BlockStructureStates.GetLength(2)];
        }
        for (int rowIndex = 0; rowIndex < BlockStructureStates.GetLength(1); rowIndex++)
        {
            for (int colIndex = 0; colIndex < BlockStructureStates.GetLength(2); colIndex++)
            {
                if (!blockObjs[rowIndex, colIndex])
                {
                    GameObject blockObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    blockObj.transform.SetParent(gameObject.transform);
                    blockObj.transform.localPosition = new Vector3(colIndex, rowIndex);
                    blockObj.transform.localScale = new Vector3(0.92f, 0.92f, 0.92f);
                    if (isGhost)
                    {
                        blockObj.GetComponent<Renderer>().material.SetColor("_Color", Color.gray);
                    } else
                    {
                        blockObj.GetComponent<Renderer>().material.SetColor("_Color", blockColor);
                    }
                    blockObjs[rowIndex, colIndex] = blockObj;
                }
                blockObjs[rowIndex, colIndex].SetActive(BlockStructureStates[(int)currentRotation, rowIndex, colIndex]);
            }
        }
    }

    public IEnumerable<int[]> GetBlockStructureEnumerator()
    {
        for (int rowIndex = 0; rowIndex < BlockStructureStates.GetLength(1); rowIndex++)
        {
            for (int colIndex = 0; colIndex < BlockStructureStates.GetLength(2); colIndex++)
            {
                if (BlockStructureStates[(int)currentRotation, rowIndex, colIndex])
                {
                    yield return new int[] { rowIndex, colIndex };
                }
            }
        }
    }

    public int[,] Rotate(bool isClockwise)
    {
        int stateChange = isClockwise ? 1 : -1;
        int prevRotationInt = (int)currentRotation;
        currentRotation += stateChange;
        if (currentRotation < 0)
        {
            currentRotation = (RotationState)BlockStructureStates.GetLength(0) - 1;
        }
        else if ((int)currentRotation >= BlockStructureStates.GetLength(0))
        {
            currentRotation = RotationState.Zero;
        }
        int currRotationInt = (int)currentRotation;
        int[,] kickTranslations = new int[WallKickData.GetLength(1), WallKickData.GetLength(2)];
        for (int kickIndex = 0; kickIndex < WallKickData.GetLength(1); kickIndex++)
        {
            for (int coordinateIndex = 0; coordinateIndex < WallKickData.GetLength(2); coordinateIndex++)
            {
                kickTranslations[kickIndex, coordinateIndex] = WallKickData[prevRotationInt, kickIndex, coordinateIndex] - WallKickData[currRotationInt, kickIndex, coordinateIndex];
            }
        }
        return kickTranslations;
    }
}
