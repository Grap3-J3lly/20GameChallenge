using Godot;
using Godot.Collections;
using System;

public partial class Paddle : AnimatableBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    private BreakoutManager breakoutManager;

    [Export]
    private MeshInstance2D meshInstance;
    [Export]
    private CollisionShape2D collisionShape;

    [Export]
    private float paddleSpeed = 10f;
    private float startSpeed;

    [Export]
    private float scaleReductionAmount = .05f;
    [Export]
    private float minScale = .5f;

    private Vector2 velocity;

    // Powerup Info
    private bool superMode = false;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------
    public float PaddleSpeed { get => paddleSpeed; set => paddleSpeed = value; }
    public float StartSpeed { get => startSpeed; }

    public Vector2 Velocity { get => velocity; set => velocity = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        meshInstance = GetChild<MeshInstance2D>(0);
        startSpeed = paddleSpeed;
        breakoutManager = BreakoutManager.Instance;
        breakoutManager.RowClear += ReducePaddleSize;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        HandleMovement(delta);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        breakoutManager.RowClear -= ReducePaddleSize;
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------
    private void HandleMovement(double delta)
	{
        if(breakoutManager.GamePaused) { return; }

        float inputDir = Input.GetAxis("ui_left", "ui_right");
        float movementAmount = inputDir * paddleSpeed * (float)delta;
        velocity = Vector2.Right * movementAmount;
        KinematicCollision2D collision = MoveAndCollide(velocity, false, .08f, true);
        if (collision != null)
        {
            HandleCollision(collision, delta);
        }

    }

    public void ResetSpeed()
    {
        paddleSpeed = startSpeed;
    }

    public void Reset()
    {
        if(breakoutManager != null)
        {
            Position = breakoutManager.PaddleStartingLocation;
        }
    }

    // --------------------------------
    //		COLLISION LOGIC	
    // --------------------------------

    public void HandleCollision(KinematicCollision2D collision, double delta)
    {
        float realDelta = (float)delta;
        GodotObject collidingObject = collision.GetCollider();
        Ball ball = collidingObject as Ball;
        PowerupOrb powerupOrb = collidingObject as PowerupOrb;
        GD.Print($"Paddle.cs: Colliding with: {collidingObject.ToString()}");

        if(powerupOrb != null)
        {
            powerupOrb.RunPaddleImpact();
            powerupOrb.QueueFree();
            return;
        }
        if (ball != null)
        {            
            // Gross hack but it does what I want
            ball.Velocity += (5 * Velocity) + (ball.Velocity.Bounce(collision.GetNormal()) * realDelta);
        }
    }

    // --------------------------------
    //		DIFFICULTY LOGIC	
    // --------------------------------

    private void ReducePaddleSize()
    {
        ChangePaddleSize(-scaleReductionAmount);
    }

    public void ChangePaddleSize(float changeAmount, bool isSuper = false)
    {
        GD.Print($"Paddle.cs: Attempting to change Paddle Size");
        if (meshInstance == null || meshInstance.Scale.X <= minScale || superMode) { return; }
        Vector2 adjustedScale = new Vector2(meshInstance.Scale.X + changeAmount, meshInstance.Scale.Y);
        meshInstance.Scale = adjustedScale;
        collisionShape.Scale = adjustedScale;
        superMode = isSuper;
    }

    public void ResetPaddleSize()
    {
        if (!IsInstanceValid(this) || (meshInstance.Scale == Vector2.One && collisionShape.Scale == Vector2.One)) return;
        meshInstance.Scale = Vector2.One;
        collisionShape.Scale = Vector2.One;
        superMode = false;
        GD.Print($"Paddle.cs: Resetting Paddle Size");
    }
}
