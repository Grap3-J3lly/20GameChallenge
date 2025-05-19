using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class UIManager : CanvasLayer
{
    private GameManager gameManager;

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
        gameManager = GameManager.Instance;
    }

    public override void _Process(double delta)
    {
        UpdateLabel(livesField, livesLabelText, gameManager.PlayerLives);
        UpdateLabel(scoreField, scoreLabelText, gameManager.PlayerScore);
    }

    private void UpdateLabel(RichTextLabel label, string preValueText, int value)
    {
        label.Text = preValueText + value.ToString();
    }

}
