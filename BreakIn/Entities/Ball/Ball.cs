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

    private Area2D collidingObject;

    private Queue<Brick> brickQueue = new Queue<Brick>();
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
        if(!gameManager.GameOver)
        {
		    Move(delta);
        }
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

    private void ReverseDirection(bool inXDirection)
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

    public void OnAreaEnter(Node2D impactObject)
    {
        if (collidingObject != null)
        {
            GD.Print($"Ball.cs: Colliding Object Name: {collidingObject.Name} on {collidingObject.GetParent().Name}");
        }
        GD.Print($"Ball.cs: Impact Object Name: {impactObject.Name} on {impactObject.GetParent().Name}");
        // GD.Print("OnAreaEnter Impact Object Name: " + impactObject.Name);
        if(collidingObject != null && (collidingObject == (Area2D)impactObject || IsChildCollision<Paddle>(impactObject) || IsChildCollision<Brick>(impactObject)))
        {
            GD.Print("Ball.cs: Incorrect Collision, Ignoring");
            return;
        }
        collidingObject = (Area2D)impactObject;
        HandleImpactEvents();

    }


    // Did we collide with the child of a specific object
    private bool IsChildCollision<T>(Node2D impactObject) where T : Godot.Node
    {
        try
        {
            GD.Print($"Ball.cs: Checking for {typeof(T).Name} Collision on: {impactObject.GetParent().Name}");
            return (T)impactObject.GetParent() != null &&
            (impactObject.GetParent() == collidingObject ||
            impactObject.GetParent() == collidingObject.GetParent());
        }
        catch { return false; }
    }

    public void OnAreaExit(Node2D impactObject)
    {
        if(collidingObject != null && collidingObject == (Area2D)impactObject)
        {
            collidingObject = null;
        }
    }

    public void HandleImpactEvents()
    {
        Brick hitBrick;
        Wall hitWall;
        uint collisionLayer;
        Goal hitGoal;

        if (IsImpactingBrick(out hitBrick, out collisionLayer))
        {
            HandleBrickImpact(hitBrick, collisionLayer);
            return;
        }

        GD.Print($"Ball.cs: Clearing Brick Queue");
        brickQueue.Clear();
        
        if (IsImpactingWall(out hitWall))
        {
            HandleWallImpact(hitWall);
            return;
        }
        if(IsImpactingPaddle(out collisionLayer))
        {
            HandlePaddleImpact(collisionLayer);
            return;
        }
        if(IsImpactingGoal(out hitGoal))
        {
            HandleGoalImpact(hitGoal);
            return;
        }
        
    }

    private bool IsImpactingWall(out Wall hitWall)
    {
        hitWall = null;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Wall desiredObject = (Wall)overlappingAreas[i];
                if (desiredObject != null)
                {
                    hitWall = desiredObject;
                    return true;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return false;
    }

    private void HandleWallImpact(Wall hitWall)
    {
        ReverseDirection(!hitWall.IsHorizontal);
    }

    private bool IsImpactingPaddle(out uint collisionLayer)
    {
        collisionLayer = 0;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Paddle desiredObject = (Paddle)(overlappingAreas[i].GetParent<Area2D>());
                if (desiredObject != null)
                {
                    collisionLayer = overlappingAreas[i].CollisionLayer;

                    return true;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return false;
    }

    private void HandlePaddleImpact(uint collisionLayer)
    {
        ReverseDirection(collisionLayer == 2);
    }

    private bool IsImpactingGoal(out Goal hitGoal)
    {
        hitGoal = null;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Goal desiredObject = (Goal)overlappingAreas[i];
                if (desiredObject != null)
                {
                    hitGoal = desiredObject;
                    return true;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return false;
    }

    private void HandleGoalImpact(Goal hitGoal)
    {
        gameManager.UpdateScore(hitGoal, true);
        ResetPosition();
    }

    private bool IsImpactingBrick(out Brick hitBrick, out uint collisionLayer)
    {
        hitBrick = null;
        collisionLayer = 0;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            // GD.Print($"Overlapping Areas at Index {i} for Brick Check: {overlappingAreas[i].Name}");
            try
            {
                Brick desiredObject = (Brick)(overlappingAreas[i].GetParent<Area2D>());
                if (desiredObject != null && overlappingAreas[i] == collidingObject)
                {
                    GD.Print($"Ball.cs: Is Impacting Brick");
                    hitBrick = desiredObject;
                    collisionLayer = overlappingAreas[i].CollisionLayer;
                    return true;
                }
            }
            catch (Exception ex)
            {
                continue;
            }
        }
        return false;
    }

    private void HandleBrickImpact(Brick hitBrick, uint collisionLayer)
    {
        if(brickQueue.Contains(hitBrick))
        {
            GD.Print($"Ball.cs: {hitBrick.Name} already in Brick Queue");
            return;
        }

        hitBrick.ProcessHit();
        brickQueue.Enqueue(hitBrick);
        ReverseDirection(collisionLayer == 2);
        GD.Print($"Ball.cs: Hit {hitBrick.Name}, Adding to Brick Queue\n\n");

        if(brickQueue.Count >= maxCollisionCount)
        {
            GD.Print($"Ball.cs: Removing {brickQueue.Peek().Name}");
            brickQueue.Dequeue();
        }
    }
}
