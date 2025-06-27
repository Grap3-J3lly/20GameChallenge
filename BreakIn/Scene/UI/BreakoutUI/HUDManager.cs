using Godot;
using System;

public partial class HUDManager : Control
{
    private BreakoutManager breakoutManager;

    [Export]
    private string livesLabelText = "Lives Remaining: ";
    [Export]
    private RichTextLabel livesField;

    [Export]
    private string scoreLabelText = "Score: ";
    [Export]
    private RichTextLabel scoreField;

    public override void _Ready()
    {
        breakoutManager = BreakoutManager.Instance;
    }

    public override void _Process(double delta)
    {
        UpdateLabel(livesField, livesLabelText, breakoutManager.PlayerLives);
        UpdateLabel(scoreField, scoreLabelText, breakoutManager.PlayerScore);
    }

    private void UpdateLabel(RichTextLabel label, string preValueText, int value)
    {
        label.Text = preValueText + value.ToString();
    }
}
