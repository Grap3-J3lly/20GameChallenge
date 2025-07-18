using Godot;
using Godot.Collections;
using System;

public partial class PopupManager : Control
{
    [Export]
    private UIManager uiManager;

    private int highScoreVal = -1;

    public enum PopupType
    {
        GamePause,
        GameLose,
        GameWin
    }
    private PopupType currentPopupType;

    [Export]
    private RichTextLabel titleLabel;
    [Export]
    private RichTextLabel currentScoreLabel;
    [Export]
    private RichTextLabel highScoreLabel;

    [Export]
    private Array<Button> swappableButtons = new Array<Button>();

    public override void _Ready()
    {
        base._Ready();
    }

    public void OpenPopup(PopupType popupType, bool isFinalLevel = false)
    {
        highScoreVal = BreakoutManager.Instance.HighScore;
        SetScore(BreakoutManager.Instance.PlayerScore);
        switch(popupType)
        {
            case PopupType.GamePause:
                SetTitle("Game Paused");
                SetActiveButton(0);
                break;
            case PopupType.GameLose:
                SetTitle("Game Over!");
                SetActiveButton(1);
                break;
            case PopupType.GameWin:
                SetTitle("You Won!");
                if(isFinalLevel) SetActiveButton(1);
                else SetActiveButton(2);
                break;
        }
    }

    public void SetTitle(string newTitle)
    {
        titleLabel.Text = newTitle;
    }

    public void SetScore(int score)
    {
        currentScoreLabel.Text = score.ToString();
        if (score > highScoreVal)
        {
            highScoreVal = score;
        }
        highScoreLabel.Text = highScoreVal.ToString();
    }

    /// <summary>
    /// Enable button at given index, disable all other buttons
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void SetActiveButton(int buttonIndex)
    {
        foreach(Button button in swappableButtons)
        {
            button.Visible = false;
        }

        swappableButtons[buttonIndex].Visible = true;
    }
}
