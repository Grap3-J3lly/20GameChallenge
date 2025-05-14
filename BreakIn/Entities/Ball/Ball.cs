using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Ball : CharacterBody2D
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
	private Vector2 currentDirection;

    [Export]
    private int maxCollisionCount = 3;

    private Queue<KinematicCollision2D> collisionsWaiting = new Queue<KinematicCollision2D>();

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
        Setup();
        
	}

	public override void _PhysicsProcess(double delta)
	{
        if(gameManager.GameOver)
        {
            return;
        }

		HandleMovement(delta);
        HandleCollision();
	}

    // --------------------------------
    //		    SETUP LOGIC	
    // --------------------------------

    private void Setup()
    {
        currentDirection = PickRandomDirection();
        startSpeed = ballSpeed;
        Velocity = currentDirection * ballSpeed;
    }

    private Vector2 PickRandomDirection()
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		Vector2 randomDirection = new Vector2(rng.RandfRange(-1f, 1f), rng.RandfRange(-1f, 1f));
		return randomDirection;
	}

    // --------------------------------
    //		TRANSFORM LOGIC	
    // --------------------------------

    private void HandleMovement(double delta)
    {
        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
        if (collision != null)
        {
            collisionsWaiting.Enqueue(collision);
        }
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

    public void HandleCollision()
    {
        if(collisionsWaiting.Count <= 0) { return; }
        KinematicCollision2D currentCollision = collisionsWaiting.Dequeue();
        GD.Print($"Ball.cs: Colliding Body's Godot Object: {currentCollision.GetCollider()}");
        GodotObject collidingObject = currentCollision.GetCollider();

        Paddle potentialPaddle = collidingObject as Paddle;
        if(potentialPaddle != null)
        {
            HandlePaddleImpact(currentCollision);
        }

        Wall potentialWall = collidingObject as Wall;
        if(potentialWall != null)
        {
            HandleWallImpact(currentCollision);
        }
        
        Brick potentialBrick = collidingObject as Brick;
        if (potentialBrick != null)
        {
            HandleBrickImpact(currentCollision, potentialBrick);
        }

        Goal potentialGoal = collidingObject as Goal;
        if(potentialGoal != null)
        {
            HandleGoalImpact(currentCollision);
        }

    }

    private void HandlePaddleImpact(KinematicCollision2D collisionInfo) //, Paddle hitPieceOfPaddle)
    {

        GD.Print($"Ball.cs: Colliding with a Paddle."); // Velocity Before: {Velocity}");
        Velocity = Velocity.Bounce(collisionInfo.GetNormal());
        Velocity += collisionInfo.GetTravel();
        // GD.Print($"Ball.cs: Colliding with a Paddle. Velocity After: {Velocity}");
    }

    private void HandleWallImpact(KinematicCollision2D collisionInfo) //, Wall hitWall)
    {
        GD.Print($"Ball.cs: Colliding with a wall.");
        Velocity = Velocity.Bounce(collisionInfo.GetNormal());
    }

    private void HandleBrickImpact(KinematicCollision2D collisionInfo, Brick hitBrick)
    {
        GD.Print($"Ball.cs: Colliding with a Brick.");
        Velocity = Velocity.Bounce(collisionInfo.GetNormal());
        hitBrick.ProcessHit();
    }

    private void HandleGoalImpact(KinematicCollision2D collisionInfo) //, Goal hitGoal)
    {
        GD.Print($"Ball.cs: Colliding with a Goal.");
        gameManager.UpdateScore(true);
        ResetPosition();
    }
}
