using ScanSort.Models;

namespace ScanSort.Commands;

/// <summary>
/// Changes the fine Rotate value on a page (deskew correction).
/// </summary>
public class RotateCommand : IUndoableCommand
{
    private readonly PageMetadata _metadata;
    private readonly float _delta;

    public string Description => $"Rotate {_metadata.Title} by {_delta}°";

    public RotateCommand(PageMetadata metadata, float angleDelta)
    {
        _metadata = metadata;
        _delta = angleDelta;
    }

    public void Execute() => _metadata.Rotate += _delta;
    public void Undo() => _metadata.Rotate -= _delta;
}
