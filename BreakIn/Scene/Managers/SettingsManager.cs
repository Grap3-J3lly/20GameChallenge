using Godot;
using Godot.Collections;

public partial class SettingsManager : Control
{
    [Export]
    private HSlider sfxSlider;
    [Export]
    private int sfxAudioBusIndex = 1;

    public static SettingsManager Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        CallDeferred("Setup");
    }

    private void Setup()
    {
        sfxSlider.ValueChanged += OnSliderChange_SFX;
        InitialSliderValueAssignment();
    }

    private void InitialSliderValueAssignment()
    {
        double sfxValue = (double)SaveSystem.GetDataItem("Settings", "sfxVolume", defaultValue: 0.0f);
        sfxSlider.SetValueNoSignal(sfxValue);
    }

    public void OnSliderChange_SFX(double value)
    {
        GD.Print($"SettingsManager.cs: Changing Volume to: {value}");

        float volume = (float)Mathf.LinearToDb(value);
        AudioServer.SetBusVolumeDb(sfxAudioBusIndex, volume);
        AudioManager.Instance.PlaySFX_Global(AudioManager.SFXType.UI_Interact);

        SaveSystem.AddDataItem("Settings", "sfxVolume", value);
        SaveSystem.SaveData("Settings");
    }
}
