using Godot;
using System;

public partial class WinText : RichTextLabel
{
	private GameManager gameManager;
	public override void _Ready()
	{
		Visible = false;
		gameManager = GameManager.Instance;
	}
	
	public void SetTextAndShow(string text)
	{
		Text = text;
		Visible = true;
	}
}
