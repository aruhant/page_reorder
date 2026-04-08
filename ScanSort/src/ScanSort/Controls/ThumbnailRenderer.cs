using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Manina.Windows.Forms;
using ScanSort.Models;
using ScanSort.Services;

namespace ScanSort.Controls;

/// <summary>
/// Modern flat thumbnail renderer matching the prototype-light.html design.
/// Card-based layout with subtle borders, inset image well, metadata badges,
/// and clean selection/hover states.
/// </summary>
public class ThumbnailRenderer : ImageListView.ImageListViewRenderer
{
    private readonly PageMetadataMap _metadataMap;
    private readonly IImageService _imageService;

    // Design tokens (matching prototype CSS variables)
    private static readonly Color BgPrimary = Color.FromArgb(245, 246, 248);     // --bg-primary
    private static readonly Color BgSecondary = Color.FromArgb(255, 255, 255);   // --bg-secondary (card)
    private static readonly Color BgTertiary = Color.FromArgb(238, 240, 244);    // --bg-tertiary (image well)
    private static readonly Color BgHover = Color.FromArgb(232, 234, 239);       // --bg-hover
    private static readonly Color Border = Color.FromArgb(212, 216, 224);        // --border
    private static readonly Color BorderLight = Color.FromArgb(192, 197, 208);   // --border-light
    private static readonly Color TextMuted = Color.FromArgb(136, 144, 160);     // --text-muted
    private static readonly Color TextSecondary = Color.FromArgb(74, 80, 96);    // --text-secondary
    private static readonly Color Accent = Color.FromArgb(74, 114, 232);         // --accent
    private static readonly Color AccentDim = Color.FromArgb(25, 74, 114, 232);  // --accent-dim
    private static readonly Color AccentBorder = Color.FromArgb(90, 74, 114, 232); // --accent-border
    private static readonly Color Danger = Color.FromArgb(224, 82, 82);          // --danger
    private static readonly Color Warning = Color.FromArgb(208, 138, 32);        // --warning
    private static readonly Color Success = Color.FromArgb(46, 169, 94);         // --success

    private static readonly Font LabelFont = new("Segoe UI", 8f, FontStyle.Regular);
    private static readonly Font BadgeFont = new("Segoe UI", 6.5f, FontStyle.Bold);
    private static readonly Font MissingFont = new("Segoe UI", 20f, FontStyle.Regular);

    private const int CardPadding = 6;     // space around card content
    private const int CardGap = 3;         // gap between cards (half on each side)
    private const int ImageInset = 1;      // border around image well
    private const int LabelHeight = 18;    // page number label area
    private const int Radius = 6;          // card corner radius
    private const int RadiusSm = 4;        // image well corner radius

    public ThumbnailRenderer(PageMetadataMap metadataMap, IImageService imageService)
    {
        _metadataMap = metadataMap;
        _imageService = imageService;
    }

    public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
    {
        if (ImageListView.View == Manina.Windows.Forms.View.Details)
        {
            base.DrawItem(g, item, state, bounds);
            return;
        }

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

        var metadata = _metadataMap[item.Text];
        if (metadata == null) { base.DrawItem(g, item, state, bounds); return; }

        // Card rect (inset from bounds for gap)
        var card = Rectangle.Inflate(bounds, -CardGap, -CardGap);

        // 1. Draw card
        DrawCard(g, card, state, metadata);

        // 2. Image well
        var imageWell = new Rectangle(
            card.X + CardPadding,
            card.Y + CardPadding,
            card.Width - CardPadding * 2,
            card.Height - CardPadding * 2 - LabelHeight);

        DrawImageWell(g, item, metadata, imageWell);

        // 3. Badges
        DrawBadges(g, metadata, imageWell);

        // 4. Page label
        var labelRect = new Rectangle(card.X, card.Bottom - LabelHeight, card.Width, LabelHeight);
        DrawLabel(g, metadata, state, labelRect);
    }

    private static void DrawCard(Graphics g, Rectangle card, ItemState state, PageMetadata metadata)
    {
        bool isSelected = (state & ItemState.Selected) != ItemState.None;
        bool isHovered = (state & ItemState.Hovered) != ItemState.None;
        bool isMissing = metadata.PageType == PageType.MissingContent;

        // Card fill
        using (var path = RoundedRect(card, Radius))
        {
            Color fill = isSelected ? AccentDim : (isHovered ? BgTertiary : BgSecondary);
            using var brush = new SolidBrush(fill);
            g.FillPath(brush, path);
        }

        // Card border
        using (var path = RoundedRect(card, Radius))
        {
            Color borderColor;
            float borderWidth;
            DashStyle dash = DashStyle.Solid;

            if (isSelected)
            {
                borderColor = Accent;
                borderWidth = 2f;
            }
            else if (isMissing)
            {
                borderColor = Warning;
                borderWidth = 1.5f;
                dash = DashStyle.Dash;
            }
            else if (isHovered)
            {
                borderColor = BorderLight;
                borderWidth = 1f;
            }
            else
            {
                borderColor = Border;
                borderWidth = 1f;
            }

            using var pen = new Pen(borderColor, borderWidth) { DashStyle = dash };
            g.DrawPath(pen, path);
        }

        // Subtle shadow (only for non-selected to keep it light)
        if (!isSelected && !isHovered)
        {
            using var shadowPath = RoundedRect(new Rectangle(card.X, card.Y + 1, card.Width, card.Height), Radius);
            using var shadowPen = new Pen(Color.FromArgb(15, 0, 0, 0), 1f);
            g.DrawPath(shadowPen, shadowPath);
        }
    }

    private void DrawImageWell(Graphics g, ImageListViewItem item, PageMetadata metadata, Rectangle well)
    {
        // Image well background + border
        using (var wellPath = RoundedRect(well, RadiusSm))
        {
            using var wellBrush = new SolidBrush(BgPrimary);
            g.FillPath(wellBrush, wellPath);
            using var wellPen = new Pen(Border, ImageInset);
            g.DrawPath(wellPen, wellPath);
        }

        if (metadata.PageType == PageType.MissingContent)
        {
            // Missing page: show warning icon
            using var brush = new SolidBrush(Color.FromArgb(100, Warning));
            var text = "\u2753"; // question mark
            var size = g.MeasureString(text, MissingFont);
            g.DrawString(text, MissingFont, brush,
                well.X + (well.Width - size.Width) / 2,
                well.Y + (well.Height - size.Height) / 2);
            return;
        }

        // Get thumbnail
        Image? img = item.GetCachedImage(CachedImageType.Thumbnail);
        if (img == null) return;

        Image? rotated = null;
        try
        {
            float totalRotation = metadata.Rotate + metadata.Orientation;
            if (Math.Abs(totalRotation) > 0.01f)
            {
                rotated = _imageService.RotateImage(img, metadata.Orientation, metadata.Rotate);
                img = rotated;
            }

            // Fit image inside well with padding
            var innerWell = Rectangle.Inflate(well, -3, -3);
            var dest = FitImage(img.Size, innerWell);

            // Clip to rounded rect
            using var clipPath = RoundedRect(dest, 2);
            var oldClip = g.Clip;
            g.SetClip(clipPath, CombineMode.Intersect);
            g.DrawImage(img, dest);
            g.Clip = oldClip;
        }
        finally
        {
            rotated?.Dispose();
        }
    }

    private static void DrawBadges(Graphics g, PageMetadata metadata, Rectangle imageWell)
    {
        int x = imageWell.Right - 4;
        int y = imageWell.Top + 4;

        if (metadata.Blurred != 0)
        {
            x = DrawTextBadge(g, "BLUR", Danger, x, y);
        }

        if (!string.IsNullOrWhiteSpace(metadata.Comment))
        {
            if (metadata.Blurred != 0) y += 14;
            DrawTextBadge(g, "NOTE", Success, metadata.Blurred != 0 ? imageWell.Right - 4 : x, y);
        }
    }

    /// <summary>Draws a small pill-shaped badge, returns the left edge X for stacking.</summary>
    private static int DrawTextBadge(Graphics g, string text, Color bg, int rightX, int y)
    {
        var size = g.MeasureString(text, BadgeFont);
        int w = (int)size.Width + 6;
        int h = 12;
        int x = rightX - w;

        using var path = RoundedRect(new Rectangle(x, y, w, h), 3);
        using var brush = new SolidBrush(bg);
        g.FillPath(brush, path);

        using var textBrush = new SolidBrush(Color.White);
        g.DrawString(text, BadgeFont, textBrush, x + 3, y + (h - size.Height) / 2);

        return x - 3; // return for horizontal stacking
    }

    private static void DrawLabel(Graphics g, PageMetadata metadata, ItemState state, Rectangle labelRect)
    {
        bool isSelected = (state & ItemState.Selected) != ItemState.None;
        Color textColor = isSelected ? Accent : TextMuted;
        FontStyle style = isSelected ? FontStyle.Bold : FontStyle.Regular;

        string text = metadata.PageNumber >= 0 ? metadata.PageNumber.ToString() : "-";

        using var font = new Font("Segoe UI", 8f, style);
        TextRenderer.DrawText(g, text, font, labelRect, textColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
    }

    #region Helpers

    private static Rectangle FitImage(Size imageSize, Rectangle area)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0) return area;

        float ratio = Math.Min(
            (float)area.Width / imageSize.Width,
            (float)area.Height / imageSize.Height);

        int w = (int)(imageSize.Width * ratio);
        int h = (int)(imageSize.Height * ratio);

        return new Rectangle(
            area.X + (area.Width - w) / 2,
            area.Y + (area.Height - h) / 2,
            w, h);
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        if (d <= 0) { path.AddRectangle(rect); return path; }
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    #endregion
}
