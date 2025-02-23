using Godot;
using Godot.Collections;
using System;

public partial class GameManager : Node
{
    // --------------------------------
    //		VARIABLES	
    // --------------------------------
    private UIManager uiManager;
    private bool gameOver = false;
    private bool resetting = false;

    private int score = 0;
    private int highScore = 0;

    [Export]
    private PackedScene characterScene;
    [Export]
    private PackedScene obstacleScene;

    private Character character;

    [Export]
    private int maxObstacleCount = 6;
    [Export]
    private Node2D obstacleParent;
    [Export]
    private Vector2 horizontalSpawnRange = new Vector2(20, 50);
    [Export]
    private Vector2 verticalSpawnRange = new Vector2(10, 60);

    private Array<Wall> createdObstacles = new Array<Wall>();

    private bool replaceObstacle = false;
    [Export]
    private AudioStreamMP3 score_AudioStream;
    [Export]
    private AudioStreamPlayer audioStreamPlayer;

    // --------------------------------
    //		PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }
    public bool GameOver { get => gameOver; set => gameOver = value; }
    public bool Resetting { get => resetting; set => resetting = value; }

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		Instance = this;
        uiManager = UIManager.Instance;
        gameOver = true;
        Setup();
    }

	public override void _Process(double delta)
	{
        uiManager.DisplayGameOver(!gameOver);// && !resetting);
	}

    // --------------------------------
    //		SETUP LOGIC	
    // --------------------------------

    public void Setup()
    {
        uiManager.ChangeScoreField(0, true);
        SpawnCharacter();
        HandleObstacleCreation();
    }

    private void SpawnCharacter()
    {
        character = characterScene.Instantiate<Character>();
        AddChild(character);
        character.EnterSpawnState();
    }

    public void DelayStart()
    {
        gameOver = false;
        character.EnterPlayState();
    }

    public void PlayCharacter()
    {
        character.EnterPlayState();
    }

    public void Reset()
    {
        score = 0;
        uiManager.ChangeScoreField(score);

        resetting = true;
        HandleObstacleCreation();
    }

    public void EnterGameOver()
    {
        gameOver = true;
        character.EnterDeathState();
    }

    // --------------------------------
    //		SCORING LOGIC	
    // --------------------------------

    public void UpdateScore()
    {
        ++score;
        uiManager.ChangeScoreField(score, score > highScore);
    }

    public void UpdateTimerUI(float time)
    {
        uiManager.AssignGameOverText(false, "[center]" + time.ToString());
    }

    public void RespawnObstacle()
    {
        if (score >= maxObstacleCount / 2)
        {
            createdObstacles[0].QueueFree();
            createdObstacles.RemoveAt(0);
            CreateObstacle();
        }
    }

    public void PlayScoreSound()
    {
        if(!audioStreamPlayer.Playing)
        {
            audioStreamPlayer.Stream = score_AudioStream;
            audioStreamPlayer.Play();
        }
    }

    // --------------------------------
    //		OBSTACLE SPAWN LOGIC	
    // --------------------------------

    public void HandleObstacleCreation()
    {
        if (createdObstacles.Count > 0)
        {
            foreach(Wall obstacle in createdObstacles)
            {
                obstacle.QueueFree();
            }
            createdObstacles.Clear();
        }        

        while(createdObstacles.Count < maxObstacleCount)
        {
            CreateObstacle();
        }
    }

    private void CreateObstacle()
    {
        Wall newObstacle = obstacleScene.Instantiate<Wall>();
        obstacleParent.AddChild(newObstacle);

        Vector2 newPosition = newObstacle.SpawnLocation;

        // Handle Vertical Position
        RandomNumberGenerator rand = new RandomNumberGenerator();
        float newHeight = rand.RandfRange(verticalSpawnRange.X, verticalSpawnRange.Y);
        newPosition.Y = newHeight;

        // Handle Horizontal Position
        if (createdObstacles.Count > 0)
        {
            Wall lastObstacle = createdObstacles[createdObstacles.Count - 1];
            // GD.Print(lastObstacle.SpawnLocation);
            float newHorizontalPos = rand.RandfRange(lastObstacle.SpawnLocation.X + horizontalSpawnRange.X, lastObstacle.SpawnLocation.X + horizontalSpawnRange.Y);
            newPosition.X = newHorizontalPos;
        }
        newObstacle.SpawnLocation = newPosition;
        newObstacle.Position = newPosition;
        createdObstacles.Add(newObstacle);
    }
}
