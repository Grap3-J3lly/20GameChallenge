using Godot;
using System;

public partial class NextLevelButton : Button
{
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }


    public void OnPress()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);
        ++GameManager.Instance.CurrentDifficulty;
        BreakoutManager.Instance.Setup();
    }
}
