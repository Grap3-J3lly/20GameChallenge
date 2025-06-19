using Godot;
using Godot.Collections;
using System;

public partial class Brick : StaticBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private GameManager gameManager;
	private ObjectPool objectPool;
	[Export]
	private Array<Color> layerColors = new Array<Color>();

	private int rowID = 0;
	[Export]
	private int layerCount = 0;

    // --------------------------------
    //			PROPERTIES		
    // --------------------------------
	
	public int RowID { get => rowID; set => rowID = value; }
	public int LayerCount { get => layerCount; set => layerCount = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		objectPool = gameManager.ObjectPool;
		Modulate = layerColors[layerCount];
	}

	public void ProcessHit(bool superBall)
	{
		GD.Print($"Brick.cs: Layer Count - {layerCount}");
		if(layerCount == 0 || superBall)
		{
			DestroyBrick();
			return;
		}
		Modulate = layerColors[--layerCount];
	}

	private void DestroyBrick()
	{
        gameManager.TriggerObjectiveSuccess();
        GD.Print($"Brick.cs: Score - {gameManager.PlayerScore} ");
        objectPool.Bricks.Remove(this);
        CheckIfBrickRowEmpty();

		objectPool.SpawnPowerupOrb(Position);

        QueueFree();
    }

	private void CheckIfBrickRowEmpty()
	{
		int bricksInRow = objectPool.GetBrickCountByRowID(rowID);
		// GD.Print($"Brick.cs: Bricks Remaining in Row {rowID}: {bricksInRow}");
		if (bricksInRow != 0)
		{
			return;
		}

		gameManager.EmitSignal(GameManager.SignalName.RowClear);
	}
}
