using Godot;
using System;

public partial class PlayButton : Button
{
	[Export]
	private CanvasLayer menuCanvas;
	[Export]
	private float secondsToGameStart = 3f;
	private float gameStartTimer = 0;
	private bool gameStarting = false;

	public override void _Ready()
	{
		Pressed += PressButton;
		menuCanvas.Visible = true;
	}

    public override void _Process(double delta)
    {
        base._Process(delta);

		if(gameStartTimer > 0)
		{
			GameManager.Instance.UpdateTimerUI(Mathf.Ceil(gameStartTimer));
			gameStartTimer -= (float)delta;
		}
		if(gameStartTimer <= 0 && GameManager.Instance.GameOver && gameStarting)
		{
			DelayStartGame();
		}
    }

    private void PressButton()
	{
		gameStarting = true;
		gameStartTimer = secondsToGameStart;
        menuCanvas.Visible = false;
	}

	private void DelayStartGame()
	{
		GameManager.Instance.DelayStart();
		gameStarting = false;
    }

}
