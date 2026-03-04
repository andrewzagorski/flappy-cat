using Godot;
using System;

public partial class BoxSprite : Sprite2D
{
    [Export]
    public float OverlapY { get; set; } = 9;
}
