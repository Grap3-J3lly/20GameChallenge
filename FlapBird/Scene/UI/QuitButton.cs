using Godot;
using System;

public partial class QuitButton : Button
{
	public override void _Ready()
	{
		Pressed += PressButton;
	}

	public override void _Process(double delta)
	{
	}

	public void PressButton()
	{
		GetTree().Quit();
	}
}
