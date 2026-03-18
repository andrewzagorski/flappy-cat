using Godot;

public partial class ScoreZone : Area2D
{
    [Signal]
    public delegate void ScoredEventHandler();

    private void OnBodyEntered(Node2D body)
    {
        if (body is Cat && !IsQueuedForDeletion())
        {
            EmitSignal(SignalName.Scored);
            QueueFree();
        }
    }
}
