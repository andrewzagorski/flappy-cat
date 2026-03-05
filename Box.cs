using Godot;
using System;

public partial class Box : StaticBody2D
{
    public override void _Ready()
    {
    }


    public override void _PhysicsProcess(double delta)
    {
    }

    public void OnScreenExited()
    {
        QueueFree();
    }
}
