using Godot;
using Godot.Collections;
using System;

public partial class GameBoard : Node2D
{
    [Export]
    private Array<MeshInstance2D> wallMeshes = new Array<MeshInstance2D>();
    [Export]
    private Array<Texture2D> wallTextures = new Array<Texture2D>();


    public void AssignTextures(int textureIndex)
    {
        foreach (MeshInstance2D wall in wallMeshes)
        {
            wall.Texture = wallTextures[textureIndex];
        }
    }

}
