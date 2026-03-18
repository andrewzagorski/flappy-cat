using Godot;

public partial class Main : Node
{

    private Label _infoLabel;
    private Score _score;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        CanvasLayer ui = GetNode<CanvasLayer>("UI");
        _infoLabel = ui.GetNode<Label>("InfoLabel");
        _score = ui.GetNode<Score>("Score");
    }

    public void OnCatDied()
    {
        GetNode<BoxSpawner>("BoxSpawner").StopSpawning();

        Node obstaclesContainer = GetNode<Node>("ObstaclesContainer");
        foreach (Node child in obstaclesContainer.GetChildren())
        {
            child.QueueFree();
        }


        _score.Hide();
        _infoLabel.Text = "Game Over. Score: " + _score.ScoreValue;
        _infoLabel.Show();
    }

    public void OnGameStarted()
    {
        _infoLabel.Hide();
        _score.ResetScore();
        _score.Show();
        GetNode<BoxSpawner>("BoxSpawner").StartSpawning();
    }
}
