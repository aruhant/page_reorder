using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using ScanSort.Models;

namespace ScanSort.Services;

public class PdfService : IPdfService
{
    private readonly IImageService _imageService;

    public PdfService(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task ExtractImagesAsync(string pdfPath, string outputFolder, PageMetadataMap metadataMap,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default)
    {
        await Task.Run(() =>
        {
            using var document = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
            int totalPages = document.PageCount;
            int imageIndex = 0;

            for (int pageIdx = 0; pageIdx < totalPages; pageIdx++)
            {
                ct.ThrowIfCancellationRequested();
                progress?.Report((pageIdx + 1, totalPages, $"Extracting page {pageIdx + 1} of {totalPages}..."));

                var page = document.Pages[pageIdx];
                int rotation = page.Rotate;
                var mediaBox = ToRectangle(page.MediaBox);
                var cropBox = page.CropBox.IsEmpty ? Rectangle.Empty : ToRectangle(page.CropBox);

                // Extract images from page resources
                var resources = page.Elements.GetDictionary("/Resources");
                if (resources == null) continue;

                var xObjects = resources.Elements.GetDictionary("/XObject");
                if (xObjects == null) continue;

                foreach (var key in xObjects.Elements.Keys)
                {
                    ct.ThrowIfCancellationRequested();

                    // In PDFsharp 6, GetObject resolves references automatically
                    var xObject = xObjects.Elements.GetDictionary(key);
                    if (xObject == null) continue;

                    var subtype = xObject.Elements.GetName("/Subtype");
                    if (subtype != "/Image") continue;

                    try
                    {
                        string title = $"{imageIndex:D3}.jpg";
                        string filePath = Path.Combine(outputFolder, title);

                        // Extract image bytes from the stream
                        byte[]? imageBytes = xObject.Stream?.Value;
                        if (imageBytes == null || imageBytes.Length == 0)
                            continue;

                        using var ms = new MemoryStream(imageBytes);
                        using var img = Image.FromStream(ms);
                        using var processed = _imageService.CropToBoundsAndRotate(img, cropBox, mediaBox, 0);
                        processed.Save(filePath, ImageFormat.Jpeg);

                        var metadata = new PageMetadata(filePath, title, originalPageNumber: imageIndex)
                        {
                            ClipRect = cropBox,
                            MediaRect = mediaBox,
                            Orientation = rotation
                        };
                        metadataMap[title] = metadata;
                        imageIndex++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error extracting image {imageIndex} from page {pageIdx}: {ex.Message}");
                    }
                }
            }
        }, ct);
    }

    public async Task ExportPdfAsync(string savePath, IReadOnlyList<string> orderedTitles, PageMetadataMap metadataMap,
        IProgress<(int current, int total, string message)>? progress = null,
        CancellationToken ct = default)
    {
        await Task.Run(() =>
        {
            using var document = new PdfDocument();
            int total = orderedTitles.Count;

            for (int i = 0; i < total; i++)
            {
                ct.ThrowIfCancellationRequested();
                progress?.Report((i + 1, total, $"Writing page {i + 1} of {total}..."));

                var metadata = metadataMap[orderedTitles[i]];
                if (metadata == null) continue;

                if (metadata.PageType == PageType.MissingContent)
                {
                    AddMissingPagePlaceholder(document, "Missing Page");
                    continue;
                }

                var pageSize = metadata.PageSizeWithRotation;
                var page = document.AddPage();
                page.Width = XUnit.FromPoint(pageSize.Width);
                page.Height = XUnit.FromPoint(pageSize.Height);
                page.Rotate = metadata.Orientation;

                using var gfx = XGraphics.FromPdfPage(page);

                if (Math.Abs(metadata.Rotate) > 0.01f)
                {
                    // Apply fine rotation as a rendering transform
                    gfx.TranslateTransform(pageSize.Width / 2, pageSize.Height / 2);
                    gfx.RotateTransform(-metadata.Rotate);

                    var sizeNoRot = metadata.PageSizeWithoutRotation;
                    gfx.TranslateTransform(-sizeNoRot.Width / 2, -sizeNoRot.Height / 2);

                    using var xImage = XImage.FromFile(metadata.FileName);
                    gfx.DrawImage(xImage, 0, 0, sizeNoRot.Width, sizeNoRot.Height);
                }
                else
                {
                    using var xImage = XImage.FromFile(metadata.FileName);
                    gfx.DrawImage(xImage, 0, 0, pageSize.Width, pageSize.Height);
                }
            }

            document.Save(savePath);
        }, ct);
    }

    private static void AddMissingPagePlaceholder(PdfDocument document, string text)
    {
        var page = document.AddPage();
        page.Width = XUnit.FromPoint(595); // A4 width
        page.Height = XUnit.FromPoint(842); // A4 height

        using var gfx = XGraphics.FromPdfPage(page);
        gfx.DrawRectangle(XBrushes.Cyan, 0, 0, page.Width.Point, page.Height.Point);

        var font = new XFont("Arial", 24);
        var size = gfx.MeasureString(text, font);
        gfx.DrawString(text, font, XBrushes.Black,
            (page.Width.Point - size.Width) / 2,
            (page.Height.Point - size.Height) / 2);
    }

    private static Rectangle ToRectangle(PdfSharp.Pdf.PdfRectangle rect)
    {
        int x = (int)rect.X1;
        int y = (int)rect.Y1;
        int w = (int)(rect.X2 - rect.X1);
        int h = (int)(rect.Y2 - rect.Y1);
        return new Rectangle(x, y, Math.Abs(w), Math.Abs(h));
    }
}
