using Godot;
using Godot.Collections;
using System;

public partial class Wall : Area2D
{
	[Export]
	private float speed = 100f;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Movement(delta);
		HandleCollision();
	}

	private void Movement(double delta)
	{
		Vector2 newPos = Position;
		newPos.X -= speed * (float)delta;
		Position = newPos;
	}

	private void HandleCollision()
	{
		Array<Node2D> overlappingAreas = GetOverlappingBodies();
		foreach(Node2D area in overlappingAreas)
		{
			GD.Print("Owner: " + area.GetClass());
		}

	}
}
