using Godot;
using Godot.Collections;
using System;

public partial class BreakoutManager : Node
{
	// --------------------------------
	//			VARIABLES	
	// --------------------------------

	// Packed Scenes
	[ExportGroup("Packed Scenes")]
	[Export]
	private PackedScene paddleScene;
	[Export]
	private PackedScene ballScene;
    [Export]
    private PackedScene brickScene;
    [Export]
    private PackedScene powerupOrbScene;

    // Managers
    [ExportGroup("Managers")]
    [Export]
	private ObjectPool objectPool;
	[Export]
	private PowerUpManager powerUpManager;
	[Export]
	private UIManager uiManager;

    // Paddle Details
    [ExportGroup("Paddle Details")]
    private Paddle paddle;
    [Export]
    private Vector2 paddleStartingLocation = new Vector2(576.0f, 796.0f);

    // Ball Details
    [ExportGroup("Ball Details")]
    private Array<Ball> balls = new Array<Ball>();
    [Export]
	private float ballIncreaseSpeedAmount = 25f;
	private Array<Vector2> previousBallVelocities = new Array<Vector2>();

    // Brick Details

    [ExportGroup("Brick Details")]
    [Export]
    private Node brickParent;
    private Brick[,] bricks;
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
    [ExportGroup("Powerup Details")]
    [Export]
    private Node powerupParent;
	[Export]
	private float powerupSpawnRate = .25f;
	private Array<PowerupOrb> activePowerups = new Array<PowerupOrb>();
	private Array<Vector2> previousPowerupVelocities = new Array<Vector2>();

    // General Game Settings
    [ExportGroup("General Game Settings")]
    [Export]
	private int playerMaxLives = 3;
	[Export]
	private int difficulty = 1;

    private int playerScore = 0;
	private int highScore = -1;
	private int playerLives = 3;
	private bool gameOver = false;
	private bool gamePaused = false;

	private Dictionary<string, float> scores = new Dictionary<string, float>();

	// Environment Settings
	[ExportGroup("Environment Settings")]
	[Export]
	private Node environmentParent;
	private ColorRect currentEnvironment;
	[Export]
	private Array<PackedScene> environments = new Array<PackedScene>();
	[Export]
	private GameBoard board;
	// Signals
	[Signal]
	public delegate void RowClearEventHandler();

	// --------------------------------
	//			PROPERTIES	
	// --------------------------------

	public static BreakoutManager Instance { get; private set; }

    // Packed Scenes
    public PackedScene PaddleScene { get => paddleScene; }
	public PackedScene BallScene { get => ballScene; }

    // Managers
    public ObjectPool ObjectPool { get => objectPool; }
	public PowerUpManager PowerUpManager { get => powerUpManager; }

    // Paddle Details
    public Paddle Paddle { get => paddle; }
	public Vector2 PaddleStartingLocation { get => paddleStartingLocation; }

    // Ball Details
    public Array<Ball> Balls { get => balls; }
    
	// Brick Details
    public Brick[,] Bricks { get { return bricks; } }

	// Powerup Details
	public Array<PowerupOrb> ActrivePowerups { get => activePowerups; }

    // General Game Settings
    public int PlayerScore { get => playerScore; set => playerScore = value; }
	public int HighScore { get => highScore; set => highScore = value; }
	public int PlayerLives { get => playerLives; set => playerLives = value; }
	public bool GameOver { get => gameOver; }
	public bool GamePaused { get => gamePaused; }

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
		HandleGameOver();
	}

	// --------------------------------
	//		SETUP LOGIC	
	// --------------------------------

	public void Setup()
	{
        difficulty = GameManager.Instance.CurrentDifficulty;
		LoadHighScore();

        ClearBalls();
        ClearPowerups();
        ClearBricks();
		if(paddle != null) paddle.Free();
		powerUpManager.ResetPowerups();

        GD.Print($"BreakoutManager.cs: Difficulty from Game Manager: {difficulty}");
        playerScore = 0;
        playerLives = playerMaxLives;
        gameOver = false;
        gamePaused = true;

        HandlePauseGame();

        SpawnPaddle(paddleStartingLocation);
        SpawnBricks(difficulty);
        SpawnBall();
        paddle.Reset();

        SetupEnvironment(difficulty);
    }

    // Reset handles the ball hitting the goal, but the game not being over
    public void Reset()
	{
		balls[0].ResetOnPaddle(paddle);
		paddle.Reset();
		ReduceLife();
		ReduceScore();
	}

	private void SetupEnvironment(int currentDifficulty)
	{
		currentEnvironment = environments[currentDifficulty - 1].Instantiate<ColorRect>();
		environmentParent.AddChild(currentEnvironment);

		board.AssignTextures(currentDifficulty - 1);
		paddle.AssignColorByIndex(currentDifficulty - 1);
		
		foreach(Ball ball in balls)
		{
			ball.AssignColorByIndex(currentDifficulty - 1);
		}
	}

	private void LoadHighScore()
	{
		scores = SaveSystem.GetDataItem("Scores", "scores", defaultValue: new Dictionary<string, float>()
		{
            { "Easy", 0 },
            { "Medium", 0 },
            { "Hard", 0 }
        });

        switch (difficulty)
        {
            case 1:
				highScore = (int)scores["Easy"];
                break;
            case 2:
                highScore = (int)scores["Medium"];
                break;
            case 3:
                highScore = (int)scores["Hard"];
                break;
            default:
                GD.PrintErr($"BreakoutManager.cs: Attempted to Load invalid difficulty");
                break;
        }
		GD.Print($"BreakoutManager.cs: High Score Loaded: {highScore}");
    }

    // --------------------------------
    //		SCORING LOGIC	
    // --------------------------------

    public void ReduceScore(int amount = 1)
    {
        if (playerScore <= 0) { return; }
        playerScore -= amount;
        GD.Print($"BreakoutManager.cpp: Reducing Score to: {playerScore}");
    }

    public void ReduceLife()
    {
        --playerLives;

        foreach (Ball ball in Balls)
        {
            ball.BallSpeed -= ballIncreaseSpeedAmount;
        }
        GD.Print($"BreakoutManager.cpp: Reducing Lives to: {playerLives}");
    }

    public bool DetermineGameOver()
    {
        return playerLives <= 0 || AreAllBricksNull();
    }

	private void HandleGameOver()
	{
		if(!gameOver || gamePaused) { return; }
		gamePaused = true;
		Pause();
		if(playerLives <= 0)
		{
			uiManager.PopupManager.OpenPopup(PopupManager.PopupType.GameLose);
		}
		else
		{
            uiManager.PopupManager.OpenPopup(PopupManager.PopupType.GameWin, difficulty >= 3);
        }
		uiManager.ToggleArea(1, true);

		if(playerScore > highScore)
		{
			switch(difficulty)
			{
				case 1:
					scores["Easy"] = playerScore;
					break;
				case 2:
                    scores["Medium"] = playerScore;
                    break;
				case 3:
                    scores["Hard"] = playerScore;
                    break;
				default: GD.PrintErr($"BreakoutManager.cs: Attempted to Save invalid difficulty");
					break;
			}
			SaveSystem.AddDataItem("Scores", "scores", scores);
			SaveSystem.SaveData("Scores");

			GD.Print($"BreakoutManager.cs: Saved Score ({playerScore}) to file");
		}
	}

	private bool AreAllBricksNull()
	{
		foreach (Brick brick in Bricks)
		{
			if(brick != null)
			{
				return false;
			}
		}

		return true;
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
		GD.Print($"BreakoutManager.cs: New Ball Speed: {balls[0].BallSpeed}");
    }

    public void HandleGeneralInput()
	{
		if(Input.IsActionJustPressed("ui_accept") && gameOver && !gamePaused)
		{
			// RestartGame();
		}
		if(Input.IsActionJustPressed("ui_accept") && !gameOver && balls[0].Velocity == Vector2.Zero && !gamePaused)
		{
			balls[0].Fire(paddle.Velocity);
		}
		if(Input.IsActionJustPressed("ui_cancel"))
		{
			powerUpManager.Debug_SuperWide();
			// HandlePauseGame();
		}
	}

	public void HandlePauseGame()
	{
        gamePaused = !gamePaused;

        if (gamePaused)
        {
            Pause();
            uiManager.PopupManager.OpenPopup(PopupManager.PopupType.GamePause);
        }
        else
        {
            Unpause();
        }
        uiManager.ToggleArea(1, gamePaused);
    }

	private void Pause()
	{
		previousBallVelocities.Clear();
		previousPowerupVelocities.Clear();
		for (int index = 0; index < balls.Count; index++)
		{
			GD.Print($"BreakoutManager: Previous Velocity: {balls[index].Velocity}");
			previousBallVelocities.Add(balls[index].Velocity);
			balls[index].Velocity = Vector2.Zero;
            GD.Print($"BreakoutManager: Current Velocity: {balls[index].Velocity}");
        }
		for (int index = 0; index < activePowerups.Count; index++)
		{
			previousPowerupVelocities.Add(activePowerups[index].Velocity);
			activePowerups[index].Velocity = Vector2.Zero;
		}
	}

	private void Unpause()
	{
		if (balls == null) return;
        for (int index = 0; index < balls.Count; index++)
        {
            GD.Print($"BreakoutManager: Stored Velocity: {previousBallVelocities[index]}");
            balls[index].Velocity = previousBallVelocities[index];
            GD.Print($"BreakoutManager: New Velocity: {balls[index].Velocity}");
        }
        for (int index = 0; index < activePowerups.Count; index++)
        {
			activePowerups[index].Velocity = previousPowerupVelocities[index];
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
				GD.PrintErr($"BreakoutManager.cs: Invalid Difficulty Provided. Defaulting to Easy");
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
				bricks[x, y].ChangeBrickLayer(gridCount.Y - y - 1);
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
			PowerupOrb newOrb = objectPool.SpawnObjectAtPosition<PowerupOrb>(powerupOrbScene, position, powerupParent);
			activePowerups.Add(newOrb);
		}
	}

    // --------------------------------
    //			CLEAR LOGIC	
    // --------------------------------

	public void ClearBalls()
	{
		if (balls == null) return;
		foreach (Ball ball in balls)
		{
			ball.Free();
		}
		balls.Clear();
	}

	public void ClearBricks()
	{
		if (bricks == null) return;
		foreach(Brick brick in bricks)
		{
			if(brick != null)
			{
				brick.QueueFree();
			}
		}
		bricks = null;
	}

    public void ClearPowerups()
	{
		if(activePowerups == null) return;
		foreach(PowerupOrb powerupOrb in activePowerups)
		{
			powerupOrb.QueueFree();
		}
		activePowerups.Clear();
	}
}
