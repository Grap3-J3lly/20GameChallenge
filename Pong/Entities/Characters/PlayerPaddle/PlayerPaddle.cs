using Godot;
using Godot.Collections;
using System;

public partial class PlayerPaddle : Paddle
{
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		Move(delta);
	}

    protected override void Move(double delta)
    {
		base.Move(delta);

		float inputDir = Input.GetAxis("ui_left", "ui_right");
		movementState = Mathf.RoundToInt(inputDir);

		if (CanMove())
		{
			Translate(inputDir, 1.0);
		}
    }

}
