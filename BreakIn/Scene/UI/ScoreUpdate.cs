using Godot;
using System;

public partial class ScoreUpdate : RichTextLabel
{
	private GameManager gameManager;
	[Export]
	private string scoreLabelText = "Score: ";

	public override void _Ready()
	{
		gameManager = GameManager.Instance;
	}

	
	public override void _Process(double delta)
	{
		Text = scoreLabelText + gameManager.PlayerScore.ToString();
	}
}
