using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles the drawing and defines the properties of the Tetromino
public class TetrominoZ : BaseTetromino
{
    public override Color blockColor
    {
        get
        {
            // Red
            return new Color(255, 0, 0);
        }
    }

    private static readonly bool[,,] _blockStructureStates =
    {
        {
            { false, false, false },
            { false, true, true },
            { true, true, false },
        },
        {
            { false, true, false },
            { false, true, true },
            { false, false, true },
        },
        {
            { false, true, true },
            { true, true, false },
            { false, false, false },
        },
        {
            { true, false, false },
            { true, true, false },
            { false, true, false },
        },
    };

    protected override bool[,,] BlockStructureStates { get { return _blockStructureStates; } }
}