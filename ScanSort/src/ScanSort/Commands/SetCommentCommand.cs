using ScanSort.Models;

namespace ScanSort.Commands;

public class SetCommentCommand : IUndoableCommand
{
    private readonly PageMetadata _metadata;
    private readonly string _newComment;
    private readonly string _oldComment;

    public string Description => $"Set comment on {_metadata.Title}";

    public SetCommentCommand(PageMetadata metadata, string newComment)
    {
        _metadata = metadata;
        _newComment = newComment;
        _oldComment = metadata.Comment;
    }

    public void Execute() => _metadata.Comment = _newComment;
    public void Undo() => _metadata.Comment = _oldComment;
}
