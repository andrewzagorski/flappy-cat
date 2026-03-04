using Godot;
using System;

public partial class BoxSpawner : Node2D
{
    [Export]
    public PackedScene BoxScene { get; set; }

    [Export]
    public PackedScene ScoreZoneScene { get; set; }

    [Export]
    public bool SpawnTopBoxes { get; set; } = false;

    [Export]
    public Vector2 BoxYSpawnRange { get; set; }

    private Node _root;

    private Vector2 _screenSize;

    private float _boxOverlapHeight;
    private int _maxBoxes;
    private float _bottomBoxY;
    private float? _topBoxY = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (BoxYSpawnRange == Vector2.Zero)
        {
            BoxYSpawnRange = new Vector2(0, GetViewportRect().Size.Y);
        }
        _screenSize = GetViewportRect().Size;
        _root = GetTree().Root;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void SetBoxSizeConstants()
    {
        if (_boxOverlapHeight == 0)
        {
            Box box = BoxScene.Instantiate<Box>();
            _boxOverlapHeight = box.GetNode<BoxSprite>("BoxSprite").OverlapY * box.Scale.Y;
            box.QueueFree();
            _maxBoxes = Math.Max(0, (int)Math.Floor((BoxYSpawnRange.Y - BoxYSpawnRange.X) / _boxOverlapHeight) - 2);
            _bottomBoxY = BoxYSpawnRange.Y;
            _topBoxY ??= BoxYSpawnRange.X;
        }
    }

    private void OnTimerTimeout()
    {
        SetBoxSizeConstants();

        ScoreZone scoreZone = ScoreZoneScene.Instantiate<ScoreZone>();
        scoreZone.GlobalPosition = new Vector2(GlobalPosition.X + _screenSize.X, 0);
        scoreZone.GetNode<CollisionShape2D>("CollisionShape2D").Shape = new RectangleShape2D() { Size = new Vector2(20, _screenSize.Y) };
        _root.AddChild(scoreZone);

        int numBoxes = (int)Math.Floor(GD.RandRange(0, (double)_maxBoxes));
        for (int i = 0; i < numBoxes; i++)
        {
            Box box = BoxScene.Instantiate<Box>();
            box.GlobalPosition = new Vector2(GlobalPosition.X + _screenSize.X, _bottomBoxY - i * _boxOverlapHeight);
            _root.AddChild(box);
        }
        if (SpawnTopBoxes)
        {
            int numTopBoxes = (int)Math.Floor(GD.RandRange(0, (double)(_maxBoxes - numBoxes)));
            for (int i = 0; i < numTopBoxes; i++)
            {
                Box boxMirror = BoxScene.Instantiate<Box>();
                boxMirror.GlobalPosition = new Vector2(GlobalPosition.X + _screenSize.X, _topBoxY.Value + i * _boxOverlapHeight);
                boxMirror.Scale *= new Vector2(1, -1);
                _root.AddChild(boxMirror);
            }
        }
    }
}
