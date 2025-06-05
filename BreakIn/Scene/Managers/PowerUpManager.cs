using Godot;
using Godot.Collections;
using System;

public partial class PowerUpManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    public static PowerUpManager Instance;
    public GameManager gameManager;

    // Tri Ball Info
    [Export]
    private float extraBallPositionOffset = 10f;
    [Export]
    private float extraBallVelocityOffset = 50f;

    // Super Ball Info
    [Export]
    private float superBallSpeedBoostAmount = 50f;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        Instance = this;
        gameManager = GameManager.Instance;
    }

    // --------------------------------
    //		    POWERUP LOGIC	
    // --------------------------------

    public void TriggerRandomPowerup()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        int powerupIndex = rng.RandiRange(0, 4);

        switch (powerupIndex)
        {
            case 0: TriggerPowerUp_TriBall();
                break;
            case 1: TriggerPowerUp_SuperBall();
                break;
            case 2: TriggerPowerUp_SuperWide();
                break;
            case 3: TriggerPowerUp_Shield();
                break;
            case 4: TriggerPowerUp_PaddleSpeed();
                break;
        }
    }

    //  - - - Tri Ball - - - // 

    // Tri-Ball: Spawns two more balls at current ball's location in random directions*
    public void Debug_TriBall()
    {
        TriggerPowerUp_TriBall();
    }

    private void TriggerPowerUp_TriBall()
    {
        Ball ballCenter = gameManager.Balls[0];
        
        SpawnExtraBall(ballCenter, Vector2.Left);
        SpawnExtraBall(ballCenter, Vector2.Right);
    }

    private void SpawnExtraBall(Ball primaryBall, Vector2 direction)
    {
        Ball newBall = gameManager.BallScene.Instantiate<Ball>();
        gameManager.ObjectPool.AddChild(newBall);
        gameManager.Balls.Add(newBall);
        newBall.Position = primaryBall.Position + (direction * extraBallPositionOffset);
        newBall.Velocity = primaryBall.Velocity + (direction * extraBallVelocityOffset);
    }

    //  - - - Super Ball - - - // 

    // Super Ball: Destroys all bricks until it hits a wall (min: 1 brick), ball speeds up for duration of powerup
    public void Debug_SuperBall()
    {
        TriggerPowerUp_SuperBall();
    }
    
    private void TriggerPowerUp_SuperBall()
    {
        Array<Ball> balls = gameManager.Balls;
        foreach (Ball ball in balls)
        {
            ball.BallSpeed += superBallSpeedBoostAmount;
            ball.SuperMode = true;
        }
    }

    //  - - - Shield - - - // 

    // Shield: Spawns protective barrier around bottom of game board in front of Paddle (Larger Paddle)
    private void TriggerPowerUp_Shield()
    {

    }

    //  - - - Super Wide - - - // 

    // SuperWide: Stretches Paddle to be larger for short period of time
    private void TriggerPowerUp_SuperWide()
    {

    }

    //  - - - Paddle Speed - - - // 

    // Paddle Speed: Increases Paddle speed for a short period of time
    private void TriggerPowerUp_PaddleSpeed()
    {

    }
}
