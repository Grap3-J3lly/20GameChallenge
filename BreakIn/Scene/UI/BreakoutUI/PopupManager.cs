using Godot;
using System;

public partial class PopupManager : Control
{
    [Export]
    private UIManager uiManager;

    private int highScoreVal = -1;

    [Export]
    private RichTextLabel titleLabel;
    [Export]
    private RichTextLabel currentScoreLabel;
    [Export]
    private RichTextLabel highScoreLabel;

    public override void _Ready()
    {
        base._Ready();
        // Where high score val is set by breakoutManager
    }

    public void ChangeTitle(string newTitle)
    {
        titleLabel.Text = newTitle;
    }

    public void ChangeScore(int score)
    {
        currentScoreLabel.Text = score.ToString();
        if(score > highScoreVal)
        {
            highScoreLabel.Text = score.ToString();
        }
    }
}
