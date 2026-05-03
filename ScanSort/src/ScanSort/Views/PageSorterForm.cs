using System.Drawing;
using System.Drawing.Drawing2D;
using ScanSort.Controls;
using ScanSort.Infrastructure;
using ScanSort.Presenters;
using ScanSort.Services;

namespace ScanSort.Views;

public class PageSorterForm : Form, IPageSorterView
{
    private readonly PageSorterPresenter _presenter;

    // UI components
    public ThumbnailPanel InputPanel { get; }
    public ThumbnailPanel OutputPanel { get; }
    public SplitContainer MainSplitContainer { get; }

    private readonly MenuStrip _menuStrip;
    private readonly ToolStrip _toolStrip;
    private readonly StatusStrip _statusStrip;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly ToolStripProgressBar _progressBar;
    private readonly ToolStripButton _duplexToggle;
    private readonly ToolStripButton _coverToggle;
    private readonly ToolStripMenuItem _duplexMenuItem;
    private readonly ToolStripMenuItem _coverMenuItem;
    private readonly Debouncer _layoutDebouncer = new();

    // Events
    public event EventHandler? OpenFileRequested;
    public event EventHandler? OpenFolderRequested;
    public event EventHandler? ExportRequested;
    public event EventHandler? ExitRequested;
    public event EventHandler? RotateLeftRequested;
    public event EventHandler? RotateRightRequested;
    public event EventHandler? ToggleLayoutRequested;
    public event EventHandler? DuplexToggled;
    public event EventHandler? CoverToggled;
    public event EventHandler? AutoDeskewRequested;
    public event EventHandler? FullScreenRequested;
    public event EventHandler? UndoRequested;
    public event EventHandler? RedoRequested;
    public event EventHandler<ContextMenuAction>? ContextMenuClicked;
    public event EventHandler<string>? LayoutModeChanged;
    public event EventHandler? SplitterMoved;
    public event EventHandler? FormResized;

    // Properties
    public new string Title { set => Text = value; }

    public string StatusText
    {
        set => InvokeOnUI(() => _statusLabel.Text = value);
    }

    public bool DuplexChecked
    {
        get => _duplexToggle.Checked;
        set => InvokeOnUI(() =>
        {
            _duplexToggle.Checked = value;
            _duplexMenuItem.Checked = value;
        });
    }

    public bool CoverChecked
    {
        get => _coverToggle.Checked;
        set => InvokeOnUI(() =>
        {
            _coverToggle.Checked = value;
            _coverMenuItem.Checked = !value;
        });
    }

    public PageSorterForm(PageSorterPresenter presenter, IImageService imageService)
    {
        _presenter = presenter;

        this.Text = "ScanSort";
        this.Size = new Size(1200, 700);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.KeyPreview = true;

        InputPanel = new ThumbnailPanel(imageService) { Dock = DockStyle.Fill };
        InputPanel.SetTitle("Source Pages");
        OutputPanel = new ThumbnailPanel(imageService) { Dock = DockStyle.Fill };
        OutputPanel.SetTitle("Output Pages");

        MainSplitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical
        };
        MainSplitContainer.Panel1.Controls.Add(InputPanel);
        MainSplitContainer.Panel2.Controls.Add(OutputPanel);
        MainSplitContainer.SplitterMoved += (s, e) => DebounceSplitterSave();
        InputPanel.Splitter.SplitterMoved += (s, e) => DebounceSplitterSave();
        OutputPanel.Splitter.SplitterMoved += (s, e) => DebounceSplitterSave();

        _menuStrip = CreateMenuStrip(out _duplexMenuItem, out _coverMenuItem);
        _menuStrip.BackColor = Color.White;
        _menuStrip.ForeColor = Color.FromArgb(74, 80, 96);
        _menuStrip.Font = new Font("Segoe UI", 8.5f);
        _menuStrip.Renderer = new FlatToolStripRenderer();
        _menuStrip.Padding = new Padding(4, 0, 0, 0);

        _toolStrip = CreateToolStrip(out _duplexToggle, out _coverToggle);

        _statusStrip = new StatusStrip
        {
            BackColor = Color.White,
            SizingGrip = false
        };
        _statusStrip.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(212, 216, 224));
            e.Graphics.DrawLine(pen, 0, 0, _statusStrip.Width, 0);
        };

        // Green dot + status label
        var dotLabel = new ToolStripStatusLabel("\u25CF")
        {
            ForeColor = Color.FromArgb(46, 169, 94),
            Font = new Font("Segoe UI", 6f),
            Padding = new Padding(8, 0, 0, 0)
        };
        _statusLabel = new ToolStripStatusLabel("Ready")
        {
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.FromArgb(136, 144, 160),
            Font = new Font("Segoe UI", 8f)
        };
        _progressBar = new ToolStripProgressBar { Visible = false, Width = 200 };
        _statusStrip.Items.Add(dotLabel);
        _statusStrip.Items.Add(_statusLabel);
        _statusStrip.Items.Add(_progressBar);

        SetupContextMenus(InputPanel, isOutput: false);
        SetupContextMenus(OutputPanel, isOutput: true);

        var container = new ToolStripContainer { Dock = DockStyle.Fill };
        container.TopToolStripPanel.Controls.Add(_toolStrip);
        container.TopToolStripPanel.Controls.Add(_menuStrip);
        container.ContentPanel.Controls.Add(MainSplitContainer);
        container.BottomToolStripPanel.Controls.Add(_statusStrip);
        container.ContentPanel.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
                ActiveControl = null;
        };
        MainSplitContainer.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
                ActiveControl = null;
        };
        MainSplitContainer.Panel1.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
                ActiveControl = null;
        };
        MainSplitContainer.Panel2.MouseDown += (s, e) =>
        {
            if (e.Button == MouseButtons.Left)
                ActiveControl = null;
        };
        Controls.Add(container);

        this.Resize += (s, e) =>
        {
            if (WindowState != FormWindowState.Minimized)
                FormResized?.Invoke(this, EventArgs.Empty);
        };
        this.Shown += (s, e) => _presenter.Initialize(this);
    }

    #region Menu

    private MenuStrip CreateMenuStrip(out ToolStripMenuItem duplexItem, out ToolStripMenuItem coverItem)
    {
        var menu = new MenuStrip();

        var fileMenu = new ToolStripMenuItem("&File");
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Open File", null, (s, e) => OpenFileRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Control | Keys.O });
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("Open Fol&der", null, (s, e) => OpenFolderRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Control | Keys.D });
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Save", null, (s, e) => ExportRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Control | Keys.S });
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("E&xit", null, (s, e) => ExitRequested?.Invoke(this, e)));

        var editMenu = new ToolStripMenuItem("&Edit");
        editMenu.DropDownItems.Add(new ToolStripMenuItem("&Undo", null, (s, e) => UndoRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Control | Keys.Z });
        editMenu.DropDownItems.Add(new ToolStripMenuItem("&Redo", null, (s, e) => RedoRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Control | Keys.Y });
        editMenu.DropDownItems.Add(new ToolStripSeparator());
        editMenu.DropDownItems.Add(new ToolStripMenuItem("Rotate &Left (-1\u00B0)", null, (s, e) => RotateLeftRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Alt | Keys.Left });
        editMenu.DropDownItems.Add(new ToolStripMenuItem("Rotate &Right (+1\u00B0)", null, (s, e) => RotateRightRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Alt | Keys.Right });
        editMenu.DropDownItems.Add(new ToolStripMenuItem("Rotate &90\u00B0", null, (s, e) => ToggleLayoutRequested?.Invoke(this, e))
            { ShortcutKeys = Keys.Alt | Keys.N });
        editMenu.DropDownItems.Add(new ToolStripSeparator());
        editMenu.DropDownItems.Add(new ToolStripMenuItem("Auto &Deskew", null, (s, e) => AutoDeskewRequested?.Invoke(this, e)));

        var selectionMenu = new ToolStripMenuItem("&Selection");
        duplexItem = new ToolStripMenuItem("&Duplex Mode") { CheckOnClick = true };
        duplexItem.Click += (s, e) => DuplexToggled?.Invoke(this, e);
        coverItem = new ToolStripMenuItem("&Missing Cover (no cover page)") { CheckOnClick = true };
        coverItem.Click += (s, e) => CoverToggled?.Invoke(this, e);
        selectionMenu.DropDownItems.Add(duplexItem);
        selectionMenu.DropDownItems.Add(coverItem);

        var viewMenu = new ToolStripMenuItem("&View");
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Horizontal", null, (s, e) => LayoutModeChanged?.Invoke(this, "Horizontal")));
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("&Vertical", null, (s, e) => LayoutModeChanged?.Invoke(this, "Vertical")));
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("Horizontal &Thumbs", null, (s, e) => LayoutModeChanged?.Invoke(this, "HorizontalThumbs")));
        viewMenu.DropDownItems.Add(new ToolStripMenuItem("Vertical T&humbs", null, (s, e) => LayoutModeChanged?.Invoke(this, "VerticalThumbs")));

        menu.Items.AddRange(new ToolStripItem[] { fileMenu, editMenu, viewMenu, selectionMenu });
        return menu;
    }

    #endregion

    #region Toolbar

    // Prototype design tokens
    private static readonly Color TbBg = Color.White;
    private static readonly Color TbBorder = Color.FromArgb(212, 216, 224);
    private static readonly Color TbTextSecondary = Color.FromArgb(74, 80, 96);
    private static readonly Color TbTextMuted = Color.FromArgb(136, 144, 160);
    private static readonly Color TbAccent = Color.FromArgb(74, 114, 232);

    // Segoe MDL2 Assets glyph codepoints (works on Win10+)
    private const string IcoOpenFile   = "\uE8E5";
    private const string IcoFolder     = "\uE838";
    private const string IcoSave       = "\uE74E";
    private const string IcoUndo       = "\uE7A7";
    private const string IcoRedo       = "\uE7A6";
    private const string IcoRotateL    = "\uE80D";
    private const string IcoRotateR    = "\uE80C";
    private const string IcoRotate90   = "\uE7AD";
    private const string IcoHoriz      = "\uE90C";
    private const string IcoVert       = "\uE90E";
    private const string IcoGrid       = "\uF0E2";
    private const string IcoDuplex     = "\uE89A";
    private const string IcoCover      = "\uE7BC";
    private const string IcoDeskew     = "\uE90F";
    private const string IcoFullScreen = "\uE740";

    /// <summary>Render an MDL2 glyph to a bitmap for use as a ToolStripButton image.</summary>
    private static Image GlyphIcon(string glyph, Color color, int size = 20)
    {
        var bmp = new Bitmap(size, size);
        using var g = Graphics.FromImage(bmp);
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        using var font = new Font("Segoe MDL2 Assets", size * 0.65f, FontStyle.Regular, GraphicsUnit.Pixel);
        using var brush = new SolidBrush(color);
        var sz = g.MeasureString(glyph, font);
        g.DrawString(glyph, font, brush, (size - sz.Width) / 2, (size - sz.Height) / 2);
        return bmp;
    }

    private ToolStrip CreateToolStrip(out ToolStripButton duplexBtn, out ToolStripButton coverBtn)
    {
        var strip = new ToolStrip
        {
            GripStyle = ToolStripGripStyle.Hidden,
            BackColor = TbBg,
            ImageScalingSize = new Size(20, 20),
            Padding = new Padding(6, 4, 6, 4),
            Renderer = new FlatToolStripRenderer()
        };
        strip.Paint += (s, e) =>
        {
            using var pen = new Pen(TbBorder);
            e.Graphics.DrawLine(pen, 0, strip.Height - 1, strip.Width, strip.Height - 1);
        };

        // File operations
        strip.Items.Add(Tb("Open", IcoOpenFile, (s, e) => OpenFileRequested?.Invoke(this, e), "Open PDF file (Ctrl+O)"));
        strip.Items.Add(Tb("Folder", IcoFolder, (s, e) => OpenFolderRequested?.Invoke(this, e), "Open image folder (Ctrl+D)"));
        strip.Items.Add(Tb("Export", IcoSave, (s, e) => ExportRequested?.Invoke(this, e), "Export PDF (Ctrl+S)"));
        strip.Items.Add(Sep());

        // Layout
        strip.Items.Add(TbLabel("VIEW"));
        strip.Items.Add(Tb("Horiz", IcoHoriz, (s, e) => LayoutModeChanged?.Invoke(this, "Horizontal"), "Horizontal layout + preview"));
        strip.Items.Add(Tb("Vert", IcoVert, (s, e) => LayoutModeChanged?.Invoke(this, "Vertical"), "Vertical layout + preview"));
        strip.Items.Add(Tb("H-Grid", IcoGrid, (s, e) => LayoutModeChanged?.Invoke(this, "HorizontalThumbs"), "Horizontal thumbnails"));
        strip.Items.Add(Tb("V-Grid", IcoGrid, (s, e) => LayoutModeChanged?.Invoke(this, "VerticalThumbs"), "Vertical thumbnails"));
        strip.Items.Add(Sep());

        // Rotation
        strip.Items.Add(TbLabel("ROTATE"));
        strip.Items.Add(Tb("-1\u00B0", IcoRotateL, (s, e) => RotateLeftRequested?.Invoke(this, e), "Rotate left -1\u00B0 (Alt+Left)"));
        strip.Items.Add(Tb("+1\u00B0", IcoRotateR, (s, e) => RotateRightRequested?.Invoke(this, e), "Rotate right +1\u00B0 (Alt+Right)"));
        strip.Items.Add(Tb("90\u00B0", IcoRotate90, (s, e) => ToggleLayoutRequested?.Invoke(this, e), "Rotate 90\u00B0 (Alt+N)"));
        strip.Items.Add(Sep());

        // Mode toggles
        duplexBtn = new ToolStripButton("Duplex", GlyphIcon(IcoDuplex, TbTextSecondary))
        {
            CheckOnClick = true,
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ToolTipText = "Toggle duplex selection",
            ForeColor = TbTextSecondary,
            Font = new Font("Segoe UI", 8.5f),
            Margin = new Padding(2, 0, 2, 0)
        };
        duplexBtn.Click += (s, e) => DuplexToggled?.Invoke(this, e);
        strip.Items.Add(duplexBtn);

        coverBtn = new ToolStripButton("Cover", GlyphIcon(IcoCover, TbTextSecondary))
        {
            CheckOnClick = true,
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            ToolTipText = "Toggle cover page mode",
            ForeColor = TbTextSecondary,
            Font = new Font("Segoe UI", 8.5f),
            Margin = new Padding(2, 0, 2, 0)
        };
        coverBtn.Click += (s, e) => CoverToggled?.Invoke(this, e);
        strip.Items.Add(coverBtn);
        strip.Items.Add(Sep());

        // Deskew
        strip.Items.Add(Tb("Deskew", IcoDeskew, (s, e) => AutoDeskewRequested?.Invoke(this, e), "Auto-deskew all pages"));
        strip.Items.Add(Tb("Full", IcoFullScreen, (s, e) => FullScreenRequested?.Invoke(this, e), "Toggle full screen preview"));

        return strip;
    }

    private static ToolStripButton Tb(string text, string glyph, EventHandler handler, string tooltip)
    {
        var btn = new ToolStripButton(text, GlyphIcon(glyph, TbTextSecondary))
        {
            ToolTipText = tooltip,
            ForeColor = TbTextSecondary,
            Font = new Font("Segoe UI", 8.5f),
            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
            TextImageRelation = TextImageRelation.ImageBeforeText,
            AutoSize = true,
            Padding = new Padding(5, 3, 5, 3),
            Margin = new Padding(2, 0, 2, 0)
        };
        btn.Click += handler;
        return btn;
    }

    private static ToolStripLabel TbLabel(string text) => new(text)
    {
        ForeColor = TbTextMuted,
        Font = new Font("Segoe UI", 7f, FontStyle.Bold),
        Padding = new Padding(4, 0, 4, 0)
    };

    private static ToolStripSeparator Sep() => new();

    #endregion

    #region Context Menus

    private void SetupContextMenus(ThumbnailPanel panel, bool isOutput)
    {
        var menu = panel.ListContextMenu;

        menu.Items.Add(new ToolStripMenuItem("&Blurred Image", null, (s, e) =>
            ContextMenuClicked?.Invoke(this, new ContextMenuAction(panel, "blur"))));
        menu.Items.Add(new ToolStripMenuItem("&Comment...", null, (s, e) =>
            ContextMenuClicked?.Invoke(this, new ContextMenuAction(panel, "comment"))));
        menu.Items.Add(new ToolStripMenuItem("&Rotate 90\u00B0", null, (s, e) =>
            ContextMenuClicked?.Invoke(this, new ContextMenuAction(panel, "rotate90"))));
        menu.Items.Add(new ToolStripMenuItem("&Full Screen", null, (s, e) =>
            ContextMenuClicked?.Invoke(this, new ContextMenuAction(panel, "fullscreen"))));

        if (isOutput)
        {
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add(new ToolStripMenuItem("&Missing Page", null, (s, e) =>
                ContextMenuClicked?.Invoke(this, new ContextMenuAction(panel, "missing"))));
        }
    }

    #endregion

    #region Layout

    public void SetLayoutMode(string mode, Orientation mainOrientation, Orientation subOrientation,
        Manina.Windows.Forms.View listViewMode, bool showPreviews)
    {
        if (IsDisposed || MainSplitContainer.IsDisposed || InputPanel.IsDisposed || OutputPanel.IsDisposed)
            return;

        if (InvokeRequired)
        {
            if (IsHandleCreated)
                BeginInvoke(() => SetLayoutMode(mode, mainOrientation, subOrientation, listViewMode, showPreviews));
            return;
        }

        try
        {
            SuspendLayout();
            MainSplitContainer.SuspendLayout();

            ApplyOrientationSafe(MainSplitContainer, mainOrientation);
            ApplyOrientationSafe(InputPanel.Splitter, subOrientation);
            ApplyOrientationSafe(OutputPanel.Splitter, subOrientation);

            InputPanel.CollapsePreview(!showPreviews);
            OutputPanel.CollapsePreview(!showPreviews);

            InputPanel.ListView.View = listViewMode;
            OutputPanel.ListView.View = listViewMode;
        }
        catch (InvalidOperationException)
        {
            // Ignore transient layout exceptions (e.g., during resize/dispose).
        }
        finally
        {
            try { MainSplitContainer.ResumeLayout(); } catch { }
            try { ResumeLayout(); } catch { }
            try { PerformLayout(); } catch { }
        }
    }

    public void SetSplitterRatio(string containerName, double ratio)
    {
        if (ratio < 0 || ratio > 1) return;
        // Defer to after current layout pass completes
        BeginInvoke(() => SetSplitterRatioCore(containerName, ratio));
    }

    private void SetSplitterRatioCore(string containerName, double ratio)
    {
        try
        {
            var container = GetSplitterByName(containerName);
            if (container == null) return;
            if (!container.Visible || !container.IsHandleCreated) return;
            if (container.Panel1Collapsed || container.Panel2Collapsed) return;
            if (container.Width <= 0 || container.Height <= 0) return;

            int dimension = container.Orientation == Orientation.Horizontal
                ? container.Height : container.Width;
            if (dimension <= container.Panel1MinSize + container.Panel2MinSize) return;

            int maxDist = dimension - container.Panel2MinSize - 1;
            int minDist = container.Panel1MinSize + 1;
            int distance = Math.Clamp((int)(dimension * ratio), minDist, maxDist);

            container.SplitterDistance = distance;
        }
        catch (InvalidOperationException) { }
    }

    public double GetSplitterRatio(string containerName)
    {
        try
        {
            var container = GetSplitterByName(containerName);
            if (container == null || container.Panel1Collapsed || container.Panel2Collapsed) return 0.5;
            int dimension = container.Orientation == Orientation.Horizontal ? container.Height : container.Width;
            return dimension > 0 ? (double)container.SplitterDistance / dimension : 0.5;
        }
        catch { return 0.5; }
    }

    #endregion

    #region Progress

    public void ShowProgress(int current, int total, string message)
    {
        InvokeOnUI(() =>
        {
            _progressBar.Visible = true;
            _progressBar.Maximum = total;
            _progressBar.Value = Math.Min(current, total);
            _statusLabel.Text = message;
        });
    }

    public void HideProgress()
    {
        InvokeOnUI(() =>
        {
            _progressBar.Visible = false;
            _statusLabel.Text = "Ready";
        });
    }

    #endregion

    #region Helpers

    public void InvokeOnUI(Action action)
    {
        if (InvokeRequired)
            Invoke(action);
        else
            action();
    }

    public void ShowFullScreenPreview(Image image, string title)
    {
        var previewForm = new Form
        {
            Text = title,
            WindowState = FormWindowState.Maximized,
            FormBorderStyle = FormBorderStyle.None,
            BackColor = Color.Black,
            StartPosition = FormStartPosition.CenterScreen,
            KeyPreview = true
        };

        var viewer = new ZoomableImageBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            Image = image
        };

        previewForm.Controls.Add(viewer);
        previewForm.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                previewForm.Close();
            else if (viewer.HandlePreviewKey(e.KeyCode))
                e.Handled = true;
        };
        previewForm.Shown += (s, e) => viewer.Focus();
        previewForm.FormClosed += (s, e) => viewer.Image?.Dispose();
        previewForm.Show(this);
    }

    private sealed class ZoomableImageBox : Control
    {
        private Image? _image;
        private float _zoom = 1f;
        private float _targetZoom = 1f;
        private const float _minZoom = 0.9f;
        private bool _cropMode;
        private bool _cropAnchorSet;
        private bool _cropFixed;
        private bool _cropFocused;
        private PointF _cropAnchorImage;
        private PointF _cropCurrentImage;
        private PointF _cropEndImage;
        private bool _cropDragging;
        private Point _cropDragStartScreen;
        private PointF _offset = new(0, 0);
        private PointF _targetOffset = new(0, 0);
        private bool _panning;
        private Point _lastMouse;
        private readonly System.Windows.Forms.Timer _zoomTimer;

        public Image? Image
        {
            get => _image;
            set
            {
                _image = value;
                ResetView();
                Invalidate();
            }
        }

        public ZoomableImageBox()
        {
            TabStop = true;
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.UserPaint
                     | ControlStyles.ResizeRedraw, true);

            _zoomTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _zoomTimer.Tick += (s, e) =>
            {
                float delta = _targetZoom - _zoom;
                if (Math.Abs(delta) < 0.001f)
                {
                    _zoom = _targetZoom;
                }
                else
                {
                    _zoom += delta * 0.2f;
                }

                float offsetDeltaX = _targetOffset.X - _offset.X;
                float offsetDeltaY = _targetOffset.Y - _offset.Y;
                if (Math.Abs(offsetDeltaX) < 0.5f && Math.Abs(offsetDeltaY) < 0.5f)
                {
                    _offset = _targetOffset;
                }
                else
                {
                    _offset = new PointF(
                        _offset.X + offsetDeltaX * 0.2f,
                        _offset.Y + offsetDeltaY * 0.2f);
                }

                ClampOffsets();

                if (_zoom == _targetZoom && _offset == _targetOffset)
                    _zoomTimer.Stop();

                Invalidate();
            };

            MouseWheel += OnMouseWheelZoom;
            MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    Focus();

                if (e.Button == MouseButtons.Left && _cropMode && !_cropFixed)
                {
                    HandleCropClick(e.Location);
                    return;
                }

                if (e.Button == MouseButtons.Left && _cropMode && _cropFixed && IsPointInCrop(e.Location))
                {
                    _cropFocused = true;
                    _cropDragging = true;
                    _cropDragStartScreen = e.Location;
                    Invalidate();
                    return;
                }

                if (e.Button == MouseButtons.Left && _cropMode && _cropFixed)
                {
                    _cropFocused = false;
                    Invalidate();
                }

                if (e.Button == MouseButtons.Left)
                {
                    _panning = true;
                    _lastMouse = e.Location;
                    Cursor = Cursors.Hand;
                }

            };

            MouseUp += (s, e) =>
            {
                _panning = false;
                _cropDragging = false;
                Cursor = Cursors.Default;
            };

            MouseMove += (s, e) =>
            {
                if (_cropMode && _cropAnchorSet && !_cropFixed)
                {
                    _cropCurrentImage = ClampToImage(ScreenToImage(e.Location));
                    Invalidate();
                }
                else if (_cropDragging)
                {
                    var deltaX = (e.X - _cropDragStartScreen.X) / _zoom;
                    var deltaY = (e.Y - _cropDragStartScreen.Y) / _zoom;
                    if (Math.Abs(deltaX) > 0f || Math.Abs(deltaY) > 0f)
                    {
                        MoveCrop(deltaX, deltaY);
                        _cropDragStartScreen = e.Location;
                    }
                }
                else if (_panning)
                {
                    _offset.X += e.X - _lastMouse.X;
                    _offset.Y += e.Y - _lastMouse.Y;
                    _targetOffset = _offset;
                    ClampOffsets();
                    _lastMouse = e.Location;
                    Invalidate();
                }
            };

            Resize += (s, e) => CenterImage();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (HandlePreviewKey(e.KeyCode))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return keyData switch
            {
                Keys.Left => true,
                Keys.Right => true,
                Keys.Up => true,
                Keys.Down => true,
                _ => base.IsInputKey(keyData)
            };
        }

        public bool TryPan(Keys keyCode)
        {
            if (_cropMode && !_cropFixed)
                return false;

            const float panStep = 20f;
            switch (keyCode)
            {
                case Keys.Left:
                    _offset.X += panStep;
                    break;
                case Keys.Right:
                    _offset.X -= panStep;
                    break;
                case Keys.Up:
                    _offset.Y += panStep;
                    break;
                case Keys.Down:
                    _offset.Y -= panStep;
                    break;
                default:
                    return false;
            }

            _targetOffset = _offset;
            ClampOffsets();
            Invalidate();
            return true;
        }

        public bool HandlePreviewKey(Keys keyCode)
        {
            if (keyCode == Keys.C)
            {
                ToggleCropMode();
                return true;
            }

            if (_cropMode && keyCode == Keys.Enter)
            {
                ApplyCrop();
                return true;
            }

            if (_cropMode && _cropFixed && _cropFocused && TryMoveCrop(keyCode))
                return true;

            var panned = TryPan(keyCode);
            if (panned && _cropMode && _cropFixed)
                _cropFocused = false;

            return panned;
        }

        private bool TryMoveCrop(Keys keyCode)
        {
            const float step = 5f;
            float dx = 0f;
            float dy = 0f;

            switch (keyCode)
            {
                case Keys.Left:
                    dx = -step;
                    break;
                case Keys.Right:
                    dx = step;
                    break;
                case Keys.Up:
                    dy = -step;
                    break;
                case Keys.Down:
                    dy = step;
                    break;
                default:
                    return false;
            }

            MoveCrop(dx, dy);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _zoomTimer.Stop();
                _zoomTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ResetView()
        {
            _zoom = 1f;
            _targetZoom = 1f;
            CenterImage();
        }

        private void CenterImage()
        {
            if (_image == null) return;

            float scale = Math.Min(
                Width / (float)_image.Width,
                Height / (float)_image.Height);

            _zoom = scale;
            _targetZoom = scale;

            float imgW = _image.Width * _zoom;
            float imgH = _image.Height * _zoom;

            _offset = new PointF(
                (Width - imgW) / 2f,
                (Height - imgH) / 2f);
            _targetOffset = _offset;

            Invalidate();
        }

        private void OnMouseWheelZoom(object? sender, MouseEventArgs e)
        {
            if (_image == null) return;

            float zoomStep = e.Delta > 0 ? 1.1f : 0.9f;
            float scale = Math.Min(
            Width / (float)_image.Width,
            Height / (float)_image.Height);

            float newTarget = Math.Clamp(_targetZoom * zoomStep, _minZoom*scale, 20f);

            float imageX = (e.X - _offset.X) / _zoom;
            float imageY = (e.Y - _offset.Y) / _zoom;

            _targetZoom = newTarget;
            _targetOffset = new PointF(
                e.X - imageX * _targetZoom,
                e.Y - imageY * _targetZoom);

            ClampOffsets();

            if (!_zoomTimer.Enabled)
                _zoomTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_image == null)
                return;

            var g = e.Graphics;

            g.Clear(BackColor);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.TranslateTransform(_offset.X, _offset.Y);
            g.ScaleTransform(_zoom, _zoom);

            g.DrawImage(_image, Point.Empty);

            g.ResetTransform();

            if (_cropMode && _cropAnchorSet)
                DrawCropOverlay(g);
        }

        private void ToggleCropMode()
        {
            _cropMode = !_cropMode;
            _cropAnchorSet = false;
            _cropFixed = false;
            _cropFocused = false;
            Cursor = _cropMode ? Cursors.Cross : Cursors.Default;
            Invalidate();
        }

        private void HandleCropClick(Point location)
        {
            if (_image == null)
                return;

            var imagePoint = ClampToImage(ScreenToImage(location));

            if (!_cropAnchorSet || _cropFixed)
            {
                _cropAnchorSet = true;
                _cropFixed = false;
                _cropAnchorImage = imagePoint;
                _cropCurrentImage = imagePoint;
                _cropEndImage = imagePoint;
            }
            else
            {
                _cropEndImage = imagePoint;
                _cropFixed = true;
                _cropFocused = true;
            }

            Invalidate();
        }

        private void DrawCropOverlay(Graphics g)
        {
            using var guidePen = new Pen(Color.FromArgb(230, 0, 0, 0), 2f)
            {
                DashStyle = DashStyle.Dot
            };

            var anchorScreen = ImageToScreen(_cropAnchorImage);
            g.DrawLine(guidePen, 0, anchorScreen.Y, Width, anchorScreen.Y);
            g.DrawLine(guidePen, anchorScreen.X, 0, anchorScreen.X, Height);

            if (_cropFixed || (_cropAnchorSet && !_cropFixed))
            {
                var endImage = _cropFixed ? _cropEndImage : _cropCurrentImage;
                var startScreen = ImageToScreen(_cropAnchorImage);
                var endScreen = ImageToScreen(endImage);
                var rect = RectangleF.FromLTRB(
                    Math.Min(startScreen.X, endScreen.X),
                    Math.Min(startScreen.Y, endScreen.Y),
                    Math.Max(startScreen.X, endScreen.X),
                    Math.Max(startScreen.Y, endScreen.Y));
                using var rectPen = new Pen(_cropFocused ? Color.FromArgb(0, 120, 215) : Color.FromArgb(230, 0, 0, 0), 2f)
                {
                    DashStyle = DashStyle.Dot
                };
                g.DrawRectangle(rectPen, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        private bool IsPointInCrop(Point location)
        {
            if (!_cropFixed)
                return false;

            var start = _cropAnchorImage;
            var end = _cropEndImage;

            var screenRect = RectangleF.FromLTRB(
                Math.Min(start.X, end.X) * _zoom + _offset.X,
                Math.Min(start.Y, end.Y) * _zoom + _offset.Y,
                Math.Max(start.X, end.X) * _zoom + _offset.X,
                Math.Max(start.Y, end.Y) * _zoom + _offset.Y);

            if (screenRect.Width <= 0f || screenRect.Height <= 0f)
                return false;

            screenRect.Inflate(4f, 4f);
            return screenRect.Contains(location);
        }

        private void ApplyCrop()
        {
            if (_image == null || !_cropAnchorSet || !_cropFixed)
                return;

            var rect = GetCropRectangle();
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            var bmp = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(_image, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }

            _image.Dispose();
            _image = bmp;

            _cropMode = false;
            _cropAnchorSet = false;
            _cropFixed = false;
            _cropFocused = false;
            Cursor = Cursors.Default;

            ResetView();
        }

        private Rectangle GetCropRectangle()
        {
            var start = _cropAnchorImage;
            var end = _cropEndImage;

            float left = Math.Clamp(Math.Min(start.X, end.X), 0, _image!.Width);
            float right = Math.Clamp(Math.Max(start.X, end.X), 0, _image!.Width);
            float top = Math.Clamp(Math.Min(start.Y, end.Y), 0, _image!.Height);
            float bottom = Math.Clamp(Math.Max(start.Y, end.Y), 0, _image!.Height);

            int x = (int)Math.Round(left);
            int y = (int)Math.Round(top);
            int width = (int)Math.Round(right - left);
            int height = (int)Math.Round(bottom - top);

            return new Rectangle(x, y, width, height);
        }

        private void MoveCrop(float dx, float dy)
        {
            if (_image == null || !_cropFixed)
                return;

            var rect = GetCropRectangle();
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            float left = rect.Left + dx;
            float top = rect.Top + dy;

            left = Math.Clamp(left, 0, _image.Width - rect.Width);
            top = Math.Clamp(top, 0, _image.Height - rect.Height);

            _cropAnchorImage = new PointF(left, top);
            _cropEndImage = new PointF(left + rect.Width, top + rect.Height);
            _cropCurrentImage = _cropEndImage;

            Invalidate();
        }

        private PointF ScreenToImage(Point point)
        {
            return new PointF(
                (point.X - _offset.X) / _zoom,
                (point.Y - _offset.Y) / _zoom);
        }

        private PointF ImageToScreen(PointF point)
        {
            return new PointF(
                point.X * _zoom + _offset.X,
                point.Y * _zoom + _offset.Y);
        }

        private PointF ClampToImage(PointF point)
        {
            if (_image == null)
                return point;

            return new PointF(
                Math.Clamp(point.X, 0, _image.Width),
                Math.Clamp(point.Y, 0, _image.Height));
        }

        private void ClampOffsets()
        {
            if (_image == null || Width <= 0 || Height <= 0)
                return;

            float imgW = _image.Width * _zoom;
            float imgH = _image.Height * _zoom;

            float minX;
            float maxX;
            if (imgW <= Width)
            {
                minX = maxX = (Width - imgW) / 2f;
            }
            else
            {
                minX = Width - imgW;
                maxX = 0f;
            }

            float minY;
            float maxY;
            if (imgH <= Height)
            {
                minY = maxY = (Height - imgH) / 2f;
            }
            else
            {
                minY = Height - imgH;
                maxY = 0f;
            }

            _offset = new PointF(
                Math.Clamp(_offset.X, minX, maxX),
                Math.Clamp(_offset.Y, minY, maxY));

            _targetOffset = new PointF(
                Math.Clamp(_targetOffset.X, minX, maxX),
                Math.Clamp(_targetOffset.Y, minY, maxY));
        }
    }

    private SplitContainer? GetSplitterByName(string name) => name switch
    {
        "main" => MainSplitContainer,
        "input" => InputPanel.Splitter,
        "output" => OutputPanel.Splitter,
        _ => null
    };

    private static void ApplyOrientationSafe(SplitContainer container, Orientation target)
    {
        if (container.IsDisposed)
            return;

        int panel1Min = container.Panel1MinSize;
        int panel2Min = container.Panel2MinSize;

        try
        {
            NormalizeSplitterDistance(container, target);
            container.Orientation = target;
        }
        catch (InvalidOperationException)
        {
            // Temporarily relax min sizes to avoid invalid SplitterDistance on orientation swap.
            container.Panel1MinSize = 0;
            container.Panel2MinSize = 0;
            if (NormalizeSplitterDistance(container, target))
                container.Orientation = target;
        }
        finally
        {
            container.Panel1MinSize = panel1Min;
            container.Panel2MinSize = panel2Min;
        }
    }

    private static bool NormalizeSplitterDistance(SplitContainer container, Orientation target)
    {
        int maxCurrent = GetMaxSplitterDistance(container, container.Orientation);
        int maxTarget = GetMaxSplitterDistance(container, target);
        int max = Math.Min(maxCurrent, maxTarget);
        int min = container.Panel1MinSize;

        if (max < min || max < 0)
            return false;

        int distance = Math.Clamp(container.SplitterDistance, min, max);
        if (distance != container.SplitterDistance)
            container.SplitterDistance = distance;
        return true;
    }

    private static int GetMaxSplitterDistance(SplitContainer container, Orientation orientation)
    {
        int dimension = orientation == Orientation.Horizontal ? container.Height : container.Width;
        return dimension - container.Panel2MinSize - container.SplitterWidth;
    }

    private void DebounceSplitterSave()
    {
        _layoutDebouncer.Debounce(() => InvokeOnUI(() => SplitterMoved?.Invoke(this, EventArgs.Empty)));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _layoutDebouncer.Dispose();
        base.Dispose(disposing);
    }

    #endregion
}
