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
    [Export]
    private PackedScene brickScene;
    [Export]
    private PackedScene powerupOrbScene;


    // Major Objects
    private Paddle paddle;

	private Array<Ball> balls = new Array<Ball>();
    private Array<Brick> bricks = new Array<Brick>();
    [Export]
	private ObjectPool objectPool;
	[Export]
	private PowerUpManager powerUpManager;

    [Export]
    private Node brickParent;
    [Export]
    private Node powerupParent;

    // Settings
    [Export]
	private float ballIncreaseSpeedAmount = 25f;
	[Export]
	private float enemyIncreaseSpeedAmount = 10f;
	[Export]
	private int playerMaxLives = 3;
    [Export]
    private Vector2 paddleStartingLocation = new Vector2(576.0f, 796.0f);
    [Export]
    private Vector2 initialBrickSpawnPosition = new Vector2(66, 138);
    [Export] 
	private Vector2 distancePerBrick = new Vector2(102, 23);

    [Export]
    private int maxRowCount_Easy = 2;
    [Export]
    private int maxRowCount_Medium = 3;
    [Export]
    private int maxRowCount_Hard = 5;

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
    public Array<Brick> Bricks { get { return bricks; } }
    public ObjectPool ObjectPool { get => objectPool; }
	public PowerUpManager PowerUpManager { get => powerUpManager; }

	public Vector2 PaddleStartingLocation { get => paddleStartingLocation; }

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

		SpawnPaddle(paddleStartingLocation);
		SpawnBall();
		SpawnBricks();
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

    // --------------------------------
    //			SPAWN LOGIC	
    // --------------------------------

	public void SpawnPaddle(Vector2 startingLocation)
	{
		paddle = objectPool.SpawnObjectAtPosition<Paddle>(paddleScene, startingLocation, objectPool);
    }

	public void SpawnBall()
	{
		Ball newBall = objectPool.SpawnObjectAtPosition<Ball>(ballScene, Vector2.Zero, paddle);
        balls.Add(newBall);
        newBall.ResetOnPaddle(paddle);
    }

	public void SpawnBricks()
	{
		Vector2I gridCount = new Vector2I(11, 2);
		// Manually assigning difficulty to easy, need to change per level
		Brick[,] newBricks = objectPool.SpawnObjectsInGrid<Brick>(brickScene, initialBrickSpawnPosition, gridCount, distancePerBrick, brickParent);
		for(int y = 0; y < newBricks.GetLength(1); y++)
		{
			for(int x = 0;  x < newBricks.GetLength(0); x++)
			{
				newBricks[x, y].GridPosition = new Vector2I(x, y);
				newBricks[x, y].LayerCount = gridCount.Y - y - 1;
				newBricks[x, y].RowID = y;
			}
			// Paddle is reducing in size way too soon, need to verify RowID's
		}
    }

    public void SpawnPowerUpOrb(Vector2 position)
	{
		objectPool.SpawnObjectAtPosition<PowerupOrb>(powerupOrbScene, position, powerupParent);
	}

}
