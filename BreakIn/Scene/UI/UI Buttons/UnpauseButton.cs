using Godot;
using System;

public partial class UnpauseButton : Button
{

    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }

    public void OnPress()
    {
        BreakoutManager.Instance.HandlePauseGame();
    }
}
