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
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);
        BreakoutManager.Instance.Setup();
    }
}
