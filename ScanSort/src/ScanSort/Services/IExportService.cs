using ScanSort.Models;

namespace ScanSort.Services;

public interface IExportService
{
    /// <summary>
    /// Exports the reordered document as PDF + comments TXT file.
    /// </summary>
    Task ExportAsync(SourceDocument document, IReadOnlyList<string> orderedTitles,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default);
}
