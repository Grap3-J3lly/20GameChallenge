using Godot;
using System;
using static Godot.WebSocketPeer;

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
	private float faceChangeTimer = 0;
	private float resetTimer = 0;

    private bool inDeathState = false;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public bool InDeathState { get => inDeathState; set => inDeathState = value; }

    // --------------------------------
    //			STANDARD LOGIC	
    // --------------------------------

    public override void _Ready()
	{
		gameManager = GameManager.Instance;
		Setup();
    }

	public override void _PhysicsProcess(double delta)
	{
        if(gameManager.GameOver && !gameManager.Resetting && Input.IsActionJustPressed("ui_accept"))
        {
            gameManager.Reset();
            return;
        }

        if(gameManager.Resetting)
        {
            ResetValues();
        }

		// resetTimer = TimerDecrementer(delta, resetTimer, true);
		faceChangeTimer = TimerDecrementer(delta, faceChangeTimer);

		HandleCharacterMovementProcess();
	}

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);
		ResetPosition(state);
	}

    // --------------------------------
    //			GENERAL LOGIC	
    // --------------------------------

	private void Setup()
	{
        Position = spawnLocation;
        LinearVelocity = Vector2.Zero;
        GravityScale = gravityScale;
    }

    public void EnterDeathState()
    {
        LinearVelocity = Vector2.Zero;
        GravityScale = 0;
        SetFace(deathFace);
        inDeathState = true;
    }

	// Handles logic for allowing movement and visual face changes during movement while game is active
	private void HandleCharacterMovementProcess()
	{
        if (Input.IsActionJustPressed("ui_accept"))
        {
            Movement();
            SetFace(jumpFace);
            faceChangeTimer = faceChangeDuration;
        }
        else if (faceChangeTimer < 0)
        {
            SetFace(defaultFace);
        }
    }

    // --------------------------------
    //			TRANSFORM LOGIC	
    // --------------------------------

    public void ResetValues()
	{
		// gameManager.Resetting = true;
		resetTimer = resetDuration;

        // Testing:
        GravityScale = gravityScale;
        gameManager.Resetting = false;
		gameManager.GameOver = false;
    }

	private void ResetPosition(PhysicsDirectBodyState2D state)
	{
        if (inDeathState && gameManager.Resetting)
        {
            GD.Print("Resetting Position");

            // state.Transform = new Transform2D(0.0f, spawnLocation);

            Transform2D transform = state.Transform;
            transform.Origin.X = spawnLocation.X;
            transform.Origin.Y = spawnLocation.Y;

            state.Transform = transform;
            inDeathState = false;
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

    // --------------------------------
    //			VISUAL LOGIC	
    // --------------------------------

	private float TimerDecrementer(double delta, float timer, bool updateUI = false)
	{
		if(timer >= 0)
		{
			timer -= (float)delta;
			if(updateUI)
			{
				gameManager.UpdateTimerUI(Mathf.Ceil(timer));
			}
		}
		return timer;
    }

    private void SetFace(CompressedTexture2D face)
	{
		faceSprite.Texture = face;
	}

	public void SetGoalFace()
	{
		SetFace(goalFace);
		faceChangeTimer = faceChangeDuration;
    }
}
