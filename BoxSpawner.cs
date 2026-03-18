using Godot;
using System;

public partial class BoxSpawner : Node2D
{
    [Signal]
    public delegate void ScoredEventHandler();

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

    private void SetBoxSizeConstants()
    {
        if (_boxOverlapHeight == 0)
        {
            Box box = BoxScene.Instantiate<Box>();
            _boxOverlapHeight = box.GetNode<BoxSprite>("BoxSprite").OverlapY * box.Scale.Y;
            box.QueueFree();
            _maxBoxes = Math.Max(0, (int)Math.Floor((BoxYSpawnRange.Y - BoxYSpawnRange.X) / _boxOverlapHeight) - 3);
            _bottomBoxY = BoxYSpawnRange.Y;
            _topBoxY ??= BoxYSpawnRange.X;
        }
    }

    private void OnTimerTimeout()
    {
        SetBoxSizeConstants();

        ScoreZone scoreZone = ScoreZoneScene.Instantiate<ScoreZone>();
        scoreZone.Scored += OnScored;
        _root.AddChild(scoreZone);
        scoreZone.GlobalPosition = new Vector2(GlobalPosition.X + 20 + _screenSize.X, 0);
        scoreZone.GetNode<CollisionShape2D>("CollisionShape2D").Shape = new RectangleShape2D() { Size = new Vector2(60, _screenSize.Y) };

        int numBoxes = (int)Math.Floor(GD.RandRange(2, (double)_maxBoxes - 1));
        for (int i = 0; i < numBoxes; i++)
        {
            Box box = BoxScene.Instantiate<Box>();
            box.GlobalPosition = new Vector2(GlobalPosition.X + _screenSize.X, _bottomBoxY - i * _boxOverlapHeight);
            _root.AddChild(box);
        }
        if (SpawnTopBoxes)
        {
            int numTopBoxes = (int)Math.Floor(GD.RandRange(1, (double)(_maxBoxes - numBoxes)));
            for (int i = 0; i < numTopBoxes; i++)
            {
                Box boxMirror = BoxScene.Instantiate<Box>();
                boxMirror.GlobalPosition = new Vector2(GlobalPosition.X + _screenSize.X, _topBoxY.Value + i * _boxOverlapHeight);
                boxMirror.Scale *= new Vector2(1, -1);
                _root.AddChild(boxMirror);
            }
        }
    }

    private void OnScored()
    {
        EmitSignal(SignalName.Scored);
    }
}
