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
        // Setup();
    }

    private void Setup()
    {
        sfxSlider.ValueChanged += OnSliderChange_SFX;

        Array data = SaveManager.Instance.LoadFromFile();
        for (int i = 0; i < data.Count - 1; i++)
        {
            if (data[i].ToString().Contains("sfxVolume"))
            {
                sfxSlider.SetValueNoSignal((double)data[i + 1]);
                return;
            }
        }

        sfxSlider.SetValueNoSignal(Mathf.DbToLinear(AudioServer.GetBusVolumeDb(sfxAudioBusIndex)));
    }

    public void OnSliderChange_SFX(double value)
    {
        float volume = (float)Mathf.LinearToDb(value);
        GD.Print($"SettingsManager.cs: Changing Volume to: {volume}");

        AudioServer.SetBusVolumeDb(sfxAudioBusIndex, volume);
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);

        SaveManager.Instance.PopulateDataToSave<string>("sfxVolume", false);
        SaveManager.Instance.PopulateDataToSave<string>(value, true);

        SaveManager.Instance.SaveToFile();
    }
}
