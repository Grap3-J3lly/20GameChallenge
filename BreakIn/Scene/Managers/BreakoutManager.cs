using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class BreakoutManager : Node
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

	// Managers
    [Export]
	private ObjectPool objectPool;

    // Paddle Details
    private Paddle paddle;
    [Export]
    private Vector2 paddleStartingLocation = new Vector2(576.0f, 796.0f);

    // Ball Details
	private Array<Ball> balls = new Array<Ball>();
    [Export]
	private float ballIncreaseSpeedAmount = 25f;

	// Brick Details
	private Brick[,] bricks;

    [Export]
    private Node brickParent;
    [Export]
    private Vector2 initialBrickSpawnPosition = new Vector2(66, 138);
    [Export] 
	private Vector2 distancePerBrick = new Vector2(102, 23);
    [Export]
    private int maxBricksPerRow = 11;
    [Export]
    private int maxRowCount_Easy = 2;
    [Export]
    private int maxRowCount_Medium = 3;
    [Export]
    private int maxRowCount_Hard = 5;

	// Powerup Details
	[Export]
	private PowerUpManager powerUpManager;
    [Export]
    private Node powerupParent;
	[Export]
	private float powerupSpawnRate = .25f;

    // General Game Settings
	[Export]
	private int playerMaxLives = 3;
	[Export]
	private int difficulty = 1;

    private int playerScore = 0;
	private int playerLives = 3;
	private bool gameOver = false;

	// Signals
	[Signal]
	public delegate void RowClearEventHandler();

	// --------------------------------
	//			PROPERTIES	
	// --------------------------------

	public static BreakoutManager Instance { get; private set; }

	public PackedScene PaddleScene { get => paddleScene; }
	public PackedScene BallScene { get => ballScene; }

	public Paddle Paddle { get => paddle; }
	public Array<Ball> Balls { get => balls; }
    public Brick[,] Bricks { get { return bricks; } }
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
		difficulty = GameManager.Instance.CurrentDifficulty;
		GD.Print($"BreakoutManager.cs: Difficulty from Game Manager: {difficulty}");
		playerScore = 0;
		playerLives = playerMaxLives;
		gameOver = false;

		SpawnPaddle(paddleStartingLocation);
		SpawnBall();
		SpawnBricks(difficulty);
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

	public void SpawnBricks(int currentDifficulty)
	{
		int columnCount = 0;

		switch(currentDifficulty)
		{
			case 1: columnCount = maxRowCount_Easy;
				break; 
			case 2: columnCount = maxRowCount_Medium;
				break;
			case 3: columnCount = maxRowCount_Hard;
				break;

			default:
				GD.PrintErr($"GameManager.cs: Invalid Difficulty Provided. Defaulting to Easy");
				columnCount = maxRowCount_Easy;
				break;
				
		}

		Vector2I gridCount = new Vector2I(maxBricksPerRow, columnCount);
		// Manually assigning difficulty to easy, need to change per level
		bricks = objectPool.SpawnObjectsInGrid<Brick>(brickScene, initialBrickSpawnPosition, gridCount, distancePerBrick, brickParent);
		for(int y = 0; y < bricks.GetLength(1); y++)
		{
			for(int x = 0;  x < bricks.GetLength(0); x++)
			{
                bricks[x, y].GridPosition = new Vector2I(x, y);
                bricks[x, y].LayerCount = gridCount.Y - y - 1;
                bricks[x, y].RowID = y;
			}
		}
    }

    public void SpawnPowerUpOrb(Vector2 position)
	{
		RandomNumberGenerator random = new RandomNumberGenerator();
		float spawnChance = random.Randf();

		if (spawnChance <= powerupSpawnRate)
		{
			objectPool.SpawnObjectAtPosition<PowerupOrb>(powerupOrbScene, position, powerupParent);
		}
	}

}
