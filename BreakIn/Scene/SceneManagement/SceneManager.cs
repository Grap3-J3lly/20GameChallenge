using Godot;
using Godot.Collections;
using System;

public partial class SceneManager : Node
{
    [Export]
    private Array<string> scenePaths;


    public override void _Ready()
    {
        base._Ready();
        // Load First PackedScene in Project:
        LoadScene(scenePaths[0]);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // ResourceLoader.LoadThreadedGetStatus(path, )
    }

    private void LoadScene(string path)
    {
        ResourceLoader.LoadThreadedRequest(path);
    }

}
