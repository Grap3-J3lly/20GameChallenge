using Godot;
using Godot.Collections;
using System;

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
        HandleImpactEvents();
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

    // --------------------------------
    //		    IMPACT LOGIC	
    // --------------------------------

    private void HandleImpactEvents()
    {
        if (IsImpactingWall())
        {
            movementState = new Vector2(movementState.X, -movementState.Y);
            currentDirection.Y *= -1;
        }
        if(IsImpactingPaddle(out bool isPlayerPaddle))
        {
            float xVal = 0;
            if(isPlayerPaddle) xVal = 1; 
            else xVal = -1;

            movementState = new Vector2(xVal, movementState.Y);

            currentDirection.X *= -1;
        }
        Goal hitGoal;
        if(IsImpactingGoal(out hitGoal))
        {
            gameManager.UpdateScore(hitGoal, true);
            ResetPosition();
        }
    }

    private bool IsImpactingWall()
    {
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Wall potentialWall = (Wall)overlappingAreas[i];
                if (potentialWall != null)
                {
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

    private bool IsImpactingPaddle(out bool isPlayerPaddle)
    {
        isPlayerPaddle = false;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Paddle potentialPaddle = (Paddle)overlappingAreas[i];
                if (potentialPaddle != null)
                {
                    try
                    {
                        if ((PlayerPaddle)potentialPaddle != null) isPlayerPaddle = true;                    
                    }
                    catch (Exception ex)
                    {
                        isPlayerPaddle = false;
                    }
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

    private bool IsImpactingGoal(out Goal hitGoal)
    {
        hitGoal = null;
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Goal potentialGoal = (Goal)overlappingAreas[i];
                if (potentialGoal != null)
                {
                    hitGoal = potentialGoal;
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
}
