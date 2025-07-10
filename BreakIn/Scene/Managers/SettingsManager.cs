using Godot;
using System;

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

        Setup();
    }

    private void Setup()
    {
        sfxSlider.SetValueNoSignal(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(sfxAudioBusIndex)));
        sfxSlider.ValueChanged += OnSliderChange_SFX;
    }

    public void OnSliderChange_SFX(double value)
    {
        AudioServer.SetBusVolumeDb(sfxAudioBusIndex, (float)Mathf.LinearToDb(value));
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);
    }
}
