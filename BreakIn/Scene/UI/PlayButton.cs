using Godot;
using System;

public partial class PlayButton : Button
{
    [Export]
    private int gameSceneIndex = 1;
    [Export]
    private int gameDifficulty = 1;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += PressButton;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PressButton()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }
}
