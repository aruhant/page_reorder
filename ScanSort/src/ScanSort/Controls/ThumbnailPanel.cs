using System.Drawing;
using Manina.Windows.Forms;
using ScanSort.Models;
using ScanSort.Services;

namespace ScanSort.Controls;

/// <summary>
/// Reusable UserControl: panel header + ImageListView thumbnails + PictureBox preview.
/// Styled to match the prototype-light.html design language.
/// </summary>
public class ThumbnailPanel : UserControl
{
    // Design tokens
    private static readonly Color BgPrimary = Color.FromArgb(245, 246, 248);
    private static readonly Color BgSecondary = Color.White;
    private static readonly Color BgTertiary = Color.FromArgb(238, 240, 244);
    private static readonly Color BorderColor = Color.FromArgb(212, 216, 224);
    private static readonly Color TextSecondary = Color.FromArgb(74, 80, 96);
    private static readonly Color Accent = Color.FromArgb(74, 114, 232);
    private static readonly Color AccentDim = Color.FromArgb(25, 74, 114, 232);

    public ImageListView ListView { get; }
    public PictureBox Preview { get; }
    public SplitContainer Splitter { get; }
    public ContextMenuStrip ListContextMenu { get; }
    public Label HeaderLabel { get; }
    public Label CountBadge { get; }

    private readonly IImageService _imageService;
    private PageMetadataMap? _metadataMap;
    private readonly Panel _headerPanel;

    public event EventHandler<ImageListViewItem?>? ItemHovered;
    public event EventHandler? SelectionChanged;
    public event EventHandler<DropCompleteEventArgs>? DropComplete;

    public ThumbnailPanel(IImageService imageService)
    {
        _imageService = imageService;
        DoubleBuffered = true;
        BackColor = BgPrimary;

        // Panel header
        _headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 34,
            BackColor = BgTertiary,
            Padding = new Padding(12, 0, 12, 0)
        };
        _headerPanel.Paint += (s, e) =>
        {
            using var pen = new Pen(BorderColor);
            e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);
        };

        HeaderLabel = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            ForeColor = TextSecondary,
            Text = "PAGES",
            Location = new Point(12, 10)
        };

        CountBadge = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
            ForeColor = Accent,
            BackColor = AccentDim,
            Text = "0",
            Padding = new Padding(6, 1, 6, 1),
            Location = new Point(80, 9)
        };

        _headerPanel.Controls.Add(HeaderLabel);
        _headerPanel.Controls.Add(CountBadge);

        // Splitter
        Splitter = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            BorderStyle = BorderStyle.None
        };

        // Preview
        Preview = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.None,
            BackColor = BgSecondary,
            Padding = new Padding(8)
        };

        // List
        ListView = new ImageListView
        {
            Dock = DockStyle.Fill,
            AllowDrag = true,
            AllowDrop = true,
            AllowDuplicateFileNames = true,
            BorderStyle = BorderStyle.None,
            BackColor = BgPrimary,
            Font = new Font("Segoe UI", 7.5f),
            ThumbnailSize = new Size(140, 140)
        };

        // Apply prototype color scheme
        ListView.Colors.BackColor = BgPrimary;
        ListView.Colors.SelectedColor1 = AccentDim;
        ListView.Colors.SelectedColor2 = AccentDim;
        ListView.Colors.HoverColor1 = Color.FromArgb(15, 0, 0, 0);
        ListView.Colors.HoverColor2 = Color.FromArgb(8, 0, 0, 0);
        ListView.Colors.SelectedBorderColor = Accent;
        ListView.Colors.HoverBorderColor = Color.FromArgb(80, Accent);
        ListView.Colors.BorderColor = Color.Transparent;

        ListContextMenu = new ContextMenuStrip();
        ListView.ContextMenuStrip = ListContextMenu;
        Preview.ContextMenuStrip = ListContextMenu;

        ListView.ItemHover += (s, e) => ItemHovered?.Invoke(this, e.Item);
        ListView.SelectionChanged += (s, e) =>
        {
            SelectionChanged?.Invoke(this, e);
            UpdateCountBadge();
        };
        ListView.DropComplete += (s, e) =>
        {
            DropComplete?.Invoke(this, e);
            UpdateCountBadge();
        };

        Splitter.Panel1.Controls.Add(Preview);
        Splitter.Panel2.Controls.Add(ListView);

        Controls.Add(Splitter);
        Controls.Add(_headerPanel); // header on top (Dock.Top)
    }

    public void SetTitle(string title)
    {
        HeaderLabel.Text = title.ToUpperInvariant();
    }

    public void SetMetadataMap(PageMetadataMap metadataMap)
    {
        _metadataMap = metadataMap;
        ListView.SetRenderer(new ThumbnailRenderer(metadataMap, _imageService));
        UpdateCountBadge();
    }

    public void UpdatePreview(ImageListViewItem? item)
    {
        if (item == null || _metadataMap == null) return;

        var metadata = _metadataMap[item.Text];
        if (metadata == null || metadata.PageType == PageType.MissingContent) return;

        try
        {
            string path = Path.Combine(item.FilePath, item.FileName);
            var oldImage = Preview.Image;
            using var original = Image.FromFile(path);
            Preview.Image = _imageService.RotateImage(original, metadata.Orientation, metadata.Rotate);
            Preview.Tag = item;
            oldImage?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating preview: {ex.Message}");
        }
    }

    public void ClearPreview()
    {
        var oldImage = Preview.Image;
        Preview.Image = null;
        Preview.Tag = null;
        oldImage?.Dispose();
    }

    public void CollapsePreview(bool collapsed)
    {
        Splitter.Panel1Collapsed = collapsed;
    }

    public IReadOnlyList<string> GetOrderedTitles()
    {
        var titles = new List<string>();
        foreach (ImageListViewItem item in ListView.Items)
            titles.Add(item.Text);
        return titles;
    }

    public IReadOnlyList<ImageListViewItem> GetSelectedItems()
    {
        var items = new List<ImageListViewItem>();
        foreach (ImageListViewItem item in ListView.SelectedItems)
            items.Add(item);
        return items;
    }

    public void RefreshCountBadge()
    {
        UpdateCountBadge();
    }

    private void UpdateCountBadge()
    {
        int count = ListView.Items.Count;
        CountBadge.Text = count.ToString();
        CountBadge.Left = HeaderLabel.Right + 8;
    }
}
