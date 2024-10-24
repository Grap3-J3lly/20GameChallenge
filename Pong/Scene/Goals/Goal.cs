using Godot;
using System;

public partial class Goal : Area2D
{
	[Export]
	private bool isPlayerGoal;

	public bool IsPlayerGoal { get => isPlayerGoal; }
}
