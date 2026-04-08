namespace ScanSort.Models;

/// <summary>
/// Thread-safe collection mapping page title to PageMetadata.
/// </summary>
public class PageMetadataMap
{
    private readonly Dictionary<string, PageMetadata> _map = new();
    private readonly object _lock = new();

    public int Count
    {
        get { lock (_lock) return _map.Count; }
    }

    public IEnumerable<string> Keys
    {
        get { lock (_lock) return _map.Keys.ToList(); }
    }

    public IEnumerable<PageMetadata> Values
    {
        get { lock (_lock) return _map.Values.ToList(); }
    }

    public PageMetadata? this[string key]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(key)) return null;
            lock (_lock)
            {
                return _map.GetValueOrDefault(key);
            }
        }
        set
        {
            if (string.IsNullOrWhiteSpace(key) || value is null) return;
            lock (_lock)
            {
                _map[key] = value;
            }
        }
    }

    /// <summary>
    /// Updates PageNumber on each entry to match its position in the ordered list.
    /// Called after every drag-drop reorder.
    /// </summary>
    public void SyncPageNumbers(IReadOnlyList<string> orderedTitles)
    {
        ArgumentNullException.ThrowIfNull(orderedTitles);
        lock (_lock)
        {
            for (int i = 0; i < orderedTitles.Count; i++)
            {
                if (_map.TryGetValue(orderedTitles[i], out var metadata))
                    metadata.PageNumber = i;
            }
        }
    }

    public bool ContainsKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        lock (_lock) return _map.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return false;
        lock (_lock) return _map.Remove(key);
    }

    public void Clear()
    {
        lock (_lock) _map.Clear();
    }
}
