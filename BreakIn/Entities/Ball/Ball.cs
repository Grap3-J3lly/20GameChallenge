using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Ball : CharacterBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    protected BreakoutManager breakoutManager;
    [Export]
    private Vector2 startingLocation = new Vector2();
    [Export]
	private float ballSpeed = 10f;
    private float startSpeed;
	protected Vector2 currentDirection = Vector2.Zero;

    [Export]
    private int maxCollisionCount = 3;

    protected Queue<KinematicCollision2D> collisionsWaiting = new Queue<KinematicCollision2D>();

    [Export]
    private Array<Color> colors = new Array<Color>();

    // Powerup Info
    private bool superMode = false;
    private int brickBreakCount = 0;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public float BallSpeed { get => ballSpeed; set => ballSpeed = value; }
    public Vector2 CurrentDirection { get => currentDirection; }

    public bool SuperMode { get => superMode; set => superMode = value; }
    public int BrickBreakCount { get => brickBreakCount; set => brickBreakCount = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        breakoutManager = BreakoutManager.Instance;
        colors = breakoutManager.LevelColors;
        Setup();
        
	}

	public override void _PhysicsProcess(double delta)
	{
        if(breakoutManager.GameOver)
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
        // currentDirection = PickRandomDirection();
        startSpeed = ballSpeed;
        Velocity = currentDirection * ballSpeed;
    }

    public void AssignColorByIndex(int index)
    {
        Modulate = colors[index];
    }

    // --------------------------------
    //		TRANSFORM LOGIC	
    // --------------------------------

    protected void HandleMovement(double delta)
    {
        KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);
        if (collision != null)
        {
            collisionsWaiting.Enqueue(collision);
        }
    }

    public void Fire(Vector2 paddleVel)
    {
        Reparent(breakoutManager.ObjectPool);
        // Setup rng between .1 and .5 for x val of PaddleVelocity
        Velocity += (2 * ballSpeed * Vector2.Up) + new Vector2(paddleVel.X * (.5f * ballSpeed), paddleVel.Y);
        // Velocity += ballSpeed * paddleVel;
        GD.Print($"Ball.cs: Velocity: {Velocity}, Ball Speed: {ballSpeed}, PaddleVel: {paddleVel}");
    }

    public void ResetOnPaddle(Node newParent)
    {
        if(GetParent() != null)
        {
            Reparent(newParent);
        }
        Position = Vector2.Up * 30f;
        Velocity = Vector2.Zero;
    }

    // --------------------------------
    //		    IMPACT LOGIC	
    // --------------------------------

    public virtual void HandleCollision()
    {
        if(collisionsWaiting.Count <= 0) { return; }
        KinematicCollision2D currentCollision = collisionsWaiting.Dequeue();
        GD.Print($"Ball.cs: Colliding Body's Godot Object: {currentCollision.GetCollider()}");
        GodotObject collidingObject = currentCollision.GetCollider();

        Paddle potentialPaddle = collidingObject as Paddle;
        Brick potentialBrick = collidingObject as Brick;
        Goal potentialGoal = collidingObject as Goal;
        Wall potentialWall = collidingObject as Wall;
        Ball potentialBall = collidingObject as Ball;

        AudioManager.Instance.PlaySFX(AudioManager.SFXType.OnHit);

        if (potentialPaddle != null)
        {
            HandlePaddleImpact(currentCollision);
        }
        if (potentialBrick != null)
        {
            HandleBrickImpact(currentCollision, potentialBrick);
        }
        if (potentialWall != null)
        {
            HandleWallImpact(currentCollision);
        }
        if (potentialGoal != null)
        {
            HandleGoalImpact(currentCollision);
        }
        if(potentialBall != null)
        {
            HandleDefaultImpact(currentCollision);
        }

    }

    private void HandleDefaultImpact(KinematicCollision2D collisionInfo)
    {
        // GD.Print($"Ball.cs: Colliding with a wall.");
        Velocity = Velocity.Bounce(collisionInfo.GetNormal());
    }
    protected virtual void HandlePaddleImpact(KinematicCollision2D collisionInfo)
    {

        GD.Print($"Ball.cs: Colliding with a Paddle."); // Velocity Before: {Velocity}");
        HandleDefaultImpact(collisionInfo);
        Velocity += collisionInfo.GetColliderVelocity() * .1f;
        GD.Print($"Ball.cs: Colliding with a Paddle. Velocity {Velocity}");
    }

    private void HandleBrickImpact(KinematicCollision2D collisionInfo, Brick hitBrick)
    {
        GD.Print($"Ball.cs: Colliding with a Brick.");
        if(!superMode)
        {
            HandleDefaultImpact(collisionInfo);
        }
        hitBrick.ProcessHit(superMode);
        brickBreakCount++;
    }

    private void HandleWallImpact(KinematicCollision2D collisionInfo)
    {
        HandleDefaultImpact(collisionInfo);
        ResetSuperMode();
    }

    private void HandleGoalImpact(KinematicCollision2D collisionInfo)
    {
        GD.Print($"Ball.cs: Colliding with a Goal.");
        if(breakoutManager.Balls.Count > 1) 
        {
            breakoutManager.Balls.Remove(this);
            QueueFree();
            return;
        }
        breakoutManager.Reset();
    }

    // --------------------------------
    //		    POWERUP LOGIC	
    // --------------------------------

    private void ResetSuperMode()
    {
        if (superMode && brickBreakCount > 0)
        {
            superMode = false;
            brickBreakCount = 0;
        }
    }

}
