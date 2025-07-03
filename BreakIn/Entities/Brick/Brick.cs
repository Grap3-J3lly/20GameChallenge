using Godot;
using Godot.Collections;
using System;

public partial class Brick : StaticBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    private BreakoutManager breakoutManager;
	private ObjectPool objectPool;
	[Export]
	private Array<Color> layerColors = new Array<Color>();

	private int rowID = 0;
	[Export]
	private int layerCount = 0;

	private Vector2I gridPosition = new Vector2I();

    // --------------------------------
    //			PROPERTIES		
    // --------------------------------
	
	public int RowID { get => rowID; set => rowID = value; }
	public int LayerCount { get => layerCount; set => layerCount = value; }
	public Vector2I GridPosition { get => gridPosition; set => gridPosition = value; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
		breakoutManager = BreakoutManager.Instance;
		objectPool = breakoutManager.ObjectPool;
		// GD.Print($"Brick.cs: Assigned Layer Count in Ready: {layerCount}");
	}

    // --------------------------------
    //		SINGLE BRICK LOGIC	
    // --------------------------------

	public void ChangeBrickLayer(int layerIndex)
	{
		layerCount = layerIndex;
        Modulate = layerColors[layerIndex];
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
        breakoutManager.TriggerObjectiveSuccess();
        GD.Print($"Brick.cs: Score - {breakoutManager.PlayerScore} ");
		breakoutManager.Bricks[gridPosition.X, gridPosition.Y] = null;
        CheckIfBrickRowEmpty();

		breakoutManager.SpawnPowerUpOrb(Position);

        QueueFree();
    }

    // --------------------------------
    //			ROW LOGIC	
    // --------------------------------

    private void CheckIfBrickRowEmpty()
	{
		int bricksInRow = GetBrickCountByRowID(rowID);
		GD.Print($"Brick.cs: Bricks Remaining in Row {rowID}: {bricksInRow}");
		if (bricksInRow != 0)
		{
			return;
		}

		breakoutManager.EmitSignal(BreakoutManager.SignalName.RowClear);
	}

    public int GetBrickCountByRowID(int rowID)
    {
        int count = 0;

        foreach (Brick brick in breakoutManager.Bricks)
        {
			// GD.Print($"Brick.cs: Brick Row ID: {brick.RowID}"); // <-- To use, add check for brick being null
            if (brick != null && brick.RowID == rowID) { count++; }
        }

        return count;
    }
}
