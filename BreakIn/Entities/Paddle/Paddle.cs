using Godot;
using Godot.Collections;
using System;

public partial class Paddle : AnimatableBody2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    [Export]
	protected float paddleSpeed = 10f;
	protected int movementState = 0;
    protected float startSpeed;

    // --------------------------------
    //			PROPERTIES	
    // --------------------------------

    public int MovementState { get => movementState; set => movementState = value; }
    public float StartSpeed { get => startSpeed; }

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
	{
        startSpeed = paddleSpeed;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

        // HandleMovement(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        HandleMovement(delta);
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    protected virtual void HandleMovement(double delta)
	{
        float inputDir = Input.GetAxis("ui_left", "ui_right");
        //movementState = Mathf.RoundToInt(inputDir);

        //if (CanMove())
        //{
        //    Translate(inputDir, 1.0);
        //}
        float movementAmount = inputDir * paddleSpeed * (float)delta;
        MoveAndCollide(Vector2.Right * movementAmount);
    }


    protected bool CanMove()
    {
        return true;
        //Array<Area2D> overlappingAreas = GetOverlappingAreas();
        //for (int i = 0; i < overlappingAreas.Count; i++)
        //{
        //    try
        //    {
        //        Wall potentialWall = (Wall)overlappingAreas[i];
        //        // If hit wall & wall is not supposed to stop object, return true
        //        if (potentialWall != null && movementState == potentialWall.MovementStateToStop)
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        continue;
        //    }
        //}
        //return true;
    }

    protected void Translate(float movementDirection, double delta)
    {
        float movementAmount = movementDirection * paddleSpeed * (float)delta;
        Vector2 newPosition = new Vector2(Position.X + movementAmount, Position.Y);
        Position = newPosition;
    }

    public void ResetSpeed()
    {
        paddleSpeed = startSpeed;
    }
}
