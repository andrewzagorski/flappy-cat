using Godot;
using System;

public partial class Cat : CharacterBody2D
{

    public Vector2 ScreenSize;

    private float _jumpForce = 400f;

    private float _catHeight;

    private bool _isOnGround = false;

    [Export]
    public float CatSpeed = 90f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        _catHeight = GetNode<CollisionShape2D>("CatShape").Shape.GetRect().Size.Y;
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
        Velocity = Vector2.Zero;
        GD.Print("Cat died!");
    }
}
