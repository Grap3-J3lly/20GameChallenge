using Godot;
using Godot.Collections;
using System;
using System.Collections;

public partial class ObjectPool : Node
{
    // --------------------------------
    //		SPAWN FUNCTIONS	
    // --------------------------------

    public T[,] SpawnObjectsInGrid<T>(PackedScene objectScene, Vector2 initialSpawnPosition, Vector2I gridCount, Vector2 distancePerObject, Node parent) where T : Node2D
    {
        T[,] objectGrid = new T[gridCount.X, gridCount.Y];

        for (int y = 0; y < gridCount.Y; y++)
        {
            for (int x = 0; x < gridCount.X; x++)
            {
                T newObject = objectScene.Instantiate<T>();
                parent.AddChild(newObject);
                newObject.Position = initialSpawnPosition + (new Vector2(distancePerObject.X * x, distancePerObject.Y * y));
                objectGrid[x, y] = newObject;
            }
        }

        return objectGrid;
    }

    public T SpawnObjectAtPosition<T>(PackedScene objectScene, Vector2 spawnPosition, Node parent) where T : Node2D
    {
        T newObject = objectScene.Instantiate<T>();
        // Make Add Child and/or Position assignment part of Call Deferred
        newObject.Position = spawnPosition;
        parent.AddChild(newObject);
        return newObject;
    }    
}
