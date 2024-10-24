using Godot;
using System;

public partial class Wall : Area2D
{
    [Export]
    private int movementStateToStop = 0;

    public int MovementStateToStop { get => movementStateToStop; }

    public override void _Ready()
    {
        
    }
}
