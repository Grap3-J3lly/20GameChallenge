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
    }

    public override void _Process(double delta)
    {
        UpdateLabel(textFields[HUDTextField.LivesRemaining], livesLabelText, breakoutManager.PlayerLives);
        UpdateLabel(textFields[HUDTextField.Score], scoreLabelText, breakoutManager.PlayerScore);

        ShiftLabelAlpha(textFields[HUDTextField.Misc], effectDuration);
    }

    // --------------------------------
    //		    LABEL LOGIC	
    // --------------------------------

    private void UpdateLabel(RichTextLabel label, string preValueText, int value)
    {
        label.Text = preValueText + value.ToString();
    }

    public void UpdateLabel(HUDTextField textFieldType, string newText, bool triggerPowerupEffect = false)
    {
        textFields[textFieldType].Text = newText;
        triggerEffect = triggerPowerupEffect;
    }

    private void ShiftLabelAlpha(RichTextLabel label, float duration)
    {
        // if (!triggerEffect) return;
        Color currentColor = label.Modulate;
        float alphaVal = (float)(Math.Abs(Mathf.Sin(Time.GetUnixTimeFromSystem())));
        label.Modulate = new Color(currentColor.R, currentColor.G, currentColor.B, alphaVal);

        // Need to find a means of capturing when the effect is close to 0 so you can increment a counter to know when to disable effect
        // Also, fix your sound bs

        GD.Print($"HUDManager.cs: Current Modulate on {label.Name}: {label.Modulate}");
    }
}
