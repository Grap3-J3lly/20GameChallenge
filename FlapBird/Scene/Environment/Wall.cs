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
    private Vector2 spawnLocation;
    [Export]
	private float speed = 100f;
	[Export]
	private Area2D topWall;
    [Export]
    private Area2D botWall;
    private bool overlappingGoal = false;

    // Floor Logic
    [Export]
    private bool isFloor = false;
    

    // --------------------------------
    //		    PROPERTIES	
    // --------------------------------

    public Vector2 SpawnLocation { get => spawnLocation; set => spawnLocation = value; }

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!gameManager.GameOver) // && !gameManager.Resetting)
		{
			Movement(delta);
			HandleGoalCollision();
		}
	}

    private void OnWallBodyEntered(RigidBody2D body)
    {
        CheckWall(topWall, body);
        CheckWall(botWall, body);
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
        SpawnLocation = newPos;
	}

	private void HandleGoalCollision()
	{
        if(!overlappingGoal)
        {
		    CheckGoal();
        }
	}

	private bool CheckWall(Area2D wall, RigidBody2D body)
	{
        if(gameManager.GameOver) { return false; }
        Array<Node2D> overlappingAreas = wall.GetOverlappingBodies();
        foreach (Node2D area in overlappingAreas)
        {
            if (area.GetClass().Equals("RigidBody2D"))
            {
                gameManager.EnterGameOver();
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
                gameManager.PlayScoreSound();
                gameManager.RespawnObstacle();
            }
        }
    }

}
