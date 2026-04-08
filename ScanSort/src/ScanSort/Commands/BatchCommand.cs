namespace ScanSort.Commands;

/// <summary>
/// Groups multiple commands into a single undo step (e.g. rotating 5 selected pages).
/// </summary>
public class BatchCommand : IUndoableCommand
{
    private readonly List<IUndoableCommand> _commands;

    public string Description { get; }

    public BatchCommand(string description, IEnumerable<IUndoableCommand> commands)
    {
        Description = description;
        _commands = commands.ToList();
    }

    public void Execute()
    {
        foreach (var cmd in _commands) cmd.Execute();
    }

    public void Undo()
    {
        // Undo in reverse order
        for (int i = _commands.Count - 1; i >= 0; i--)
            _commands[i].Undo();
    }
}
