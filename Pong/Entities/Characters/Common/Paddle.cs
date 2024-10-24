using Godot;
using Godot.Collections;
using System;

public partial class Paddle : Area2D
{
	[Export]
	protected float paddleSpeed = 10f;
	protected int movementState = 0;
    protected float startSpeed;

	public int MovementState { get => movementState; set => movementState = value; }
    public float StartSpeed { get => startSpeed; }

	public override void _Ready()
	{
        startSpeed = paddleSpeed;
	}


    protected virtual void Move(double delta)
	{
	}


    protected bool CanMove()
    {
        Array<Area2D> overlappingAreas = GetOverlappingAreas();
        for (int i = 0; i < overlappingAreas.Count; i++)
        {
            try
            {
                Wall potentialWall = (Wall)overlappingAreas[i];
                // If hit wall & wall is not supposed to stop object, return true
                if (potentialWall != null && movementState == potentialWall.MovementStateToStop)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                continue;
            }
        }
        return true;
    }

    protected void Translate(float movementDirection, double delta)
    {
        Vector2 newPosition = new Vector2(Position.X, Position.Y + (movementDirection * paddleSpeed * (float)delta));
        Position = newPosition;
    }

    public void ResetSpeed()
    {
        paddleSpeed = startSpeed;
    }
}
