using Godot;
using System;

public partial class EnemyPaddle : Paddle
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;
	private Ball ball;

    [Export]
    private float paddleWidth = 100f;

	[Export]
	private Vector2 restLocation;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public float PaddleSpeed { get => paddleSpeed; set => paddleSpeed = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
		gameManager = GameManager.Instance;
		ball = gameManager.Ball;
    }

    public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Move(delta);
	}

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    protected override void Move(double delta)
	{
        // If Ball's X coordinate is Positive (heading towards enemy paddle), then try to follow ball's position

        // Otherwise, Reset to center
        if(ball == null) 
        {
            GD.Print("Ball is Null");
            return; 
        }
        base.Move(delta);

        movementState = CalculateMovementDirection(ball.Position);
        GD.Print("Movement Direction: " + movementState);
        GD.Print("Ball X Direction: " + ball.CurrentDirection.X);
        if (CanMove() && ball.CurrentDirection.X > 0)
        {
            GD.Print("Moving toward Ball");
            Translate(movementState, delta);
        }
        if(ball.CurrentDirection.X < 0 && Position != restLocation)
        {
            GD.Print("Moving to reset position");
            movementState = CalculateMovementDirection(restLocation);
            Translate(movementState, delta);
        }
    }

    // Returns 1 if target is below paddle, -1 if target is above paddle, 0 otherwise
    private int CalculateMovementDirection(Vector2 targetPosition)
    {
        if(targetPosition.Y > Position.Y + (paddleWidth / 2))
        {
            return 1;
        }
        if (targetPosition.Y < Position.Y - (paddleWidth / 2))
        {
            return -1;
        }
        return 0;
    }
}
