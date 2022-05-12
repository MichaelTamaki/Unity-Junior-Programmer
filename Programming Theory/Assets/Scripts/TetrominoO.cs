using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoO : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Yellow
            return new Color(255, 255, 0);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false },
            { false, true, true },
            { false, true, true }
        },
        {
            { false, true, true },
            { false, true, true },
            { false, false, false }
        },
        {
            { true, true, false },
            { true, true, false },
            { false, false, false }
        },
        {
            { false ,false, false },
            { true, true, false },
            { true, true, false }
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }

    private static readonly int[,,] _wallKickData =
{
        {
            { 0, 0 }
        },
        {
            { 0, -1 },
        },
        {
            { -1, -1 },
        },
        {
            { -1, 0 },
        },
    };
    protected override int[,,] WallKickData { get { return _wallKickData; } }
}