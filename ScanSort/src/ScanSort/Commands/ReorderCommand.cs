using ScanSort.Models;

namespace ScanSort.Commands;

/// <summary>
/// Captures before/after ordering of pages for undo/redo of drag-drop operations.
/// </summary>
public class ReorderCommand : IUndoableCommand
{
    private readonly PageMetadataMap _metadataMap;
    private readonly Action<IReadOnlyList<string>> _applyOrder;
    private readonly IReadOnlyList<string> _beforeOrder;
    private readonly IReadOnlyList<string> _afterOrder;

    public string Description => "Reorder pages";

    /// <param name="metadataMap">The metadata map to sync page numbers on.</param>
    /// <param name="applyOrder">Callback to update the UI list to match the given order.</param>
    /// <param name="beforeOrder">Page titles before the reorder.</param>
    /// <param name="afterOrder">Page titles after the reorder.</param>
    public ReorderCommand(PageMetadataMap metadataMap, Action<IReadOnlyList<string>> applyOrder,
        IReadOnlyList<string> beforeOrder, IReadOnlyList<string> afterOrder)
    {
        _metadataMap = metadataMap;
        _applyOrder = applyOrder;
        _beforeOrder = beforeOrder;
        _afterOrder = afterOrder;
    }

    public void Execute()
    {
        _applyOrder(_afterOrder);
        _metadataMap.SyncPageNumbers(_afterOrder);
    }

    public void Undo()
    {
        _applyOrder(_beforeOrder);
        _metadataMap.SyncPageNumbers(_beforeOrder);
    }
}
