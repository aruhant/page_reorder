using System.Drawing;
using Manina.Windows.Forms;
using ScanSort.Commands;
using ScanSort.Controls;
using ScanSort.Infrastructure;
using ScanSort.Models;
using ScanSort.Services;
using ScanSort.Views;

namespace ScanSort.Presenters;

public class PageSorterPresenter
{
    private readonly IPdfService _pdfService;
    private readonly IImageService _imageService;
    private readonly IDeskewService _deskewService;
    private readonly IExportService _exportService;
    private readonly IConfigService _configService;
    private readonly CommandHistory _commandHistory;

    private IPageSorterView _view = null!;
    private SourceDocument? _document;
    private AppSettings _settings = null!;
    private CancellationTokenSource? _cts;

    public PageSorterPresenter(IPdfService pdfService, IImageService imageService,
        IDeskewService deskewService, IExportService exportService,
        IConfigService configService, CommandHistory commandHistory)
    {
        _pdfService = pdfService;
        _imageService = imageService;
        _deskewService = deskewService;
        _exportService = exportService;
        _configService = configService;
        _commandHistory = commandHistory;
    }

    public void Initialize(IPageSorterView view)
    {
        _view = view;
        _settings = _configService.Load();

        // Restore settings
        _view.DuplexChecked = _settings.EnableDuplexSelectionMode;
        _view.CoverChecked = _settings.EnableDocumentWithCoverMode;

        // Subscribe to view events
        _view.OpenFileRequested += async (s, e) => await OnOpenFile();
        _view.OpenFolderRequested += async (s, e) => await OnOpenFolder();
        _view.ExportRequested += async (s, e) => await OnExport();
        _view.ExitRequested += (s, e) => OnExit();
        _view.RotateLeftRequested += (s, e) => OnRotate(-1f);
        _view.RotateRightRequested += (s, e) => OnRotate(1f);
        _view.ToggleLayoutRequested += (s, e) => OnToggleOrientation();
        _view.DuplexToggled += (s, e) => OnDuplexToggle();
        _view.CoverToggled += (s, e) => OnCoverToggle();
        _view.AutoDeskewRequested += async (s, e) => await OnAutoDeskew();
        _view.FullScreenRequested += (s, e) => OnFullScreenPreview();
        _view.UndoRequested += (s, e) => OnUndo();
        _view.RedoRequested += (s, e) => OnRedo();
        _view.LayoutModeChanged += (s, mode) => OnLayoutModeChanged(mode);
        _view.SplitterMoved += (s, e) => OnSplitterMoved();
        _view.FormResized += (s, e) => RestoreLayout();

        // Subscribe to context menu
        _view.ContextMenuClicked += (s, action) => OnContextMenu(action);

        // Subscribe to panel events
        _view.InputPanel.ItemHovered += (s, item) => _view.InputPanel.UpdatePreview(item);
        _view.OutputPanel.ItemHovered += (s, item) => _view.OutputPanel.UpdatePreview(item);
        _view.InputPanel.SelectionChanged += (s, e) => OnSelectionChanged(_view.InputPanel);
        _view.OutputPanel.SelectionChanged += (s, e) => OnSelectionChanged(_view.OutputPanel);
        _view.InputPanel.DropComplete += (s, e) => OnDropComplete();
        _view.OutputPanel.DropComplete += (s, e) => OnDropComplete();

        // Subscribe to command history for dirty tracking
        _commandHistory.StateChanged += (s, e) => UpdateTitle();

        // Restore layout
        OnLayoutModeChanged(_settings.LayoutMode);
    }

    #region File Operations

    private async Task OnOpenFile()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "PDF Files|*.pdf",
            Title = "Select a PDF File"
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        if (_commandHistory.IsDirty && !ConfirmDiscardChanges()) return;

        _document = new SourceDocument(dialog.FileName);
        _commandHistory.Clear();
        _view.InputPanel.ClearPreview();
        _view.OutputPanel.ClearPreview();

        var outputFolder = GetExtractionFolder(_document);
        PrepareExtractionFolder(outputFolder);

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var progress = new Progress<(int current, int total, string message)>(
                p => _view.ShowProgress(p.current, p.total, p.message));

            await _pdfService.ExtractImagesAsync(_document.SourcePath, outputFolder,
                _document.PageMetadataMap, progress, _cts.Token);

            LoadImages(outputFolder);
            SetupPanels();
            UpdateTitle();
            _view.HideProgress();
        }
        catch (OperationCanceledException)
        {
            _view.StatusText = "Extraction cancelled.";
            _view.HideProgress();
        }
    }

    private async Task OnOpenFolder()
    {
        using var dialog = new OpenFileDialog
        {
            ValidateNames = false,
            CheckFileExists = false,
            CheckPathExists = true,
            FileName = "Folder Selection."
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        if (_commandHistory.IsDirty && !ConfirmDiscardChanges()) return;

        string folderPath = Path.GetDirectoryName(dialog.FileName)!;
        _document = new SourceDocument(folderPath);
        _commandHistory.Clear();
        _view.InputPanel.ClearPreview();
        _view.OutputPanel.ClearPreview();

        LoadImages(folderPath);
        SetupPanels();
        UpdateTitle();
    }

    private async Task OnExport()
    {
        if (_document == null) return;
        var orderedTitles = _view.OutputPanel.GetOrderedTitles();
        if (orderedTitles.Count == 0)
        {
            MessageBox.Show("No pages in the output list to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            var progress = new Progress<(int current, int total, string message)>(
                p => _view.ShowProgress(p.current, p.total, p.message));

            await _exportService.ExportAsync(_document, orderedTitles, progress, _cts.Token);
            _commandHistory.MarkSaved();
            UpdateTitle();
            _view.HideProgress();
        }
        catch (OperationCanceledException)
        {
            _view.StatusText = "Export cancelled.";
            _view.HideProgress();
        }
    }

    private void OnExit()
    {
        if (_commandHistory.IsDirty && !ConfirmDiscardChanges()) return;
        Application.Exit();
    }

    #endregion

    #region Rotation

    private void OnRotate(float delta)
    {
        var (panel, items) = GetFocusedSelection();
        if (items.Count == 0 || _document == null) return;

        var commands = items
            .Select(item => _document.PageMetadataMap[item.Text])
            .Where(m => m != null)
            .Select(m => (IUndoableCommand)new RotateCommand(m!, delta))
            .ToList();

        if (commands.Count > 0)
        {
            _commandHistory.Execute(new BatchCommand($"Rotate {commands.Count} pages by {delta}°", commands));
            RefreshItems(panel!, items);
        }
    }

    private void OnToggleOrientation()
    {
        var (panel, items) = GetFocusedSelection();
        if (items.Count == 0 || _document == null) return;

        var commands = items
            .Select(item => _document.PageMetadataMap[item.Text])
            .Where(m => m != null)
            .Select(m => (IUndoableCommand)new OrientationCommand(m!))
            .ToList();

        if (commands.Count > 0)
        {
            _commandHistory.Execute(new BatchCommand($"Rotate layout of {commands.Count} pages", commands));
            RefreshItems(panel!, items);
        }
    }

    private async Task OnAutoDeskew()
    {
        if (_document == null) return;

        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        var pages = _document.PageMetadataMap.Values.ToList();
        int total = pages.Count;

        await Task.Run(() =>
        {
            for (int i = 0; i < total; i++)
            {
                _cts.Token.ThrowIfCancellationRequested();
                var page = pages[i];
                float angle = _deskewService.DetectSkewAngle(page.FileName);
                page.Rotate = -angle; // Negate: correction is opposite of detected skew
                _view.ShowProgress(i + 1, total, $"Deskewed {page.Title} to {page.Rotate:F1}°");
            }
        }, _cts.Token);

        _view.InputPanel.ListView.Refresh();
        _view.OutputPanel.ListView.Refresh();
        _view.HideProgress();
    }

    #endregion

    #region Selection & Duplex

    private void OnSelectionChanged(ThumbnailPanel panel)
    {
        if (_settings.EnableDuplexSelectionMode)
            DuplexSelectionHelper.ApplyDuplexSelection(panel.ListView, _settings.EnableDocumentWithCoverMode);
    }

    private void OnDuplexToggle()
    {
        _settings.EnableDuplexSelectionMode = !_settings.EnableDuplexSelectionMode;
        _view.DuplexChecked = _settings.EnableDuplexSelectionMode;
        _configService.Save(_settings);
    }

    private void OnCoverToggle()
    {
        _settings.EnableDocumentWithCoverMode = !_settings.EnableDocumentWithCoverMode;
        _view.CoverChecked = _settings.EnableDocumentWithCoverMode;
        _configService.Save(_settings);
    }

    #endregion

    #region Context Menu

    private void OnContextMenu(ContextMenuAction action)
    {
        if (_document == null) return;

        var items = action.Panel.GetSelectedItems();
        // If nothing selected in list, try the previewed item
        if (items.Count == 0 && action.Panel.Preview.Tag is ImageListViewItem previewItem)
            items = new[] { previewItem };
        if (items.Count == 0) return;

        switch (action.Action)
        {
            case "blur":
                var blurCmds = items
                    .Select(i => _document.PageMetadataMap[i.Text])
                    .Where(m => m != null)
                    .Select(m => (IUndoableCommand)new ToggleBlurCommand(m!))
                    .ToList();
                if (blurCmds.Count > 0)
                {
                    _commandHistory.Execute(new BatchCommand("Toggle blur", blurCmds));
                    foreach (var item in items) item.Update();
                }
                break;

            case "comment":
                string comment = PromptDialog.Show("Enter Comment", "Comment");
                if (string.IsNullOrEmpty(comment)) break;
                var commentCmds = items
                    .Select(i => _document.PageMetadataMap[i.Text])
                    .Where(m => m != null)
                    .Select(m => (IUndoableCommand)new SetCommentCommand(m!, comment))
                    .ToList();
                if (commentCmds.Count > 0)
                {
                    _commandHistory.Execute(new BatchCommand("Set comment", commentCmds));
                    foreach (var item in items) item.Update();
                }
                break;

            case "rotate90":
                var rotateCmds = items
                    .Select(i => _document.PageMetadataMap[i.Text])
                    .Where(m => m != null)
                    .Select(m => (IUndoableCommand)new OrientationCommand(m!))
                    .ToList();
                if (rotateCmds.Count > 0)
                {
                    _commandHistory.Execute(new BatchCommand("Rotate 90", rotateCmds));
                    RefreshItems(action.Panel, items);
                }
                break;

            case "missing":
                if (items.Count > 0)
                {
                    // Find the index of the first selected item in the output list
                    int insertIndex = 0;
                    for (int i = 0; i < action.Panel.ListView.Items.Count; i++)
                    {
                        if (action.Panel.ListView.Items[i].Text == items[0].Text)
                        {
                            insertIndex = i;
                            break;
                        }
                    }
                    var metadata = _document.PageMetadataMap[items[0].Text];
                    int pageNum = metadata?.PageNumber ?? insertIndex;

                    var cmd = new InsertMissingPageCommand(
                        _document.PageMetadataMap,
                        (idx, title) => _view.InvokeOnUI(() =>
                            action.Panel.ListView.Items.Insert(idx, new ImageListViewItem(title))),
                        (title) => _view.InvokeOnUI(() =>
                        {
                            for (int i = 0; i < action.Panel.ListView.Items.Count; i++)
                            {
                                if (action.Panel.ListView.Items[i].Text == title)
                                {
                                    action.Panel.ListView.Items.RemoveAt(i);
                                    break;
                                }
                            }
                        }),
                        insertIndex, pageNum);
                    _commandHistory.Execute(cmd);
                }
                break;
        }
    }

    #endregion

    #region Drag-Drop

    private void OnDropComplete()
    {
        if (_document == null) return;
        var titles = _view.OutputPanel.GetOrderedTitles();
        _document.PageMetadataMap.SyncPageNumbers(titles);
    }

    #endregion

    #region Undo/Redo

    private void OnUndo()
    {
        _commandHistory.Undo();
        _view.InputPanel.ListView.Refresh();
        _view.OutputPanel.ListView.Refresh();
    }

    private void OnRedo()
    {
        _commandHistory.Redo();
        _view.InputPanel.ListView.Refresh();
        _view.OutputPanel.ListView.Refresh();
    }

    private void OnFullScreenPreview()
    {
        if (_document == null) return;

        var (panel, items) = GetFocusedSelection();
        if (panel == null) return;

        var item = items.Count > 0 ? items[0] : panel.Preview.Tag as ImageListViewItem;
        if (item == null) return;

        var metadata = _document.PageMetadataMap[item.Text];
        if (metadata == null || metadata.PageType == PageType.MissingContent) return;

        try
        {
            string path = Path.Combine(item.FilePath, item.FileName);
            using var original = Image.FromFile(path);
            var previewImage = _imageService.RotateImage(original, metadata.Orientation, metadata.Rotate);
            _view.ShowFullScreenPreview(previewImage, item.Text);
        }
        catch
        {
            _view.StatusText = "Unable to open full screen preview.";
        }
    }

    #endregion

    #region Layout

    private void OnLayoutModeChanged(string mode)
    {
        _settings.LayoutMode = mode;

        var (mainOrient, subOrient, viewMode, showPreviews) = mode switch
        {
            "Horizontal" => (Orientation.Horizontal, Orientation.Horizontal,
                Manina.Windows.Forms.View.HorizontalStrip, true),
            "Vertical" => (Orientation.Vertical, Orientation.Vertical,
                Manina.Windows.Forms.View.VerticalStrip, true),
            "HorizontalThumbs" => (Orientation.Horizontal, Orientation.Horizontal,
                Manina.Windows.Forms.View.Thumbnails, false),
            "VerticalThumbs" => (Orientation.Vertical, Orientation.Vertical,
                Manina.Windows.Forms.View.Thumbnails, false),
            _ => (Orientation.Horizontal, Orientation.Horizontal,
                Manina.Windows.Forms.View.HorizontalStrip, true)
        };

        _view.SetLayoutMode(mode, mainOrient, subOrient, viewMode, showPreviews);
        RestoreLayout();
        _configService.Save(_settings);
    }

    private void OnSplitterMoved()
    {
        _settings.SplitterRatios[$"main_{_settings.LayoutMode}"] = _view.GetSplitterRatio("main");
        _settings.SplitterRatios[$"input_{_settings.LayoutMode}"] = _view.GetSplitterRatio("input");
        _settings.SplitterRatios[$"output_{_settings.LayoutMode}"] = _view.GetSplitterRatio("output");
        _configService.Save(_settings);
    }

    private void RestoreLayout()
    {
        string mode = _settings.LayoutMode;
        _view.SetSplitterRatio("main", _settings.SplitterRatios.GetValueOrDefault($"main_{mode}", 0.5));
        _view.SetSplitterRatio("input", _settings.SplitterRatios.GetValueOrDefault($"input_{mode}", 0.25));
        _view.SetSplitterRatio("output", _settings.SplitterRatios.GetValueOrDefault($"output_{mode}", 0.25));
    }

    #endregion

    #region Helpers

    private void LoadImages(string folder)
    {
        var dir = new DirectoryInfo(folder);
        _view.InputPanel.ListView.Items.Clear();
        _view.OutputPanel.ListView.Items.Clear();
        _view.InputPanel.ListView.SuspendLayout();

        int index = 1;
        var files = dir.GetFiles("*.*")
            .Where(f => f.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                     || f.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                     || f.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.Name)
            .ToList();

        foreach (var file in files)
        {
            string title = file.Name;
            var item = new ImageListViewItem(file.FullName, title);

            if (_document!.PageMetadataMap[title] == null)
            {
                var metadata = new PageMetadata(file.FullName, title, PageType.Content, index, index);
                metadata.MediaRect = _imageService.GetImageRect(file.FullName);
                _document.PageMetadataMap[title] = metadata;
                index++;
            }

            _view.InputPanel.ListView.Items.Add(item);
        }

        _view.InputPanel.ListView.ResumeLayout();
        _view.StatusText = "Ready";
    }

    private void SetupPanels()
    {
        if (_document == null) return;
        _view.InputPanel.SetMetadataMap(_document.PageMetadataMap);
        _view.OutputPanel.SetMetadataMap(_document.PageMetadataMap);
    }

    private void UpdateTitle()
    {
        string title = "ScanSort";
        if (_document != null)
            title = _document.Title;
        if (_commandHistory.IsDirty)
            title += " *";
        _view.Title = title;
    }

    private (ThumbnailPanel? panel, IReadOnlyList<ImageListViewItem> items) GetFocusedSelection()
    {
        if (_view.InputPanel.ListView.Focused && _view.InputPanel.ListView.SelectedItems.Count > 0)
            return (_view.InputPanel, _view.InputPanel.GetSelectedItems());
        if (_view.OutputPanel.ListView.Focused && _view.OutputPanel.ListView.SelectedItems.Count > 0)
            return (_view.OutputPanel, _view.OutputPanel.GetSelectedItems());
        return (null, Array.Empty<ImageListViewItem>());
    }

    private void RefreshItems(ThumbnailPanel panel, IReadOnlyList<ImageListViewItem> items)
    {
        foreach (var item in items) item.Update();
        if (items.Count > 0)
            panel.UpdatePreview(items[0]);
    }

    private static string GetExtractionFolder(SourceDocument document)
    {
        string basePath = Path.Combine(Path.GetTempPath(), "ScanSort", "images");
        return Path.Combine(basePath, document.Title);
    }

    private static void PrepareExtractionFolder(string folder)
    {
        if (Directory.Exists(folder))
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                try { File.Delete(file); } catch { }
            }
        }
        else
        {
            Directory.CreateDirectory(folder);
        }
    }

    private bool ConfirmDiscardChanges()
    {
        var result = MessageBox.Show(
            "You have unsaved changes. Discard them?",
            "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        return result == DialogResult.Yes;
    }

    #endregion
}
