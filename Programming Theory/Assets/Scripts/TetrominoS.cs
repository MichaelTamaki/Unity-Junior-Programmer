using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoS : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Green
            return new Color(0, 255, 0);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false },
            { true, true, false },
            { false, true, true },
        },
        {
            { false, false, true },
            { false, true, true },
            { false, true, false },
        },
        {
            { true, true, false },
            { false, true, true },
            { false, false, false },
        },
        {
            { false, true, false },
            { true, true, false },
            { true, false, false },
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }
}