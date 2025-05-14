using Godot;
using Godot.Collections;
using System;

public partial class Wall : StaticBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    [Export]
    private bool isHorizontal = false;
    [Export]
    private int movementStateToStop = 0;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public bool IsHorizontal { get => isHorizontal; }
    public int MovementStateToStop { get => movementStateToStop; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        
    }
}
