using Godot;
using Godot.Collections;
using System;

public partial class MenuManager : CanvasLayer
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    [Export]
    private Array<Control> menus = new Array<Control>();
    private Control previousMenu;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static MenuManager Instance { get; private set; }

    public Array<Control> Menus { get => menus; }
    public Control PreviousMenu { get => previousMenu; set => previousMenu = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
        previousMenu = menus[0];
    }

    // --------------------------------
    //		    MENU LOGIC	
    // --------------------------------

    // Disables all menus except one provided
    public void OpenMenu(int menuIndex, bool clearPreviousMenu = false)
    {
        CloseMenus(clearPreviousMenu);
        menus[menuIndex].Visible = true;
    }

    public void CloseMenus(bool clearPreviousMenu = false)
    {
        foreach (Control menu in menus)
        {
            if (menu.Visible)
            {
                previousMenu = menu;
            }
            menu.Visible = false;
        }

        if (clearPreviousMenu)
        {
            previousMenu = null;
        }
    }
}
