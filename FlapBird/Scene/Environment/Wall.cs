using Godot;
using Godot.Collections;
using System;

public partial class Wall : Area2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
	private GameManager gameManager;

    [Export]
    private bool isFloor = false;
    [Export]
	private float speed = 100f;
	[Export]
	private Area2D topWall;
    [Export]
    private Area2D botWall;
    private bool overlappingGoal = false;

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (gameManager.GameRunning && !gameManager.Resetting)
		{
			Movement(delta);
			HandleCollision();
		}
	}

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    private void Movement(double delta)
	{
        if (isFloor)
        {
            return;
        }
        Vector2 newPos = Position;
		newPos.X -= speed * (float)delta;
		Position = newPos;
	}

	private void HandleCollision()
	{
        if (topWall == null || botWall == null) return;
        
        if(CheckWall(topWall) || CheckWall(botWall))
        {
            return;
        }

        if(!overlappingGoal)
        {
		    CheckGoal();
        }
	}

	private bool CheckWall(Area2D wall)
	{
        if(!gameManager.GameRunning) { return false; }
        Array<Node2D> overlappingAreas = wall.GetOverlappingBodies();
        foreach (Node2D area in overlappingAreas)
        {
            if (area.GetClass().Equals("RigidBody2D"))
            {
                gameManager.GameRunning = false;
				return true;
            }
        }
        return false;
	}

	private void CheckGoal()
	{
        Array<Node2D> overlappingAreas = GetOverlappingBodies();
        foreach (Node2D area in overlappingAreas)
        {
            if (area.GetClass().Equals("RigidBody2D"))
            {
                overlappingGoal = true;
                Character character = (Character)area;
				character.SetGoalFace();
                gameManager.UpdateScore();
            }
        }
    }
}
