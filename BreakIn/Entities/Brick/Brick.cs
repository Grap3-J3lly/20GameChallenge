using Godot;
using Godot.Collections;
using System;

public partial class Brick : StaticBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;
	[Export]
	private Array<Color> layerColors = new Array<Color>();

	private int rowID = 0;
	[Export]
	private int layerCount = 0;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		Modulate = layerColors[layerCount];
	}

	public void ProcessHit()
	{
		GD.Print($"Brick.cs: Layer Count - {layerCount}");
		if(layerCount == 0)
		{
			gameManager.PlayerScore++;
			GD.Print($"Brick.cs: Score - {gameManager.PlayerScore} ");
			QueueFree();
			return;
		}
		Modulate = layerColors[--layerCount];
	}
}
