using System.Configuration;

public class AppConfig
{
    private static AppConfig _instance;
    public readonly Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    private bool DuplexSelectModeOn = false;
    private bool HasCover = true;

    private AppConfig()
    {
        if (config.AppSettings.Settings["DuplexSelectMode"] == null)
        {
            config.AppSettings.Settings.Add("DuplexSelectMode", "false");
        }
        if (config.AppSettings.Settings["HasCover"] == null)
        {
            config.AppSettings.Settings.Add("HasCover", "true");
        }

        DuplexSelectModeOn = bool.Parse(config.AppSettings.Settings["DuplexSelectMode"].Value);
        HasCover = bool.Parse(config.AppSettings.Settings["HasCover"].Value);
        _instance = this;
    }

    public static AppConfig Instance { get { return _instance ?? (_instance = new AppConfig()); } }
    public bool enableDuplexSelectionMode { get { return DuplexSelectModeOn; } set { DuplexSelectModeOn = value; Save(); } }
    public bool enableDocumentWithCoverMode { get { return HasCover; } set { HasCover = value; Save(); } }
    public void Save()
    {
        config.AppSettings.Settings["DuplexSelectMode"].Value = DuplexSelectModeOn.ToString();
        config.AppSettings.Settings["HasCover"].Value = HasCover.ToString();
        config.Save(ConfigurationSaveMode.Modified);
    }
    public void Dispose()
    {
        config.Save(ConfigurationSaveMode.Modified);
    }
}
