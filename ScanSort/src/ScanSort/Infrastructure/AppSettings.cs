namespace ScanSort.Infrastructure;

public class AppSettings
{
    public bool EnableDuplexSelectionMode { get; set; } = false;
    public bool EnableDocumentWithCoverMode { get; set; } = true;
    public string LayoutMode { get; set; } = "Horizontal";
    public Dictionary<string, double> SplitterRatios { get; set; } = new();
}
