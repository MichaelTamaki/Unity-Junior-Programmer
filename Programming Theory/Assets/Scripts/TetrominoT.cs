using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoT : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Purple
            return new Color(153, 0, 255);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false },
            { true, true, true },
            { false, true, false },
        },
        {
            { false, true, false },
            { false, true, true },
            { false, true, false },
        },
        {
            { false, true, false },
            { true, true, true },
            { false, false, false },
        },
        {
            { false, true, false },
            { true, true, false },
            { false, true, false },
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }
}