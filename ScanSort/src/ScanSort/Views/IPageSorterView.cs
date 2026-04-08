using ScanSort.Controls;

namespace ScanSort.Views;

public interface IPageSorterView
{
    // Panels
    ThumbnailPanel InputPanel { get; }
    ThumbnailPanel OutputPanel { get; }

    // Events — file operations
    event EventHandler OpenFileRequested;
    event EventHandler OpenFolderRequested;
    event EventHandler ExportRequested;
    event EventHandler ExitRequested;

    // Events — edit operations
    event EventHandler RotateLeftRequested;
    event EventHandler RotateRightRequested;
    event EventHandler ToggleLayoutRequested;
    event EventHandler UndoRequested;
    event EventHandler RedoRequested;

    // Events — context menu (panel + action name)
    event EventHandler<ContextMenuAction> ContextMenuClicked;

    // Events — mode toggles
    event EventHandler DuplexToggled;
    event EventHandler CoverToggled;
    event EventHandler AutoDeskewRequested;

    // Events — layout
    event EventHandler<string> LayoutModeChanged;
    event EventHandler SplitterMoved;
    event EventHandler FormResized;

    // Display
    string Title { set; }
    string StatusText { set; }
    bool DuplexChecked { get; set; }
    bool CoverChecked { get; set; }

    // Layout
    void SetLayoutMode(string mode, Orientation mainOrientation, Orientation subOrientation,
        Manina.Windows.Forms.View listViewMode, bool showPreviews);
    void SetSplitterRatio(string containerName, double ratio);
    double GetSplitterRatio(string containerName);
    SplitContainer MainSplitContainer { get; }

    // Progress
    void ShowProgress(int current, int total, string message);
    void HideProgress();

    // Helpers
    void InvokeOnUI(Action action);
}

public record ContextMenuAction(ThumbnailPanel Panel, string Action);
