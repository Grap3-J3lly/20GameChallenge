using Godot;
using System;

public partial class BackButton : Button
{

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
        Control previousMenu = menuManager.PreviousMenu;
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);

        if (previousMenu != null)
        {
            int indexOfPreviousMenu = menuManager.Menus.IndexOf(previousMenu);
            menuManager.OpenMenu(indexOfPreviousMenu);
        }
        else
        {
            menuManager.CloseMenus();
        }
    }
}
