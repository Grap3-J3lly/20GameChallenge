using Godot;
using System;

public partial class RetryButton : Button
{
    
    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }


    public void OnPress()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);
        BreakoutManager.Instance.Setup();
    }
}
