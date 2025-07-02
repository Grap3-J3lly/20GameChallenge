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
        MenuManager.Instance.CloseMenus(true);
        GameManager.Instance.CurrentDifficulty = gameDifficulty;
        SceneManager.LoadScene(gameSceneIndex, false);
    }
}
