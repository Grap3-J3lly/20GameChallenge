using Godot;
using Godot.Collections;
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

	// Major Objects
	private Paddle paddle;

	private Array<Ball> balls = new Array<Ball>();
	[Export]
	private ObjectPool objectPool;
	[Export]
	private PowerUpManager powerUpManager;

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

	// Signals
	[Signal]
	public delegate void RowClearEventHandler();

	// --------------------------------
	//			PROPERTIES	
	// --------------------------------

	public static GameManager Instance { get; private set; }

	public PackedScene PaddleScene { get => paddleScene; }
	public PackedScene BallScene { get => ballScene; }

	public Paddle Paddle { get => paddle; }
	public Array<Ball> Balls { get => balls; }
	public ObjectPool ObjectPool { get => objectPool; }
	public PowerUpManager PowerUpManager { get => powerUpManager; }

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

		foreach (Ball ball in Balls)
		{
			ball.BallSpeed -= ballIncreaseSpeedAmount;
		}
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

		Ball newBall = ballScene.Instantiate<Ball>();
		balls.Add(newBall);
		paddle.AddChild(newBall);
        newBall.ResetOnPaddle(paddle);
		// ball.Position = Vector2.Up * 25; // Need to do this elsewhere

		// Manually assigning difficulty to easy, need to change per level
		objectPool.SpawnBricks(1);
	}

	// Reset handles the ball hitting the goal, but the game not being over
	public void Reset()
	{
		balls[0].ResetOnPaddle(paddle);
		paddle.Reset();
		ReduceLife();
		ReduceScore();
	}

	// Restart handles the game fully ending
    private void RestartGame()
    {
		playerLives = playerMaxLives;
		gameOver = false;
		balls[0].ResetSpeed();
		balls[0].Visible = true;
    }

    // --------------------------------
    //		GENERAL LOGIC	
    // --------------------------------

	public void TriggerObjectiveSuccess()
	{
        playerScore++;

		foreach (Ball ball in balls)
		{
			ball.BallSpeed += ballIncreaseSpeedAmount;
		}
		GD.Print($"GameManager.cs: New Ball Speed: {balls[0].BallSpeed}");
    }

    public void HandleGeneralInput()
	{
		if(Input.IsActionJustPressed("ui_accept") && gameOver)
		{
			RestartGame();
		}
		if(Input.IsActionJustPressed("ui_accept") && !gameOver && balls[0].Velocity == Vector2.Zero)
		{
			balls[0].Fire(paddle.Velocity);
		}
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			// Opens Menu
			GD.Print($"GameManager.cs: Triggering Powerup");
			// powerUpManager.Debug_TriBall();
			// powerUpManager.Debug_SuperBall();
			// powerUpManager.Debug_SuperWide();
			// powerUpManager.Debug_PaddleSpeed();
			// powerUpManager.Debug_Shield();
		}
	}

}
