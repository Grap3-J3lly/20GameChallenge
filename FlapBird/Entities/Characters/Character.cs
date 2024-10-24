using Godot;
using System;

public partial class Character : RigidBody2D
{
	[Export]
	private float jumpForce = 1.5f;
	[Export]
	private Sprite2D faceSprite;
	[Export]
	private CompressedTexture2D defaultFace;
	[Export]
	private CompressedTexture2D jumpFace;

	[Export]
	private float faceChangeDuration = 1f;
	private float timer = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (timer >= 0)
		{ 
			timer -= (float)delta;
		}

		if(Input.IsActionJustPressed("ui_accept"))
		{
			Movement();
			SetFace(jumpFace);
			timer = faceChangeDuration;
		}
		else if(timer <= 0)
		{
			SetFace(defaultFace);
		}
	}

	private void Movement()
	{
        if (LinearVelocity.Y > 0)
        {
            LinearVelocity = Vector2.Zero;
        }
        ApplyCentralImpulse(Vector2.Up * jumpForce);

    }

	private void SetFace(CompressedTexture2D face)
	{
		faceSprite.Texture = face;
	}
}
