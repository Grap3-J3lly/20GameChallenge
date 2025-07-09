using Godot;
using Godot.Collections;
using System;

public partial class AudioManager : Node
{
    [Export]
    private AudioStreamPlayer audioPlayer;
    public enum SFXType
    {
        None,
        OnHit,
        OnDestroy,
        UI_Interact
    }

    [Export]
    public Dictionary<SFXType, AudioStreamOggVorbis> sfxLibrary = new Dictionary<SFXType, AudioStreamOggVorbis>();

    public static AudioManager Instance { get; private set; }
    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }

    public void PlaySFX(SFXType type)
    {
        GD.Print($"AudioManager.cs: Playing SFX: {type}");
        
        if(audioPlayer != null )//&& !audioPlayer.Playing)
        {
            audioPlayer.Stream = sfxLibrary[type];

            audioPlayer.Play();
        }
    }
}
