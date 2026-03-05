using Godot;
using System;

public partial class Cat : CharacterBody2D
{
    [Signal]
    public delegate void DiedEventHandler();

    private float _jumpForce = 400f;

    private bool _isDead = false;

    private bool _isAnimatingDeath = false;

    [Export]
    public float CatSpeed = 90f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Vector2 baseVelocity = Velocity;
        baseVelocity.X = CatSpeed;
        Velocity = baseVelocity;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            if (_isAnimatingDeath)
            {
                Vector2 deadVelocityUpdate = Velocity;
                deadVelocityUpdate.Y += Math.Clamp(GetGravity().Y * (float)delta, -1000f, 1000f);
                Velocity = deadVelocityUpdate;
                GetNode<AnimatedSprite2D>("Sprite").Rotation += (float)(Math.PI * (float)delta);
                MoveAndSlide();
            }
            return;
        }

        Vector2 velocityUpdate = Velocity;
        velocityUpdate.Y += Math.Clamp(GetGravity().Y * (float)delta, -1000f, 1000f);
        Velocity = velocityUpdate;
        GetNode<AnimatedSprite2D>("Sprite").Rotation = (float)(Math.PI / 2 * Velocity.Y / 1000.0);

        MoveAndSlide();

        Vector2 positionUpdate = Position;
        if (positionUpdate.Y < 0)
        {
            positionUpdate.Y = 0;
            velocityUpdate.Y = 0;
            Velocity = velocityUpdate;
            Position = positionUpdate;
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            Node collider = (Node)collision.GetCollider();

            if (collider is Ground)
            {
                velocityUpdate.Y = 0;
                Velocity = velocityUpdate;
                GetNode<AnimatedSprite2D>("Sprite").Play("run");
            }
            else if (collider.IsInGroup("boxes"))
            {
                Die();
            }
        }
        if (Velocity.Y > 0)
        {
            GetNode<AnimatedSprite2D>("Sprite").Play("hover");
        }
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (_isDead) return;
        if (@event.IsActionPressed("jump"))
        {
            GetNode<AnimatedSprite2D>("Sprite").Play("jump");
            Vector2 velocityUpdate = Velocity;
            velocityUpdate.Y -= _jumpForce;
            Velocity = velocityUpdate;
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        Velocity = Vector2.Zero;
        SetCollisionLayerValue(1, false);
        SetCollisionMaskValue(2, false);
        SetCollisionMaskValue(3, false);
        BoxSpawner boxSpawner = GetNode<BoxSpawner>("BoxSpawner");
        boxSpawner.GetNode<Timer>("Timer").Stop();
        EmitSignal("Died");
        GetNode<Timer>("DeathAnimationTimer").Start();
    }

    private void AnimateDeath()
    {
        // Translates and rotates the cat to fall off the screen to simulate a death animation

    }

    private void OnDeathAnimationTimerTimeout()
    {
        _isAnimatingDeath = true;
        Velocity = new Vector2(60, 0);
    }

    public void OnScreenExited()
    {
        GetNode<AnimatedSprite2D>("Sprite").Hide();
        _isAnimatingDeath = false;
        Velocity = Vector2.Zero;
    }

    public void Reset()
    {
        _isDead = false;
        _isAnimatingDeath = false;
        Velocity = Vector2.Zero;
        Position = new Vector2(100, 200);
        GetNode<AnimatedSprite2D>("Sprite").Show();
        GetNode<AnimatedSprite2D>("Sprite").Play("hover");
        SetCollisionLayerValue(1, true);
        SetCollisionMaskValue(2, true);
        SetCollisionMaskValue(3, true);
        BoxSpawner boxSpawner = GetNode<BoxSpawner>("BoxSpawner");
        boxSpawner.GetNode<Timer>("Timer").Start();
    }
}
