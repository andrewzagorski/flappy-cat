using Godot;
using System;

public partial class Score : Label
{
    [Export]
    public int ScoreValue { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void AddScore(int points)
    {
        ScoreValue += points;
        Text = ScoreValue.ToString();
    }

    public void OnBoxSpawnerScored()
    {
        AddScore(1);
    }

    public void ResetScore()
    {
        ScoreValue = 0;
        Text = ScoreValue.ToString();
    }
}
