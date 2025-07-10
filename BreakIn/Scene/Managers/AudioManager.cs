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
