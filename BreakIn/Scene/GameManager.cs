using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
	//			VARIABLES	
	// --------------------------------

	// Packed Scenes
	[Export]
	private PackedScene paddleScene;
	[Export]
	private PackedScene ballScene;
	[Export]
	private PackedScene brickScene;

	// Major Objects
	private Paddle paddle;
	private Ball ball;
	[Export]
	private Node objectPool;

	// Settings
	[Export]
	private float ballIncreaseSpeedAmount = 25f;
	[Export]
	private float enemyIncreaseSpeedAmount = 10f;
	[Export]
	private int playerMaxLives = 3;

	// Hidden Settings
	private int playerScore = 0;
	private int playerLives = 3;
	private bool gameOver = false;

	// --------------------------------
	//			PROPERTIES	
	// --------------------------------

	public static GameManager Instance { get; private set; }

	public Ball Ball { get => ball; }
	public Node ObjectPool { get => objectPool; }

	public int PlayerScore { get => playerScore; set => playerScore = value; }
	public int PlayerLives { get => playerLives; set => playerLives = value; }
	public bool GameOver { get => gameOver; }

	// --------------------------------
	//		STANDARD FUNCTIONS	
	// --------------------------------

	public override void _Ready()
	{
		Instance = this;
		Setup();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		HandleGeneralInput();
		gameOver = DetermineGameOver();
	}

	// --------------------------------
	//		SCORING LOGIC	
	// --------------------------------

	public void ReduceScore(int amount = 1)
	{
		if(playerScore <= 0) { return; }
		playerScore -= amount;
        GD.Print($"GameManager.cpp: Reducing Score to: {playerScore}");
    }

	public void ReduceLife()
	{
		--playerLives;
		ball.BallSpeed -= ballIncreaseSpeedAmount;
		GD.Print($"GameManager.cpp: Reducing Lives to: {playerLives}");
	}

	public bool DetermineGameOver()
	{
		return playerLives <= 0;
	}

	// --------------------------------
	//		SETUP LOGIC	
	// --------------------------------

	private void Setup()
	{
		playerScore = 0;
		playerLives = playerMaxLives;
		gameOver = false;

		paddle = paddleScene.Instantiate<Paddle>();
		objectPool.AddChild(paddle);

		ball = ballScene.Instantiate<Ball>();
		paddle.AddChild(ball);
		ball.ResetOnPaddle(paddle);
		// ball.Position = Vector2.Up * 25; // Need to do this elsewhere
	}

	// Reset handles the ball hitting the goal, but the game not being over
	public void Reset()
	{
		ball.ResetOnPaddle(paddle);
		paddle.Reset();
		ReduceLife();
		ReduceScore();
	}

	// Restart handles the game fully ending
    private void RestartGame()
    {
		playerLives = playerMaxLives;
		gameOver = false;
		ball.ResetSpeed();
		ball.Visible = true;
    }

    // --------------------------------
    //		GENERAL LOGIC	
    // --------------------------------

	public void TriggerObjectiveSuccess()
	{
        playerScore++;
		ball.BallSpeed += ballIncreaseSpeedAmount;
		GD.Print($"GameManager.cs: New Ball Speed: {ball.BallSpeed}");
    }

    public void HandleGeneralInput()
	{
		if(Input.IsActionJustPressed("ui_accept") && gameOver)
		{
			RestartGame();
		}
		if(Input.IsActionJustPressed("ui_accept") && !gameOver && ball.Velocity == Vector2.Zero)
		{
			ball.Fire(paddle.Velocity);
		}
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			// Opens Menu
		}
	}

}
