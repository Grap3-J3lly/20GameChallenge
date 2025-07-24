using Godot;
using Godot.Collections;
using System;

public partial class UIManager : CanvasLayer
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private BreakoutManager breakoutManager;

    [Export]
    private Array<Control> uiAreas = new Array<Control>();
    [Export]
    private HUDManager hudManager;
    [Export]
    private PopupManager popupManager;

    private const int popupAreaIndex = 1;
    private const bool popupDefaultVisibility = false;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static UIManager Instance { get; private set; }
    public HUDManager HudManager { get => hudManager; }
    public PopupManager PopupManager { get => popupManager; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------
    public override void _Ready()
    {
        Instance = this;
        breakoutManager = BreakoutManager.Instance;

        ToggleArea(popupAreaIndex, popupDefaultVisibility);
    }

    // --------------------------------
    //			GENERAL LOGIC	
    // --------------------------------

    /// <summary>
    /// AreaIndex: Area to toggle, 0 - HUDManager, 1 - PopupManager
    /// IsVisible: Make AreaIndex Visible or not
    /// </summary>
    /// <param name="areaIndex"></param>
    /// <param name="isVisible"></param>
    public void ToggleArea(int areaIndex, bool isVisible)
    {
        uiAreas[areaIndex].Visible = isVisible;
    }

}
