using Godot;
using Godot.Collections;
using System;

public partial class HUDManager : Control
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private BreakoutManager breakoutManager;

    public enum HUDTextField
    {
        LivesRemaining,
        Score,
        Misc
    }

    [Export]
    private Dictionary<HUDTextField, RichTextLabel> textFields = new Dictionary<HUDTextField, RichTextLabel>();

    [Export]
    private string livesLabelText = "Lives Remaining: ";

    [Export]
    private string scoreLabelText = "Score: ";
    private bool triggerEffect = false;
    [Export]
    private float effectDuration = 3.0f;
    private float effectTimer;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public Dictionary<HUDTextField, RichTextLabel> TextFields { get => textFields; }

    // --------------------------------
    //		    STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
    {
        breakoutManager = BreakoutManager.Instance;
        // textFields[HUDTextField.Misc].Visible = false;
        effectTimer = effectDuration;

    }

    public override void _Process(double delta)
    {
        UpdateLabel(textFields[HUDTextField.LivesRemaining], livesLabelText, breakoutManager.PlayerLives);
        UpdateLabel(textFields[HUDTextField.Score], scoreLabelText, breakoutManager.PlayerScore);

        if(triggerEffect) ShiftLabelAlpha(textFields[HUDTextField.Misc], (float)delta);
    }

    // --------------------------------
    //		    LABEL LOGIC	
    // --------------------------------

    private void UpdateLabel(RichTextLabel label, string preValueText, int value)
    {
        label.Text = preValueText + value.ToString();
    }

    public void UpdateLabel(HUDTextField textFieldType, string newText)
    {
        textFields[textFieldType].Text = newText;
        triggerEffect = true;
        effectTimer = effectDuration;
    }

    private void ShiftLabelAlpha(RichTextLabel label, float deltaTime)
    {
        if (effectTimer <= 0)
        {
            triggerEffect = false;
            return;
        }
        // if (!triggerEffect) return;
        Color currentColor = label.Modulate;
        effectTimer -= deltaTime;
        float alphaVal = (float)(Math.Abs(Mathf.Sin(effectTimer)));

        label.Modulate = new Color(currentColor.R, currentColor.G, currentColor.B, alphaVal);

        GD.Print($"HUDManager.cs: Current alpha value: {alphaVal}");
    }
}
