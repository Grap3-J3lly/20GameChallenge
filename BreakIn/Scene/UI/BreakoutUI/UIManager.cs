using Godot;
using Godot.Collections;
using System;

public partial class UIManager : CanvasLayer
{
    private BreakoutManager breakoutManager;

    [Export]
    private Array<Control> uiAreas = new Array<Control>();
    [Export]
    private HUDManager hudManager;
    [Export]
    private PopupManager popupManager;



    public static UIManager Instance { get; private set; }
    public PopupManager PopupManager { get => popupManager; }


    public override void _Ready()
    {
        Instance = this;
        breakoutManager = BreakoutManager.Instance;

        ToggleArea(1, false);
    }

    public override void _Process(double delta)
    {
        
    }

    public void ToggleArea(int areaIndex, bool isVisible)
    {
        uiAreas[areaIndex].Visible = isVisible;
    }

}
