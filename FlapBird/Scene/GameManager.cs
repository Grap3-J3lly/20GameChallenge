using Godot;
using Godot.Collections;
using System;

public partial class GameManager : Node
{
    // --------------------------------
    //		VARIABLES	
    // --------------------------------
    private UIManager uiManager;
    private bool gameRunning = true;
    private bool resetting = false;

    private int score = 0;
    private int highScore = 0;

    [Export]
    private Character character;
    [Export]
    private Array<Wall> walls;

    // --------------------------------
    //		PROPERTIES	
    // --------------------------------
    public static GameManager Instance { get; private set; }
    public bool GameRunning { get => gameRunning; set => gameRunning = value; }
    public bool Resetting { get => resetting; set => resetting = value; }

    // --------------------------------
    //		STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		Instance = this;
        uiManager = UIManager.Instance;
        uiManager.ChangeScoreField(0, true);
	}

	public override void _Process(double delta)
	{
        if (!gameRunning)
        {
            uiManager.AssignGameOverText();
        }
        uiManager.DisplayGameOver(gameRunning && !resetting);
	}

    // --------------------------------
    //		SCORING LOGIC	
    // --------------------------------

    public void UpdateScore()
    {
        ++score;
        uiManager.ChangeScoreField(score, score > highScore);
    }

    public void UpdateTimer(float time)
    {
        uiManager.AssignGameOverText(false, "[center]" + time.ToString());
    }
}
