using Godot;

public partial class Box : StaticBody2D
{

    public void OnScreenExited()
    {
        QueueFree();
    }
}
