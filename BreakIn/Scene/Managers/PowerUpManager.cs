using Godot;
using System;

public partial class PowerUpManager : Node
{
    public static PowerUpManager Instance;
    public GameManager gameManager;

    public override void _Ready()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }

    public void TriggerRandomPowerup()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        int powerupIndex = rng.RandiRange(0, 4);

        switch (powerupIndex)
        {
            case 0: TriggerPowerUp_TriBall();
                break;
            case 1: TriggerPowerUp_Shield();
                break;
            case 2: TriggerPowerUp_SuperWide();
                break;
            case 3: TriggerPowerUp_SuperBall();
                break;
            case 4: TriggerPowerUp_PaddleSpeed();
                break;
        }
    }
    

    // Tri-Ball: Spawns two more balls at current ball's location in random directions*
    public void TriggerPowerUp_TriBall()
    {
        for (int i = 0; i < 2; i++)
        {
            Ball newBall = gameManager.BallScene.Instantiate<Ball>();
            gameManager.ObjectPool.AddChild(newBall);
            newBall.Position = Vector2.Zero;
            gameManager.Balls.Add(newBall);
            newBall.Velocity += -5 * (i + 1) * Vector2.Up;
        }
    }

    // Shield: Spawns protective barrier around bottom of game board in front of Paddle (Larger Paddle)
    private void TriggerPowerUp_Shield()
    {

    }

    // SuperWide: Stretches Paddle to be larger for short period of time
    private void TriggerPowerUp_SuperWide()
    {

    }

    // Super Ball: Destroys all bricks until it hits a wall (min: 1 brick), ball speeds up for duration of powerup
    private void TriggerPowerUp_SuperBall()
    {

    }

    // Paddle Speed: Increases Paddle speed for a short period of time
    private void TriggerPowerUp_PaddleSpeed()
    {

    }
}
