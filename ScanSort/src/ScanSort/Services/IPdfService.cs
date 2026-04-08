using ScanSort.Models;

namespace ScanSort.Services;

public interface IPdfService
{
    /// <summary>
    /// Extracts embedded images from a PDF into the output folder.
    /// Populates the PageMetadataMap with per-page metadata.
    /// </summary>
    Task ExtractImagesAsync(string pdfPath, string outputFolder, PageMetadataMap metadataMap,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default);

    /// <summary>
    /// Creates a new PDF from the ordered pages with lossless rotation metadata.
    /// </summary>
    Task ExportPdfAsync(string savePath, IReadOnlyList<string> orderedTitles, PageMetadataMap metadataMap,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default);
}
