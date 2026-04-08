namespace ScanSort.Models;

public class SourceDocument
{
    public string SourcePath { get; }
    public DocumentType DocumentType { get; }
    public PageMetadataMap PageMetadataMap { get; } = new();

    public string Title => Path.GetFileNameWithoutExtension(SourcePath);

    public string PDFSaveAs => DocumentType == DocumentType.PDF
        ? Path.Combine(Path.GetDirectoryName(SourcePath)!, $"Reordered-{Path.GetFileName(SourcePath)}")
        : SourcePath + ".pdf";

    public string TXTSaveAs => DocumentType == DocumentType.PDF
        ? Path.Combine(Path.GetDirectoryName(SourcePath)!, $"Reordered-{Path.GetFileNameWithoutExtension(SourcePath)}.txt")
        : SourcePath + ".txt";

    public SourceDocument(string path)
    {
        if (File.Exists(path))
        {
            SourcePath = path;
            DocumentType = DocumentType.PDF;
        }
        else if (Directory.Exists(path))
        {
            SourcePath = path;
            DocumentType = DocumentType.ImageFolder;
        }
        else
        {
            throw new FileNotFoundException("Invalid source path.", path);
        }
    }
}
