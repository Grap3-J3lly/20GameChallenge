using Godot;
using System;

public partial class SkyBackground : TextureRect
{
    [Export]
    private double skySpeedMult = .1f;

    public override void _Process(double delta)
    {
        base._Process(delta);
        NoiseTexture2D noiseTexture = (NoiseTexture2D)Texture;
        FastNoiseLite noise = (FastNoiseLite)noiseTexture.Noise;
        
        Vector3 offset = noise.Offset;
        offset.X += (float)(delta * skySpeedMult);
        offset.Y += (float)(delta * skySpeedMult);
        offset.Z += (float)(delta * skySpeedMult);

        // GD.Print($"SkyBackground: {offset}");

        noise.Offset = offset;
    }
}
