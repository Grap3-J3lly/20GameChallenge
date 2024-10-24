using Godot;
using System;

public partial class GameManager : Node
{
	// --------------------------------
	//			VARIABLES	
	// --------------------------------

	[Export]
	private PlayerPaddle playerPaddle;
	[Export]
	private EnemyPaddle enemyPaddle;
    [Export]
	private Ball ball;
	[Export]
	private Goal playerGoal;
	[Export]
	private Goal enemyGoal;
	[Export]
	private int winScore = 5;
	[Export]
	private WinText winText;
	[Export]
	private float ballIncreaseSpeedAmount = 25f;
    [Export]
    private float enemyIncreaseSpeedAmount = 10f;

	private int playerScore = 0;
	private int enemyScore = 0;
	private bool gameOver = false;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static GameManager Instance { get; private set; }
    public Ball Ball { get => ball; }
	public int PlayerScore { get => playerScore; }
	public int EnemyScore { get => enemyScore; }
	public bool GameOver { get => gameOver; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		Instance = this;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		HandleGeneralInput();
    }

    // --------------------------------
    //		SCORING LOGIC	
    // --------------------------------

    public void UpdateScore(Goal goal, bool increment, int newVal = 0)
	{

		if (goal == playerGoal)
		{
			if (increment) ++enemyScore;
			else { enemyScore = newVal; }
			GD.Print("Enemy Score: " + enemyScore);
		}
		if(goal == enemyGoal)
		{
            if (increment) ++playerScore;
            else { playerScore = newVal; }
            GD.Print("Player Score: " + playerScore);
			enemyPaddle.PaddleSpeed += enemyIncreaseSpeedAmount;
		}
		DetermineGameOver();
		ball.BallSpeed += ballIncreaseSpeedAmount;
	}

	public void DetermineGameOver()
	{
		if(playerScore >= winScore)
		{
			winText.SetTextAndShow("[center]Player Wins!\nPress SPACE to Restart!");
			gameOver = true;
			return;
		}
		if (enemyScore >= winScore)
		{
            winText.SetTextAndShow("[center]Enemy Wins!\nPress SPACE to Restart!");
            gameOver = true; 
			return;
		}
	}

    // --------------------------------
    //		GENERAL LOGIC	
    // --------------------------------

    private void RestartGame()
    {
		UpdateScore(playerGoal, false);
		UpdateScore(enemyGoal, false);
		gameOver = false;
		enemyPaddle.ResetSpeed();
		ball.ResetSpeed();
		ball.Visible = true;
		winText.Visible = false;
    }

    public void HandleGeneralInput()
	{
		if(Input.IsActionJustPressed("ui_accept") && gameOver)
		{
			RestartGame();
		}
		if(Input.IsActionJustPressed("ui_cancel"))
		{

		}
	}

}
