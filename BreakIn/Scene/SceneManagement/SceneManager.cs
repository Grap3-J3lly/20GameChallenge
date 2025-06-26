using Godot;
using Godot.Collections;
using System;

public struct PackedSceneLoad
{
    private string path;
    private Godot.Collections.Array loadProgress = new Godot.Collections.Array();
    public PackedScene LoadedScene { get; private set; }
    public float Progress { get; private set; }

    public PackedSceneLoad(string scenePath, out Godot.Error error)
    {
        path = scenePath;
        error = ResourceLoader.LoadThreadedRequest(scenePath);
    }

    public ResourceLoader.ThreadLoadStatus Tick()
    {
        ResourceLoader.ThreadLoadStatus status;
        status = ResourceLoader.LoadThreadedGetStatus(path, loadProgress);

        switch(status)
        {
            case ResourceLoader.ThreadLoadStatus.Loaded:
                Progress = 1;
                LoadedScene = ResourceLoader.LoadThreadedGet(path) as PackedScene;
                break;
            case ResourceLoader.ThreadLoadStatus.InProgress:
                Progress = (float)loadProgress[0];
                break;
            case ResourceLoader.ThreadLoadStatus.Failed:
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                Progress = 1;

                break;
        }

        return status;
    }
}

public partial class SceneManager : Node
{
    [Export]
    private Array<string> scenePaths;
    private Node currentScene;

    public static SceneManager Instance { get; private set; }

    public override void _Ready()
    {
        base._Ready();

        Instance = this;
        LoadScene(scenePaths[0]);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // Eventually will need to add to this to account for Async Scene Loading
    }

    public static void LoadScene(int sceneIndex)
    {
        LoadScene(Instance.scenePaths[sceneIndex]);
    }

    private static void LoadScene(string path)
    {
        GD.Print($"SceneManager.cs: Loading Scene: {path}"); // <- Eventually replace with SceneSettings to make this easier to read
        PackedSceneLoad packedSceneLoad = new PackedSceneLoad(path, out Godot.Error error);

        if(error != Error.Ok)
        {
            GD.PrintErr($"SceneManager.cs: Error Loading Scene at Path: {path}");
            return;
        }

        ResourceLoader.ThreadLoadStatus status = ResourceLoader.ThreadLoadStatus.InProgress;
        while(status == ResourceLoader.ThreadLoadStatus.InProgress)
        {
            // GD.Print($"SceneManager.cs: Scene Load Progress: {packedSceneLoad.Progress}");
            status = packedSceneLoad.Tick();
        }

        if(status == ResourceLoader.ThreadLoadStatus.Loaded)
        {
            // Unload Scene once new Scene is loaded
            if(Instance.currentScene != null)
            {
                GD.Print($"SceneManager.cs: Unloading Previous Scene: {Instance.currentScene.Name}");
                Instance.currentScene.QueueFree();
            }

            // Load New Scene
            Instance.currentScene = packedSceneLoad.LoadedScene.Instantiate();
            Instance.AddChild(Instance.currentScene);
            GD.Print($"SceneManager.cs: Scene Successfully Loaded: {Instance.currentScene.Name}");
        }
    }

}
