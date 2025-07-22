using Godot;
using Godot.Collections;
using System;

public partial class PowerUpManager : Node
{
    // --------------------------------
    //			VARIABLES	
    // --------------------------------

    public static PowerUpManager Instance;
    public BreakoutManager breakoutManager;

    private Paddle paddle;

    // Tri Ball Info
    [Export]
    private float extraBallPositionOffset = 10f;
    [Export]
    private float extraBallVelocityOffset = 50f;
    private bool triBallRunning = false;

    // Super Ball Info
    [Export]
    private float superBallSpeedBoostAmount = 50f;

    // Shield Info
    [Export]
    private PackedScene wallScene;
    private Wall shield;
    [Export]
    private Vector2 shieldPosition = new Vector2();
    [Export]
    private float shieldWidth = 2f;
    [Export]
    private Color shieldColor;

    [Export]
    private float shieldDuration = 10f;
    private float shieldTimer = -1;

    // Super Wide Info
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
        breakoutManager = BreakoutManager.Instance;
        paddle = breakoutManager.Paddle;
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

    public void ResetPowerups()
    {
        if(breakoutManager != null)
        {
            paddle = breakoutManager.Paddle;
        }
        // Triball gets handled during regular setup

        // Disables Superball
        if(breakoutManager != null && breakoutManager.Balls != null)
        {
            Array<Ball> balls = breakoutManager.Balls;
            foreach (Ball ball in balls)
            {
                if (ball != null)
                {
                    ball.SuperMode = false;
                }
            }
        }

        shieldTimer = 0;
        superWideTimer = 0;
        paddleSpeedTimer = 0;
    }

    /// <summary>
    /// Trigger one of the supported powerups by a given index. 
    /// </summary>
    /// <param name="powerupIndex">
    /// Index 0: TriBall
    /// Index 1: SuperBall
    /// Index 2: SuperWide
    /// Index 3: Shield
    /// Index 4: PaddleSpeed
    /// </param>
    public void TriggerPowerupByIndex(int powerupIndex)
    {
        //RandomNumberGenerator rng = new RandomNumberGenerator();
        //int powerupIndex = rng.RandiRange(0, 4);

        paddle = breakoutManager.Paddle;

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
        paddle = breakoutManager.Paddle;
        TriggerPowerUp_TriBall();
    }

    private void TriggerPowerUp_TriBall()
    {
        if(triBallRunning)
        {
            return;
        }
        triBallRunning = true;

        GD.Print($"PowerupManager.cs: Triggering TriBall");

        Ball leftBall = null;
        Ball rightBall = null;

        Array<Ball> newBalls = new Array<Ball>();

        for(int i = 0; i < breakoutManager.Balls.Count; i++)
        {
            Ball ballCenter = breakoutManager.Balls[i];
        
            leftBall = SpawnExtraBall(ballCenter, Vector2.Left);
            rightBall = SpawnExtraBall(ballCenter, Vector2.Right);

            newBalls.Add(leftBall);
            newBalls.Add(rightBall);
        }
        breakoutManager.Balls.AddRange(newBalls);
        triBallRunning = false;
    }

    private Ball SpawnExtraBall(Ball primaryBall, Vector2 direction)
    {
        Ball newBall = breakoutManager.BallScene.Instantiate<Ball>();
        breakoutManager.ObjectPool.AddChild(newBall);
        newBall.Position = primaryBall.Position + (direction * extraBallPositionOffset);
        newBall.Velocity = primaryBall.Velocity + (direction * extraBallVelocityOffset);
        newBall.Modulate = primaryBall.Modulate;
        return newBall;
    }

    //  - - - Super Ball - - - // 
    // Destroys all bricks until it hits a wall (min: 1 brick), ball speeds up for duration of powerup

    public void Debug_SuperBall()
    {
        paddle = breakoutManager.Paddle;
        TriggerPowerUp_SuperBall();
    }
    
    private void TriggerPowerUp_SuperBall()
    {
        GD.Print($"PowerupManager.cs: Triggering SuperBall");
        Array<Ball> balls = breakoutManager.Balls;
        foreach (Ball ball in balls)
        {
            if(!ball.SuperMode)
            {
                ball.BallSpeed += superBallSpeedBoostAmount;
            }
            ball.SuperMode = true;
            ball.BrickBreakCount = 0;
        }
    }

    //  - - - Shield - - - // 
    // Shield: Spawns protective barrier around bottom of game board in front of Paddle (Larger Paddle)

    public void Debug_Shield()
    {
        paddle = breakoutManager.Paddle;
        TriggerPowerUp_Shield();
    }

    private void TriggerPowerUp_Shield()
    {
        GD.Print($"PowerupManager.cs: Triggering Shield");
        shieldTimer = shieldDuration;
        if(shield != null) { return; }

        shield = wallScene.Instantiate<Wall>();
        //shield.Modulate = shieldColor;
        breakoutManager.ObjectPool.AddChild(shield);
        shield.Rotation = Mathf.Pi/2;
        shield.Position = shieldPosition;
        shield.Scale = new Vector2(shield.Scale.X, shield.Scale.Y * shieldWidth);
    }

    private void RunShieldTimer(float realDelta)
    {
        if(shield != null)
        {
            GD.Print($"PowerUpManager.cs: Shield Value: {shield}");
        }
        if (shieldTimer >= 0 && shield != null)
        {
            shieldTimer -= realDelta;
            if (shieldTimer < 0)
            {
                GD.Print("PowerUpManager.cs: Deleting Shield");
                shieldTimer = 0;
                shield.Free();
                shield = null;
            }
        }
    }

    //  - - - Super Wide - - - // 
    // SuperWide: Stretches Paddle to be larger for short period of time
    public void Debug_SuperWide()
    {
        paddle = breakoutManager.Paddle;
        TriggerPowerUp_SuperWide();
    }
    
    
    private void TriggerPowerUp_SuperWide()
    {
        GD.Print($"PowerupManager.cs: Triggering SuperWide");
        if(!IsInstanceValid(paddle))
        {
            return;
        }
        paddle.ChangePaddleSize(isSuper: true);
        superWideTimer = superWideDuration;
    }

    private void RunSuperWideTimer(float realDelta)
    {
        if (superWideTimer >= 0)
        {
            superWideTimer -= realDelta;
            if (superWideTimer < 0 || paddle.IsQueuedForDeletion())
            {
                superWideTimer = 0;
                paddle.ChangePaddleSize(1);
            }
        }
    }

    //  - - - Paddle Speed - - - // 
    // Paddle Speed: Increases Paddle speed for a short period of time

    public void Debug_PaddleSpeed()
    {
        paddle = breakoutManager.Paddle;
        TriggerPowerUp_PaddleSpeed();
    }
    
    private void TriggerPowerUp_PaddleSpeed()
    {
        GD.Print($"PowerupManager.cs: Triggering PaddleSpeed");
        paddle.PaddleSpeed = maxPaddleSpeed;
        paddleSpeedTimer = paddleSpeedDuration;
    }

    private void RunPaddleSpeedTimer(float realDelta)
    {
        if (paddleSpeedTimer >= 0)
        {
            paddleSpeedTimer -= realDelta;
            if (paddleSpeedTimer < 0)
            {
                paddleSpeedTimer = 0;
                paddle.ResetSpeed();
            }
        }
    }
}
