using Godot;
using Godot.Collections;
using System;

public partial class ObjectPool : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    [Export]
    private Node brickParent;
    
    [Export]
    private int maxRowCount_Easy = 2;
    [Export]
    private int maxRowCount_Medium = 3;
    [Export]
    private int maxRowCount_Hard = 5;
    [Export]
    private int maxBrickPerRow = 11;

    [Export]
    private Vector2 initialSpawnPosition = new Vector2(66, 138);
    [Export] Vector2 distancePerBrick = new Vector2(102, 23);


    private Array<Brick> bricks = new Array<Brick>();

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public Array<Brick> Bricks {  get { return bricks; } }

    // --------------------------------
    //		GENERAL FUNCTIONS	
    // --------------------------------

    public void SpawnBricks(PackedScene brickScn, int difficulty)
    {
        int rowCount = -1;
        switch(difficulty)
        {
            case 1: rowCount = maxRowCount_Easy;
                break;
            case 2: rowCount = maxRowCount_Medium;
                break;
            case 3: rowCount = maxRowCount_Hard;
                break;
            default: GD.PrintErr($"ObjectPool.cs: Invalid Difficulty Value Provided");
                break;
        }

        for (int y = 0; y < rowCount; y++)
        {
            for(int x = 0; x < maxBrickPerRow; x++)
            {
                Brick newBrick = brickScn.Instantiate<Brick>();
                brickParent.AddChild(newBrick);
                newBrick.RowID = y;
                newBrick.Position = initialSpawnPosition + (new Vector2(distancePerBrick.X * x, distancePerBrick.Y * y));
            }
        }

    }

    public int GetBrickCountByRowID(int rowID)
    {
        int count = 0;
        foreach(Brick brick in bricks)
        {
            if(brick.RowID == rowID) { count++; }
        }

        return count;
    }

    
}
