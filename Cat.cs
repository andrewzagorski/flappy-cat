using Godot;
using System;

public partial class Cat : CharacterBody2D
{
    [Signal]
    public delegate void DiedEventHandler();

    [Signal]
    public delegate void StartedEventHandler();

    [Export]
    public float JumpForce { get; set; } = 400f;

    private enum CatState { _waitingToPlay, _playing, _dead, _animatingDeath }

    private CatState _currentState = CatState._waitingToPlay;

    [Export]
    public float CatSpeed { get; set; } = 90f;

    private AnimatedSprite2D _sprite;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _sprite = GetNode<AnimatedSprite2D>("Sprite");
        Hide();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_currentState == CatState._dead)
        {
            return;
        }

        if (_currentState == CatState._animatingDeath)
        {
            Vector2 deadVelocityUpdate = Velocity;
            deadVelocityUpdate.Y += Math.Clamp(GetGravity().Y * (float)delta, -1000f, 1000f);
            Velocity = deadVelocityUpdate;
            _sprite.Rotation += (float)(Math.PI * (float)delta);
            MoveAndSlide();
        }

        Vector2 velocityUpdate = Velocity;
        velocityUpdate.Y += Math.Clamp(GetGravity().Y * (float)delta, -1000f, 1000f);
        Velocity = velocityUpdate;
        _sprite.Rotation = (float)(Math.PI / 2 * Velocity.Y / 1000.0);

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
                _sprite.Play("run");
            }
            else if (collider.IsInGroup("boxes"))
            {
                Die();
            }
        }
        if (Velocity.Y > 0)
        {
            _sprite.Play("hover");
        }
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (_currentState == CatState._waitingToPlay)
        {
            if (@event.IsActionPressed("jump"))
            {
                Reset();
                EmitSignal(SignalName.Started);
            }
            return;
        }
        if (_currentState == CatState._dead) return;
        if (@event.IsActionPressed("jump"))
        {
            _sprite.Play("jump");
            Vector2 velocityUpdate = Velocity;
            velocityUpdate.Y -= JumpForce;
            Velocity = velocityUpdate;
        }
    }

    private void Die()
    {
        if (_currentState == CatState._dead) return;
        _currentState = CatState._dead;
        Velocity = Vector2.Zero;
        SetCollisionLayerValue(PhysicsLayer.Cat, false);
        SetCollisionMaskValue(PhysicsLayer.Box, false);
        SetCollisionMaskValue(PhysicsLayer.Ground, false);
        BoxSpawner boxSpawner = GetNode<BoxSpawner>("BoxSpawner");
        boxSpawner.GetNode<Timer>("Timer").Stop();
        EmitSignal(SignalName.Died);
        GetNode<Timer>("DeathAnimationTimer").Start();
    }

    private void OnDeathAnimationTimerTimeout()
    {
        _currentState = CatState._animatingDeath;
        Velocity = new Vector2(60, 0);
    }

    public void OnScreenExited()
    {
        _sprite.Hide();
        _currentState = CatState._waitingToPlay;
        Velocity = Vector2.Zero;
        Hide();
    }

    public void Reset()
    {
        _currentState = CatState._playing;
        Show();
        Vector2 baseVelocity = Velocity;
        baseVelocity.X = CatSpeed;
        Velocity = baseVelocity;
        Vector2 viewSize = GetViewport().GetVisibleRect().Size;
        float spriteHeight = _sprite.SpriteFrames.GetFrameTexture("hover", 0).GetHeight();
        Position = new Vector2(viewSize.X / 2, viewSize.Y - spriteHeight * Scale.Y);
        _sprite.Show();
        _sprite.Play("hover");
        SetCollisionLayerValue(PhysicsLayer.Cat, true);
        SetCollisionMaskValue(PhysicsLayer.Box, true);
        SetCollisionMaskValue(PhysicsLayer.Ground, true);
        BoxSpawner boxSpawner = GetNode<BoxSpawner>("BoxSpawner");
        boxSpawner.GetNode<Timer>("Timer").Start();
    }
}
