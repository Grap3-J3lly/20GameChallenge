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
    private float paddleSpeed = 10f;
    private float startSpeed;

    [Export]
    private float scaleReductionAmount = .05f;
    [Export]
    private float minScale = .5f;

    private Vector2 velocity;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public Vector2 StartingLocation { get => startingLocation; set => startingLocation = value; } 
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
            HandleCollision(collision);
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

    public void HandleCollision(KinematicCollision2D collision)
    {
        GodotObject collidingObject = collision.GetCollider();
        Ball ball = collidingObject as Ball;


        if (ball != null)
        {

            GD.Print($"Paddle.cs: Collision Normal: {collision.GetNormal()}");
            ball.Velocity -= Velocity.Bounce(collision.GetNormal());
            // Currently, the paddle is being stopped by the ball when hit on the side, 
            // considering disabling collision on ball after hitting paddle and changing course.
        }
    }

    // --------------------------------
    //		DIFFICULTY LOGIC	
    // --------------------------------

    private void ReducePaddleSize()
    {
        GD.Print($"Paddle.cs: Attempting to reduce Paddle Size");
        if(Scale.X <= minScale) { return; }
        Vector2 adjustedScale = new Vector2(Scale.X - scaleReductionAmount, Scale.Y);
        Scale = adjustedScale;
    }

    private void ResetPaddleSize()
    {
        Scale = Vector2.One;
    }
}
