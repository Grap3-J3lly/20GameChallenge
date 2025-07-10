using Godot;
using System;

// Button used to navigate around various Controls, typically used in a menu system
public partial class ControlButton : Button
{
    [Export]
    private int controlIndex = -1;
    [Export]
    private bool clearPreviousMenu = false;

    private MenuManager menuManager;

    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
        CallDeferred("DelayedSetup");
    }

    private void DelayedSetup()
    {
        menuManager = MenuManager.Instance;
    }

    public void OnPress()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);
        menuManager.OpenMenu(controlIndex, clearPreviousMenu);
    }
}
