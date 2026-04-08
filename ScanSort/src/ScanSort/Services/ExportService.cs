using ScanSort.Models;

namespace ScanSort.Services;

public class ExportService : IExportService
{
    private readonly IPdfService _pdfService;

    public ExportService(IPdfService pdfService)
    {
        _pdfService = pdfService;
    }

    public async Task ExportAsync(SourceDocument document, IReadOnlyList<string> orderedTitles,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default)
    {
        // Check for overwrite
        if (File.Exists(document.PDFSaveAs))
        {
            var result = MessageBox.Show(
                $"File '{Path.GetFileName(document.PDFSaveAs)}' already exists. Overwrite?",
                "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes) return;
        }

        // Export PDF
        await _pdfService.ExportPdfAsync(document.PDFSaveAs, orderedTitles, document.PageMetadataMap, progress, ct);

        // Export comments
        await Task.Run(() => ExportComments(document.TXTSaveAs, orderedTitles, document.PageMetadataMap), ct);

        MessageBox.Show("PDF saved successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Open the exported PDF in the default viewer
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = document.PDFSaveAs,
                UseShellExecute = true
            });
        }
        catch { }
    }

    private static void ExportComments(string savePath, IReadOnlyList<string> orderedTitles, PageMetadataMap metadataMap)
    {
        using var writer = new StreamWriter(savePath);
        foreach (var title in orderedTitles)
        {
            var metadata = metadataMap[title];
            if (metadata == null) continue;

            writer.WriteLine(metadata.ToString());
            if (!string.IsNullOrWhiteSpace(metadata.Comment))
            {
                string pageLabel = metadata.Title.Contains('.') ? metadata.Title.Split('.')[0] : metadata.Title;
                writer.WriteLine($"Page: {pageLabel} : {metadata.Comment}");
            }
        }
    }
}
