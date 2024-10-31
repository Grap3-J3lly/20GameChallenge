using Godot;
using System;

public partial class UIManager : Node
{
	[Export]
	private RichTextLabel scoreNumber;
    [Export]
    private RichTextLabel highScoreNumber;
	[Export]
	private RichTextLabel gameOverText;
	[Export]
	private string gameOverMessage = "[center]Game Over!\r\nPress SPACE to Restart";


    public static UIManager Instance { get; private set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Instance = this;
    }

	
	public override void _Process(double delta)
	{
	}

	public void ChangeScoreField(int score, bool isHighScore = false)
	{
		RichTextLabel textField;
		if (isHighScore)
		{
			textField = highScoreNumber;
			textField.Text = score.ToString();
		}

        textField = scoreNumber;
        textField.Text = score.ToString();
	}

	public void DisplayGameOver(bool isGameRunning)
	{
		gameOverText.Visible = !isGameRunning;
	}

	public void AssignGameOverText(bool isDefault = true, string message = "")
	{
		if(isDefault)
		{
			gameOverText.Text = gameOverMessage;
			return;
		}
		gameOverText.Text = message;
	}
}
