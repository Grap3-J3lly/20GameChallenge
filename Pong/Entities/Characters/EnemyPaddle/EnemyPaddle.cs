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
        if(ball == null) { return; }
        base.Move(delta);

        float movementDirection = CalculateMovementDirection(ball.Position);
        movementState = Mathf.RoundToInt(movementDirection);

        if (CanMove() && ball.CurrentDirection.X > 0)
        {
            Translate(movementDirection, delta);
        }
        if(ball.CurrentDirection.X < 0 && Position != restLocation)
        {
            movementDirection = CalculateMovementDirection(restLocation);
            Translate(movementDirection, delta);
        }
    }

    private float CalculateMovementDirection(Vector2 targetPosition)
    {
        if(targetPosition.Y > Position.Y + (paddleWidth / 2))
        {
            return 1.0f;
        }
        if (targetPosition.Y < Position.Y - (paddleWidth / 2))
        {
            return -1.0f;
        }
        return 0.0f;
    }
}
