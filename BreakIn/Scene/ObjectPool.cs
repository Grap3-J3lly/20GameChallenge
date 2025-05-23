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
    private int maxRowCount = 2;
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

    public void SpawnBricks(PackedScene brickScn)
    {
        Brick newBrick = brickScn.Instantiate<Brick>();
        brickParent.AddChild(newBrick);
        // Need to spawn bricks by count and row (consider using maxBricks per row, brickCount, and rowId)
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
