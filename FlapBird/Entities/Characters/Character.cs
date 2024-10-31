using Godot;
using System;

public partial class Character : RigidBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

	private GameManager gameManager;

	[Export]
	private Vector2 spawnLocation;
	[Export]
	private float gravityScale = 1;
    [Export]
	private float jumpForce = 1.5f;
	[Export]
	private Sprite2D faceSprite;
	[Export]
	private CompressedTexture2D defaultFace;
	[Export]
	private CompressedTexture2D jumpFace;
	[Export]
	private CompressedTexture2D deathFace;
	[Export]
	private CompressedTexture2D goalFace;

	[Export]
	private float faceChangeDuration = 1f;
	[Export]
	private float resetDuration = 3f;
	private float timer = 0;

    // --------------------------------
    //			STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
        Position = spawnLocation;
        LinearVelocity = Vector2.Zero;
        GravityScale = gravityScale;
    }

	public override void _PhysicsProcess(double delta)
	{
        if (Input.IsActionJustPressed("ui_accept") && !gameManager.GameRunning)
        {
            Reset();
			return;
        }

        if (!gameManager.GameRunning) 
		{
			LinearVelocity = Vector2.Zero;
			GravityScale = 0;
			SetFace(deathFace);
			return; 
		}

		if (timer >= 0)
		{ 
			timer -= (float)delta;
            gameManager.UpdateTimer(Mathf.Ceil(timer));
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
			gameManager.Resetting = false;
            GravityScale = gravityScale;
        }
	}

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);

		if (gameManager.Resetting)
		{
			Transform2D transform = state.Transform;
			transform.Origin.X = spawnLocation.X;
			transform.Origin.Y = spawnLocation.Y;

			state.Transform = transform;
		}
	}

    // --------------------------------
    //			GENERAL LOGIC	
    // --------------------------------

    public void Reset()
	{
		gameManager.Resetting = true;
		gameManager.GameRunning = true;
		timer = resetDuration;
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

	public void SetGoalFace()
	{
		SetFace(goalFace);
		timer = faceChangeDuration;
    }
}
