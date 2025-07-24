using Godot;
using Godot.Collections;
using System;

public partial class AudioManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    [Export]
    private AudioStreamPlayer sfxAudioPlayer;

    public enum SFXType
    {
        None,
        OnHit,
        OnDestroy,
        UI_Interact,
        TriggerDefaultPowerup
    }

    [Export]
    public Dictionary<SFXType, AudioStreamOggVorbis> sfxLibrary = new Dictionary<SFXType, AudioStreamOggVorbis>();

    private const int sfxAudioBusIndex = 1;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public static AudioManager Instance { get; private set; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        base._Ready();
        Instance = this;

        CallDeferred("Setup");
    }

    // --------------------------------
    //		    SETUP LOGIC	
    // --------------------------------

    private void Setup()
    {
        AssignDefaultVolumeLevel();
    }

    private void AssignDefaultVolumeLevel()
    {
        double sfxValue = (double)SaveSystem.GetDataItem("Settings", "sfxVolume", defaultValue: 0.0f);
        AudioServer.SetBusVolumeDb(sfxAudioBusIndex, (float)Mathf.LinearToDb(sfxValue));
    }

    // --------------------------------
    //		    AUDIO LOGIC	
    // --------------------------------

    public void PlaySFX_Global(SFXType type)
    {
        GD.Print($"AudioManager.cs: Playing SFX: {type}");
        
        if(sfxAudioPlayer != null )//&& !audioPlayer.Playing)
        {
            sfxAudioPlayer.Stream = sfxLibrary[type];

            sfxAudioPlayer.Play();
        }
    }

    public void PlaySFX_2D(AudioStreamPlayer2D streamPlayer, SFXType type)
    {
        GD.Print($"AudioManager.cs: Playing SFX: {type} on StreamPlayer: {streamPlayer}");

        if (streamPlayer != null)
        {            
            streamPlayer.Stream = sfxLibrary[type];

            streamPlayer.Play();
        }
    }
}
