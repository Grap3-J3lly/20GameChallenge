using Godot;
using System;

public partial class GameManager : Node
{
    private int currentDifficulty = -1;

    public static GameManager Instance { get; private set; }
    public int CurrentDifficulty { get => currentDifficulty; set => currentDifficulty = value; }

    public override void _Ready()
    {
        base._Ready();
        Instance = this;
    }
}
