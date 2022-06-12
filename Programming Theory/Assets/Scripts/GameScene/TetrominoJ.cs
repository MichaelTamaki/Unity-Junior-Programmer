using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoJ : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Dark blue
            return new Color(0, 0, 255);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false },
            { true, true, true },
            { true, false, false },
        },
        {
            { false, true, false },
            { false, true, false },
            { false, true, true },
        },
        {
            { false, false, true },
            { true, true, true },
            { false, false, false },
        },
        {
            { true, true, false },
            { false, true, false },
            { false, true, false },
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }
}
