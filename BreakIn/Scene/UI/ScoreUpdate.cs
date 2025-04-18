using Godot;
using System;

public partial class ScoreUpdate : RichTextLabel
{
	[Export]
	private bool isPlayerScore = true;
	private GameManager gameManager;

	public override void _Ready()
	{
		gameManager = GameManager.Instance;
	}

	
	public override void _Process(double delta)
	{
		// Text = isPlayerScore? gameManager.PlayerScore.ToString() : gameManager.EnemyScore.ToString();
	}
}
