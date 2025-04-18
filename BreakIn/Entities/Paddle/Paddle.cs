using Godot;
using Godot.Collections;
using System;

public partial class Paddle : Area2D
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------
    [Export]
	protected float paddleSpeed = 10f;
	protected int movementState = 0;
    protected float startSpeed;

    [Export]
    private Dictionary<Area2D, Vector2> DirectionalValues = new Dictionary<Area2D, Vector2>();

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

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Move(delta);
    }

    // --------------------------------
    //		MOVEMENT LOGIC	
    // --------------------------------

    protected virtual void Move(double delta)
	{
        float inputDir = Input.GetAxis("ui_left", "ui_right");
        movementState = Mathf.RoundToInt(inputDir);

        if (CanMove())
        {
            Translate(inputDir, 1.0);
        }
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
        float movementAmount = movementDirection * paddleSpeed * (float)delta;
        Vector2 newPosition = new Vector2(Position.X + movementAmount, Position.Y);
        Position = newPosition;
    }

    public void ResetSpeed()
    {
        paddleSpeed = startSpeed;
    }
}
