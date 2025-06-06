using Godot;
using Godot.Collections;
using System;

public partial class Paddle : AnimatableBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    [Export]
    private Vector2 startingLocation = new Vector2(576.0f, 796.0f);
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

    public Vector2 StartingLocation { get => startingLocation; set => startingLocation = value; } 
    public float PaddleSpeed { get => paddleSpeed; set => paddleSpeed = value; }
    public float StartSpeed { get => startSpeed; }

    public Vector2 Velocity { get => velocity; set => velocity = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        startSpeed = paddleSpeed;
        Position = startingLocation;
        GameManager.Instance.RowClear += ReducePaddleSize;
	}

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        HandleMovement(delta);
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------
    private void HandleMovement(double delta)
	{
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
        Position = startingLocation;
    }

    // --------------------------------
    //		COLLISION LOGIC	
    // --------------------------------

    public void HandleCollision(KinematicCollision2D collision, double delta)
    {
        float realDelta = (float)delta;
        GodotObject collidingObject = collision.GetCollider();
        Ball ball = collidingObject as Ball;


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
        if (meshInstance.Scale.X <= minScale || superMode) { return; }
        Vector2 adjustedScale = new Vector2(meshInstance.Scale.X + changeAmount, meshInstance.Scale.Y);
        meshInstance.Scale = adjustedScale;
        collisionShape.Scale = adjustedScale;
        superMode = isSuper;
    }

    public void ResetPaddleSize()
    {
        meshInstance.Scale = Vector2.One;
        collisionShape.Scale = Vector2.One;
        superMode = false;
        GD.Print($"Paddle.cs: Resetting Paddle Size");
    }
}
