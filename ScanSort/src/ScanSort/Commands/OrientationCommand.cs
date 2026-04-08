using ScanSort.Models;

namespace ScanSort.Commands;

/// <summary>
/// Toggles the coarse Orientation by 90 degrees.
/// </summary>
public class OrientationCommand : IUndoableCommand
{
    private readonly PageMetadata _metadata;
    private readonly int _delta;

    public string Description => $"Rotate layout of {_metadata.Title} by {_delta}°";

    public OrientationCommand(PageMetadata metadata, int angleDelta = 90)
    {
        _metadata = metadata;
        _delta = angleDelta;
    }

    public void Execute() => _metadata.Orientation = (_metadata.Orientation + _delta) % 360;
    public void Undo() => _metadata.Orientation = (_metadata.Orientation - _delta + 360) % 360;
}
