using Godot;
using System;

public partial class PlayButton : Button
{
    [Export]
    private int gameSceneIndex = 1;
    [Export]
    private int gameDifficulty = 1;

    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }


    public void OnPress()
    {
        GameManager.Instance.CurrentDifficulty = gameDifficulty;
        SceneManager.LoadScene(gameSceneIndex);
    }
}
