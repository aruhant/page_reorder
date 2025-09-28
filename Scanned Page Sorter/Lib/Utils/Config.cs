using System;
using System.Configuration;

/// <summary>
/// Thread-safe singleton configuration manager for application settings
/// </summary>
public sealed class AppConfig : IDisposable
{
    private static readonly object _lock = new object();
    private static AppConfig _instance;
    
    private readonly Configuration _config;
    private bool _duplexSelectModeOn;
    private bool _hasCover;
    private bool _disposed;

    // Constants for configuration keys
    private const string DUPLEX_SELECT_MODE_KEY = "DuplexSelectMode";
    private const string HAS_COVER_KEY = "HasCover";
    private const string DEFAULT_DUPLEX_MODE = "false";
    private const string DEFAULT_HAS_COVER = "true";

    /// <summary>
    /// Gets the singleton instance of AppConfig
    /// </summary>
    public static AppConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new AppConfig();
                    }
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Gets or sets whether duplex selection mode is enabled
    /// </summary>
    public bool enableDuplexSelectionMode
    {
        get { return _duplexSelectModeOn; }
        set
        {
            if (_duplexSelectModeOn != value)
            {
                _duplexSelectModeOn = value;
                Save();
            }
        }
    }

    /// <summary>
    /// Gets or sets whether document with cover mode is enabled
    /// </summary>
    public bool enableDocumentWithCoverMode
    {
        get { return _hasCover; }
        set
        {
            if (_hasCover != value)
            {
                _hasCover = value;
                Save();
            }
        }
    }

    private AppConfig()
    {
        try
        {
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            InitializeSettings();
            LoadSettings();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing configuration: {ex.Message}");
            // Use default values if configuration fails
            _duplexSelectModeOn = false;
            _hasCover = true;
        }
    }

    /// <summary>
    /// Initializes default settings if they don't exist
    /// </summary>
    private void InitializeSettings()
    {
        if (_config.AppSettings.Settings[DUPLEX_SELECT_MODE_KEY] == null)
        {
            _config.AppSettings.Settings.Add(DUPLEX_SELECT_MODE_KEY, DEFAULT_DUPLEX_MODE);
        }

        if (_config.AppSettings.Settings[HAS_COVER_KEY] == null)
        {
            _config.AppSettings.Settings.Add(HAS_COVER_KEY, DEFAULT_HAS_COVER);
        }
    }

    /// <summary>
    /// Loads settings from configuration file
    /// </summary>
    private void LoadSettings()
    {
        try
        {
            var duplexValue = _config.AppSettings.Settings[DUPLEX_SELECT_MODE_KEY]?.Value;
            var coverValue = _config.AppSettings.Settings[HAS_COVER_KEY]?.Value;

            _duplexSelectModeOn = ParseBoolSetting(duplexValue, false);
            _hasCover = ParseBoolSetting(coverValue, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
            // Use defaults on error
            _duplexSelectModeOn = false;
            _hasCover = true;
        }
    }

    /// <summary>
    /// Safely parses a boolean setting with fallback to default value
    /// </summary>
    /// <param name="value">String value to parse</param>
    /// <param name="defaultValue">Default value if parsing fails</param>
    /// <returns>Parsed boolean value</returns>
    private bool ParseBoolSetting(string value, bool defaultValue)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    /// <summary>
    /// Saves current settings to configuration file
    /// </summary>
    public void Save()
    {
        if (_disposed || _config == null)
        {
            return;
        }

        try
        {
            lock (_lock)
            {
                _config.AppSettings.Settings[DUPLEX_SELECT_MODE_KEY].Value = _duplexSelectModeOn.ToString();
                _config.AppSettings.Settings[HAS_COVER_KEY].Value = _hasCover.ToString();
                _config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving configuration: {ex.Message}");
        }
    }

    /// <summary>
    /// Resets all settings to their default values
    /// </summary>
    public void ResetToDefaults()
    {
        _duplexSelectModeOn = false;
        _hasCover = true;
        Save();
    }

    /// <summary>
    /// Validates current configuration state
    /// </summary>
    /// <returns>True if configuration is valid, false otherwise</returns>
    public bool ValidateConfiguration()
    {
        try
        {
            return _config != null && _config.AppSettings != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Disposes of resources used by the configuration
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            Save();
            _disposed = true;
        }
    }

    /// <summary>
    /// Finalizer to ensure resources are cleaned up
    /// </summary>
    ~AppConfig()
    {
        Dispose();
    }
}
