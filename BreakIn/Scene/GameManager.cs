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

	public void ReduceLife()
	{
		--playerLives;
		GD.Print($"GameManager.cpp: Reducing Lives to: {playerLives}");
		ball.BallSpeed += ballIncreaseSpeedAmount;
	}

	public bool DetermineGameOver()
	{
		return playerLives <= 0;
	}

	// --------------------------------
	//		GENERAL LOGIC	
	// --------------------------------

	private void Setup()
	{
		playerScore = 0;
		playerLives = playerMaxLives;
		gameOver = false;

		paddle = paddleScene.Instantiate<Paddle>();
		objectPool.AddChild(paddle);

		ball = ballScene.Instantiate<Ball>();
		ball.Position = Vector2.Up * 25; // Need to do this elsewhere
		paddle.AddChild(ball);
	}

	// Reset handles the ball hitting the goal, but the game not being over
	public void Reset()
	{
		// Need Reset to also reset ball position
		ball.Reparent(paddle);
		ReduceLife();
	}

	// Restart handles the game fully ending
    private void RestartGame()
    {
		playerLives = playerMaxLives;
		gameOver = false;
		ball.ResetSpeed();
		ball.Visible = true;
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

		}
	}

}
