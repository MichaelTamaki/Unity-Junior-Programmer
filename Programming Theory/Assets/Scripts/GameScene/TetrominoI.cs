using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoI : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Cyan
            return new Color(0, 255, 255);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false, false, false },
            { false, false, false, false, false },
            { false, true, true, true, true},
            { false, false, false, false, false },
            { false, false, false, false, false },
        },
        {
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, false, false, false },
        },
        {
            { false, false, false, false, false },
            { false, false, false, false, false },
            { true, true, true, true, false},
            { false, false, false, false, false },
            { false, false, false, false, false },
        },
        {
            { false, false, false, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
            { false, false, true, false, false },
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }

    private static readonly int[,,] _wallKickData =
{
        {
            { 0, 0 },
            { -1, 0 },
            { 2, 0 },
            { -1, 0 },
            { 2, 0 }
        },
        {
            { -1, 0 },
            { 0, 0 },
            { 0, 0 },
            { 0, 1 },
            { 0, -2 }
        },
        {
            { -1, 1 },
            { 1, 1 },
            { -2, 1 },
            { 1, 0 },
            { -2, 0 }
        },
        {
            { 0, 1 },
            { 0, 1 },
            { 0, 1 },
            { 0, -1 },
            { 0, 2 }
        },
    };
    protected override int[,,] WallKickData { get { return _wallKickData; } }

    // Assumes the middle of the field is 4 and the tetromino is 5 blocks wide
    public override int SpawnX { get { return 2; } }
    // Spawn in row 22.
    // Note that this is 0 indexed and the first 2 rows are empty
    public override int SpawnY { get { return 19; } }
}