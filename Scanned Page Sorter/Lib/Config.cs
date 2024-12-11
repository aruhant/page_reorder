using System.Windows.Forms;

    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Threading;

    public class AppConfig
    {
        private static AppConfig instance;

        public Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private bool duplexSelectMode = false;
        private bool hasCover = true;        

        private AppConfig()
        {
            if (_config.AppSettings.Settings["DuplexSelectMode"] == null)
            {
                _config.AppSettings.Settings.Add("DuplexSelectMode", "false");
            }
            if (_config.AppSettings.Settings["HasCover"] == null)
            {
                _config.AppSettings.Settings.Add("HasCover", "true");
            }

            duplexSelectMode =  bool.Parse(_config.AppSettings.Settings["DuplexSelectMode"].Value);
            hasCover =  bool.Parse(_config.AppSettings.Settings["HasCover"].Value);
            instance = this;
        }

        public static AppConfig Instance { get { return instance ?? (instance = new AppConfig()); } }
        public bool enableDuplexSelectionMode { get { return duplexSelectMode; } set { duplexSelectMode = value; Save();  } }
        public bool enableDocumentWithCoverMode { get { return hasCover; } set { hasCover = value; Save(); } }
        public void Save()
        {
            _config.AppSettings.Settings["DuplexSelectMode"].Value = duplexSelectMode.ToString();
            _config.AppSettings.Settings["HasCover"].Value = hasCover.ToString();
            _config.Save(ConfigurationSaveMode.Modified);
        }
        public void Dispose()
        {
            _config.Save(ConfigurationSaveMode.Modified);
        }
    }
