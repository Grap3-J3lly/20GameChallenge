using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Ball : Area2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    private GameManager gameManager;
    [Export]
    private Vector2 startingLocation = new Vector2();
    [Export]
	private float ballSpeed = 10f;
    private float startSpeed;
	private Vector2 movementState = new Vector2();
	private Vector2 currentDirection;

    [Export]
    private int maxCollisionCount = 3;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public float BallSpeed { get => ballSpeed; set => ballSpeed = value; }
    public Vector2 CurrentDirection { get => currentDirection; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        gameManager = GameManager.Instance;
        startSpeed = ballSpeed;
        Setup();
	}

	public override void _PhysicsProcess(double delta)
	{
        if(gameManager.GameOver)
        {
            return;
        }

		Move(delta);
        HandleCollision();
	}

    // --------------------------------
    //		    SETUP LOGIC	
    // --------------------------------

    private void Setup()
    {
        currentDirection = PickRandomDirection();
        AssignMovementState();
    }

    private Vector2 PickRandomDirection()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Vector2 randomDirection = new Vector2(rng.RandfRange(-1f, 1f), rng.RandfRange(-1f, 1f));
		return randomDirection;
	}

	private void AssignMovementState()
	{
		if(currentDirection.X > 0) { movementState.X = 1; }
        if(currentDirection.X < 0) { movementState.X = -1; }
        if(currentDirection.Y > 0) { movementState.Y = 1; }
        if(currentDirection.Y < 0) { movementState.Y = -1; }
    }

    // --------------------------------
    //		TRANSFORM LOGIC	
    // --------------------------------

    private void Move(double delta)
    {
        // HandleImpactEvents();
        Vector2 newPosition = new Vector2(Position.X + (ballSpeed * (float)delta) * movementState.X, Position.Y + (ballSpeed * (float)delta) * movementState.Y);
        Position = newPosition;
    }

    private void ResetPosition()
    {
        Position = startingLocation;
        if(!gameManager.GameOver)
        {
            Setup();
        }
        else
        {
            Visible = false;
        }
    }

    public void ResetSpeed()
    {
        ballSpeed = startSpeed;
    }

    private void AssignDirection(bool inXDirection) // , Vector2 directionalAssignment)
    {
        if (inXDirection)
        {
            movementState = new Vector2(-movementState.X, movementState.Y);
            currentDirection.X *= -1;
        }
        else
        {
            movementState = new Vector2(movementState.X, -movementState.Y);
            currentDirection.Y *= -1;
        }
        GD.Print($"Ball.cs: Reversing Direction. Current Direction: {currentDirection}, Movement State: {movementState}");
    }

    // --------------------------------
    //		    IMPACT LOGIC	
    // --------------------------------

    public void HandleCollision()
    {
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        Queue<Area2D> areasToHandle = new Queue<Area2D>();

        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            if(IsAreaImportant(overlappingAreas[i]))
            {
                areasToHandle.Enqueue(overlappingAreas[i]);
            }
        }

        while(areasToHandle.Count > 0)
        {
            // Paddle
            // 1 Wall
            // 2 Walls
            // 1 Brick
            // 2 Bricks
            // 3 Bricks
            // 1 Wall 1 Brick
            // 1 Wall 2 Bricks
            
            HandleImpactEvent(areasToHandle.Dequeue());
        }

    }

    public bool IsAreaImportant(Area2D overlappingArea)
    {
        return // IsObjectOfType<Brick>(overlappingArea)
            IsObjectOfType<Wall>(overlappingArea)
            // || IsObjectOfType<Paddle>(overlappingArea) 
            || IsObjectOfType<Goal>(overlappingArea)
            || IsObjectOfType<Brick>(overlappingArea.GetParent<Area2D>())
            || IsObjectOfType<Paddle>(overlappingArea.GetParent<Area2D>());
    }

    public void HandleImpactEvent(Area2D overlappingArea)
    {
        //if (IsObjectOfType<Brick>(overlappingArea))
        //{
        //    HandleBrickImpact((Brick)overlappingArea);
        //    return;
        //}
        if (IsObjectOfType<Wall>(overlappingArea))
        {
            HandleWallImpact((Wall)overlappingArea);
            return;
        }
        //if (IsObjectOfType<Paddle>(overlappingArea))
        //{
        //    HandlePaddleImpact();
        //    return;
        //}
        if (IsObjectOfType<Goal>(overlappingArea))
        {
            HandleGoalImpact((Goal)overlappingArea);
            return;
        }
        if (IsObjectOfType<Brick>(overlappingArea.GetParent<Area2D>()))
        {
            HandleBrickImpact((Brick)overlappingArea);
            return;
        }
        if (IsObjectOfType<Paddle>(overlappingArea.GetParent<Area2D>()))
        {
            HandlePaddleImpact();
            return;
        }
    }

    private bool IsObjectOfType<T>(Area2D overlappingArea) where T : Godot.Area2D
    {
        try
        {
            T desiredObject = (T)overlappingArea;
            if (desiredObject != null)
            {
                GD.Print($"Ball.cs: Is Impacting Object of Type {typeof(T).Name}");
                return true;
            }
        }
        catch (Exception ex)
        {
            // GD.Print($"Ball.cs: Object was Null");
        }
        
        return false;
    }

    private void HandleBrickImpact(Brick hitBrick)
    {
        GD.Print($"Ball.cs: Registering Brick Collision");
        //if(brickQueue.Contains(hitBrick))
        //{
        //    GD.Print($"Ball.cs: {hitBrick.Name} already in Brick Queue");
        //    return;
        //}

        //hitBrick.ProcessHit();
        //brickQueue.Enqueue(hitBrick);
        //// AssignDirection(collisionLayer == 2);
        //GD.Print($"Ball.cs: Hit {hitBrick.Name}, Adding to Brick Queue\n\n");

        //if(brickQueue.Count >= maxCollisionCount)
        //{
        //    GD.Print($"Ball.cs: Removing {brickQueue.Peek().Name}");
        //    brickQueue.Dequeue();
        //}
    }

    private void HandleWallImpact(Wall hitWall)
    {
        GD.Print($"Ball.cs: Registering Wall Collision");
        // AssignDirection(!hitWall.IsHorizontal);
    }

    private void HandlePaddleImpact()
    {
        GD.Print($"Ball.cs: Registering Paddle Collision");
        // AssignDirection(collisionLayer == 2);
    }

    private void HandleGoalImpact(Goal hitGoal)
    {
        GD.Print($"Ball.cs: Registering Goal Collision");
        //gameManager.UpdateScore(hitGoal, true);
        //ResetPosition();
    }
}
