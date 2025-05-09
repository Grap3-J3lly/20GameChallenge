using Godot;
using Godot.Collections;
using System;

public partial class Wall : Area2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    [Export]
    private bool isHorizontal = false;
    [Export]
    private int movementStateToStop = 0;

    [Export]
    private Dictionary<Area2D, Vector2> directionalValues = new Dictionary<Area2D, Vector2>();

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public bool IsHorizontal { get => isHorizontal; }
    public int MovementStateToStop { get => movementStateToStop; }
    public Dictionary<Area2D, Vector2> DirectionalValues { get { return directionalValues; } }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        
    }
}
