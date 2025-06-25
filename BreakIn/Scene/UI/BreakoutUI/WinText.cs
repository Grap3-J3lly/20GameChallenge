using Godot;
using System;

public partial class WinText : RichTextLabel
{
	private BreakoutManager gameManager;
	public override void _Ready()
	{
		Visible = false;
		gameManager = BreakoutManager.Instance;
	}
	
	public void SetTextAndShow(string text)
	{
		Text = text;
		Visible = true;
	}
}
