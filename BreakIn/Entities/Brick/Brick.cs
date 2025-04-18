using Godot;
using Godot.Collections;
using System;

public partial class Brick : Area2D
{
	private GameManager gameManager;
	[Export]
	private Array<Color> LayerColors = new Array<Color>();

	private int rowID = 0;
	[Export]
	private int layerCount = 0;

    [Export]
    private Dictionary<Area2D, Vector2> DirectionalValues = new Dictionary<Area2D, Vector2>();

	// Currently defining direction for each side of object that can be collided with. Intent here is to remove the reverse direction logic,
	// which is being called twice when ball collides with two bricks at the same time.

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		Modulate = LayerColors[layerCount];
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
		Modulate = LayerColors[--layerCount];
	}
}
