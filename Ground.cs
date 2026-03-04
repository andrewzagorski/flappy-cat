using Godot;
using System;

public partial class Ground : StaticBody2D
{
    [Export]
    public Cat PlayerCat { get; set; }

    public Vector2 ScreenSize;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PlayerCat != null)
        {
            GlobalPosition = new Vector2(PlayerCat.GlobalPosition.X - ScreenSize.X / 2, GlobalPosition.Y);
        }
    }
}
