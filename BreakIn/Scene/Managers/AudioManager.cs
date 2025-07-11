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
        UI_Interact
    }

    [Export]
    public Dictionary<SFXType, AudioStreamOggVorbis> sfxLibrary = new Dictionary<SFXType, AudioStreamOggVorbis>();

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
        Godot.Collections.Array data = SaveManager.Instance.LoadFromFile();
        for (int i = 0; i < data.Count - 1; i++)
        {
            if (data[i].ToString().Contains("sfxVolume"))
            {
                GD.Print($"AudioManager.cs: Assigning Initial SFX Volume");
                AudioServer.SetBusVolumeDb(1, (float)Mathf.LinearToDb((float)data[i + 1]));
                return;
            }
        }
    }

    // --------------------------------
    //		    AUDIO LOGIC	
    // --------------------------------

    public void PlaySFX(SFXType type)
    {
        GD.Print($"AudioManager.cs: Playing SFX: {type}");
        
        if(sfxAudioPlayer != null )//&& !audioPlayer.Playing)
        {
            sfxAudioPlayer.Stream = sfxLibrary[type];

            sfxAudioPlayer.Play();
        }
    }
}
