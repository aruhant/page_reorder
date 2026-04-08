using ScanSort.Models;

namespace ScanSort.Commands;

public class InsertMissingPageCommand : IUndoableCommand
{
    private readonly PageMetadataMap _metadataMap;
    private readonly Action<int, string> _insertItem;
    private readonly Action<string> _removeItem;
    private readonly string _title;
    private readonly int _insertIndex;

    public string Description => $"Insert missing page at position {_insertIndex}";

    public InsertMissingPageCommand(PageMetadataMap metadataMap,
        Action<int, string> insertItem, Action<string> removeItem,
        int insertIndex, int pageNumber)
    {
        _metadataMap = metadataMap;
        _insertItem = insertItem;
        _removeItem = removeItem;
        _insertIndex = insertIndex;
        _title = $"Missing Page ~{Random.Shared.Next()}";

        _metadataMap[_title] = new PageMetadata(string.Empty, _title, PageType.MissingContent, pageNumber);
    }

    public void Execute()
    {
        _insertItem(_insertIndex, _title);
    }

    public void Undo()
    {
        _removeItem(_title);
        _metadataMap.Remove(_title);
    }
}
