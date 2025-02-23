using Godot;
using System;

public partial class Island : Node2D
{
	[Export]
	private float islandSpeed = 1.0f;

	[Export]
	private float finishLine;
	[Export]
	private float respawnLocation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.Instance != null && !GameManager.Instance.GameOver)
		{
			Position = new Vector2(Position.X - islandSpeed, Position.Y);
		}
		if(Position.X <= finishLine)
		{
			Position = new Vector2(respawnLocation, Position.Y);
			GD.Print("Teleporting Island: " + Name + " to: " + Position);
		}
	}
}
