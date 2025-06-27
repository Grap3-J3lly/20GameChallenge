using Godot;
using Godot.Collections;
using System;

public partial class MenuManager : Control
{
    [Export]
    private Array<Control> menus = new Array<Control>();

    public static MenuManager Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
    }

    // Disables all menus except one provided
    public void OpenMenu(int menuIndex)
    {
        foreach (Control menu in menus)
        {
            menu.Visible = false;
        }
        menus[menuIndex].Visible = true;
    }
}
