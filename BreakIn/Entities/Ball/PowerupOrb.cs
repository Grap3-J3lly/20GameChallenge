using Godot;
using Godot.Collections;
using System;

public partial class PowerupOrb : Ball
{
    private PowerUpManager powerUpManager;
    [Export]
    private Array<Color> colors;
    private int powerupIndex = 0;


    public override void _Ready()
    {
        currentDirection = Vector2.Down;
        base._Ready();
        powerUpManager = breakoutManager.PowerUpManager;

        RandomNumberGenerator rng = new RandomNumberGenerator();
        GD.Print($"PowerupOrb.cs: Colors Count: {colors.Count}");
        powerupIndex = rng.RandiRange(0, colors.Count-1);
        GD.Print($"PowerupOrb.cs: Index: {powerupIndex}");
        Modulate = colors[powerupIndex];
    }

    public override void _PhysicsProcess(double delta)
    {
        // base._PhysicsProcess(delta);
        if (breakoutManager.GameOver)
        {
            return;
        }
        HandleMovement(delta);
        HandleCollision();
    }

    public override void HandleCollision()
    {
        if (collisionsWaiting.Count <= 0) { return; }
        KinematicCollision2D currentCollision = collisionsWaiting.Dequeue();
        GD.Print($"Ball.cs: Colliding Body's Godot Object: {currentCollision.GetCollider()}");
        GodotObject collidingObject = currentCollision.GetCollider();

        Paddle potentialPaddle = collidingObject as Paddle;
        Goal potentialGoal = collidingObject as Goal;
        if (potentialPaddle != null)
        {
            HandlePaddleImpact(currentCollision);
        }
        if (potentialPaddle != null || potentialGoal != null)
        {
            QueueFree();
        }
    }

    protected override void HandlePaddleImpact(KinematicCollision2D collisionInfo)
    {
        powerUpManager.TriggerPowerupByIndex(powerupIndex);
    }
}
