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

    private Paddle paddle;

    // Tri Ball Info
    [Export]
    private float extraBallPositionOffset = 10f;
    [Export]
    private float extraBallVelocityOffset = 50f;

    // Super Ball Info
    [Export]
    private float superBallSpeedBoostAmount = 50f;

    // Shield Info
    [Export]
    private PackedScene wallScene;
    private Wall shield;
    [Export]
    private float shieldWidth = 2f;
    [Export]
    private float shieldDuration = 10f;
    private float shieldTimer = -1;

    // Super Wide Info
    [Export]
    private float superWidePaddleWidth = 2f;
    [Export]
    private float superWideDuration = 10f;
    private float superWideTimer = -1;

    // Paddle Speed Info
    [Export]
    private float maxPaddleSpeed = 1000f;
    [Export]
    private float paddleSpeedDuration = 10f;
    private float paddleSpeedTimer = -1;

    // --------------------------------
    //		STANDARD FUNCTIONS	
    // --------------------------------

    public override void _Ready()
    {
        Instance = this;
        gameManager = GameManager.Instance;
        paddle = gameManager.Paddle;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        float realDelta = (float)delta;

        RunSuperWideTimer(realDelta);
        RunPaddleSpeedTimer(realDelta);
        RunShieldTimer(realDelta);
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
    // Tri-Ball: Spawns two more balls at current ball's location in random directions

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
    // Destroys all bricks until it hits a wall (min: 1 brick), ball speeds up for duration of powerup

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

    public void Debug_Shield()
    {
        TriggerPowerUp_Shield();
    }

    private void TriggerPowerUp_Shield()
    {
        shield = wallScene.Instantiate<Wall>();
        gameManager.ObjectPool.AddChild(shield);
        shield.Rotation = Mathf.Pi/2;
        shield.Position = new Vector2(gameManager.Paddle.Position.X, gameManager.Paddle.Position.Y - 100);
        shield.Scale = new Vector2(shield.Scale.X, shield.Scale.Y * shieldWidth);
        shieldTimer = shieldDuration;
    }

    private void RunShieldTimer(float realDelta)
    {
        if (shieldTimer > 0)
        {
            shieldTimer -= realDelta;
            if (shieldTimer <= 0)
            {
                shieldTimer = 0;
                shield.QueueFree();
            }
        }
    }

    //  - - - Super Wide - - - // 
    // SuperWide: Stretches Paddle to be larger for short period of time
    public void Debug_SuperWide()
    {
        TriggerPowerUp_SuperWide();
    }
    
    
    private void TriggerPowerUp_SuperWide()
    {
        paddle.ChangePaddleSize(superWidePaddleWidth, true);
        superWideTimer = superWideDuration;
    }

    private void RunSuperWideTimer(float realDelta)
    {
        if (superWideTimer > 0)
        {
            superWideTimer -= realDelta;
            if (superWideTimer <= 0)
            {
                superWideTimer = 0;
                paddle.ResetPaddleSize();
            }
        }
    }

    //  - - - Paddle Speed - - - // 
    // Paddle Speed: Increases Paddle speed for a short period of time

    public void Debug_PaddleSpeed()
    {
        TriggerPowerUp_PaddleSpeed();
    }
    
    private void TriggerPowerUp_PaddleSpeed()
    {
        paddle.PaddleSpeed = maxPaddleSpeed;
        paddleSpeedTimer = paddleSpeedDuration;
    }

    private void RunPaddleSpeedTimer(float realDelta)
    {
        if (paddleSpeedTimer > 0)
        {
            paddleSpeedTimer -= realDelta;
            if (paddleSpeedTimer <= 0)
            {
                paddleSpeedTimer = 0;
                paddle.ResetSpeed();
            }
        }
    }
}
