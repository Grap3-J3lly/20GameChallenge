using Godot;
using System;

public partial class HomeButton : Button
{
    [Export]
    private int sceneIndex = 0;

    public override void _Ready()
    {
        base._Ready();
        Pressed += OnPress;
    }


    public void OnPress()
    {        
        AudioManager.Instance.PlaySFX(AudioManager.SFXType.UI_Interact);
        SceneManager.LoadScene(sceneIndex);
    }
}
