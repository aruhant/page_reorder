using ScanSort.Models;

namespace ScanSort.Commands;

public class ToggleBlurCommand : IUndoableCommand
{
    private readonly PageMetadata _metadata;

    public string Description => $"Toggle blur on {_metadata.Title}";

    public ToggleBlurCommand(PageMetadata metadata)
    {
        _metadata = metadata;
    }

    public void Execute() => _metadata.Blurred = 1 - _metadata.Blurred;
    public void Undo() => _metadata.Blurred = 1 - _metadata.Blurred;
}
