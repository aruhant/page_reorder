namespace ScanSort.Infrastructure;

/// <summary>
/// Delays execution of an action until a quiet period has elapsed.
/// Used for saving layout positions to avoid excessive writes during drag.
/// </summary>
public sealed class Debouncer : IDisposable
{
    private readonly System.Threading.Timer _timer;
    private Action? _pendingAction;
    private readonly int _defaultDelayMs;

    public Debouncer(int defaultDelayMs = 1250)
    {
        _defaultDelayMs = defaultDelayMs;
        _timer = new System.Threading.Timer(OnTimerElapsed);
    }

    public void Debounce(Action action, int? delayMs = null)
    {
        _pendingAction = action;
        _timer.Change(delayMs ?? _defaultDelayMs, Timeout.Infinite);
    }

    private void OnTimerElapsed(object? state)
    {
        var action = _pendingAction;
        _pendingAction = null;
        action?.Invoke();
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
