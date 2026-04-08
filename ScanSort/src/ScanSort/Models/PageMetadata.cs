using System.Drawing;

namespace ScanSort.Models;

public class PageMetadata
{
    private float _rotate;
    private string _comment = string.Empty;
    private string _title = string.Empty;
    private Rectangle _clipRect;
    private Rectangle _mediaRect;

    public string FileName { get; }

    public string Title
    {
        get => _title;
        set => _title = value ?? string.Empty;
    }

    public string Comment
    {
        get => _comment;
        set => _comment = value ?? string.Empty;
    }

    /// <summary>
    /// Fine rotation in degrees (deskew correction). Normalized to 0-360.
    /// </summary>
    public float Rotate
    {
        get => _rotate;
        set => _rotate = NormalizeAngle(value);
    }

    /// <summary>
    /// Coarse rotation in 90-degree increments (0, 90, 180, 270).
    /// </summary>
    public int Orientation { get; set; }

    public PageType PageType { get; set; }
    public int PageNumber { get; set; }
    public int OriginalPageNumber { get; set; }
    public int Blurred { get; set; }

    public Rectangle ClipRect
    {
        get => _clipRect;
        set => _clipRect = ValidateRect(value);
    }

    public Rectangle MediaRect
    {
        get => _mediaRect;
        set => _mediaRect = ValidateRect(value);
    }

    #region Computed Properties

    public string Flags
    {
        get
        {
            var parts = new List<string>();
            if (Blurred != 0) parts.Add("Blurred");
            if (!string.IsNullOrWhiteSpace(Comment)) parts.Add(Comment);
            return string.Join(" ", parts);
        }
    }

    /// <summary>
    /// Page dimensions from ClipRect (or MediaRect if ClipRect is empty), without rotation.
    /// </summary>
    public SizeF PageSizeWithoutRotation
    {
        get
        {
            float w = ClipRect.Width == 0 ? MediaRect.Width : ClipRect.Width;
            float h = ClipRect.Height == 0 ? MediaRect.Height : ClipRect.Height;
            return new SizeF(w, h);
        }
    }

    /// <summary>
    /// Page dimensions adjusted for the Rotate angle's bounding box expansion.
    /// </summary>
    public SizeF PageSizeWithRotation
    {
        get
        {
            var size = PageSizeWithoutRotation;
            if (Math.Abs(Rotate) < 0.01f)
                return size;

            double rad = Rotate * Math.PI / 180.0;
            double cos = Math.Abs(Math.Cos(rad));
            double sin = Math.Abs(Math.Sin(rad));
            float w = (float)(cos * size.Width + sin * size.Height);
            float h = (float)(cos * size.Height + sin * size.Width);
            return new SizeF(w, h);
        }
    }

    #endregion

    public PageMetadata(string fileName, string title = "", PageType pageType = PageType.Content,
        int pageNumber = -1, int originalPageNumber = -1)
    {
        if (string.IsNullOrWhiteSpace(fileName) && pageType != PageType.MissingContent)
            throw new ArgumentException("FileName cannot be empty for non-missing pages.", nameof(fileName));

        FileName = fileName ?? string.Empty;
        Title = title;
        PageType = pageType;
        PageNumber = pageNumber;
        OriginalPageNumber = originalPageNumber;
    }

    public PageMetadata Clone()
    {
        return new PageMetadata(FileName, Title, PageType, PageNumber, OriginalPageNumber)
        {
            Comment = Comment,
            Rotate = Rotate,
            Orientation = Orientation,
            ClipRect = ClipRect,
            MediaRect = MediaRect,
            Blurred = Blurred
        };
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(FileName)
            && MediaRect.Width >= 0 && MediaRect.Height >= 0
            && ClipRect.Width >= 0 && ClipRect.Height >= 0;
    }

    public override string ToString()
    {
        return $"FileName: {FileName}, Title: {Title}, Comment: {Comment}, " +
               $"Rotate: {Rotate}, Orientation: {Orientation}, PageNumber: {PageNumber}, " +
               $"OriginalPageNumber: {OriginalPageNumber}, ClipRect: {ClipRect}, MediaRect: {MediaRect}";
    }

    public override bool Equals(object? obj)
    {
        return obj is PageMetadata other
            && string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase)
            && PageNumber == other.PageNumber;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName?.ToLowerInvariant(), PageNumber);
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    private static Rectangle ValidateRect(Rectangle rect)
    {
        if (rect.Width < 0 || rect.Height < 0)
            return Rectangle.Empty;
        return rect;
    }
}
