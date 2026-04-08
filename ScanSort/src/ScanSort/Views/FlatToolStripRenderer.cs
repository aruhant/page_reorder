using System.Drawing;
using System.Drawing.Drawing2D;

namespace ScanSort.Views;

/// <summary>
/// Clean flat renderer for ToolStrip matching the prototype's button styling.
/// Transparent buttons, subtle hover, accent highlight on checked state.
/// </summary>
public class FlatToolStripRenderer : ToolStripProfessionalRenderer
{
    private static readonly Color BgHover = Color.FromArgb(232, 234, 239);
    private static readonly Color BgActive = Color.FromArgb(223, 226, 232);
    private static readonly Color Accent = Color.FromArgb(74, 114, 232);
    private static readonly Color AccentDim = Color.FromArgb(25, 74, 114, 232);
    private static readonly Color AccentBorder = Color.FromArgb(90, 74, 114, 232);
    private static readonly Color Border = Color.FromArgb(212, 216, 224);
    private const int Radius = 4;

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        e.Graphics.Clear(Color.White);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var bounds = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);

        if (e.Item is ToolStripButton btn && btn.Checked)
        {
            // Checked state: accent background + border
            using var path = RoundedRect(bounds, Radius);
            using var fill = new SolidBrush(AccentDim);
            g.FillPath(fill, path);
            using var pen = new Pen(AccentBorder, 1f);
            g.DrawPath(pen, path);
            btn.ForeColor = Accent;
        }
        else if (e.Item.Pressed)
        {
            using var path = RoundedRect(bounds, Radius);
            using var fill = new SolidBrush(BgActive);
            g.FillPath(fill, path);
        }
        else if (e.Item.Selected)
        {
            // Hover
            using var path = RoundedRect(bounds, Radius);
            using var fill = new SolidBrush(BgHover);
            g.FillPath(fill, path);
            using var pen = new Pen(Border, 1f);
            g.DrawPath(pen, path);
        }
        // else: transparent - no background
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        var g = e.Graphics;
        int x = e.Item.Width / 2;
        using var pen = new Pen(Border);
        g.DrawLine(pen, x, 4, x, e.Item.Height - 4);
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        // No border — we draw our own in the Paint event
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = new Rectangle(2, 1, e.Item.Width - 4, e.Item.Height - 2);
            using var path = RoundedRect(bounds, 3);
            using var fill = new SolidBrush(AccentDim);
            g.FillPath(fill, path);
        }
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
}
