using System.Windows.Forms;

    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Threading;

    public class AppConfig
    {
        private static AppConfig _instance;
        private Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private bool DuplexSelectModeOn = false;
        private bool HasCover = true;        

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

            DuplexSelectModeOn =  bool.Parse(_config.AppSettings.Settings["DuplexSelectMode"].Value);
            HasCover =  bool.Parse(_config.AppSettings.Settings["HasCover"].Value);
            _instance = this;
        }

        public static AppConfig Instance { get { return _instance ?? (_instance = new AppConfig()); } }
        public bool enableDuplexSelectionMode { get { return DuplexSelectModeOn; } set { DuplexSelectModeOn = value; Save();  } }
        public bool enableDocumentWithCoverMode { get { return HasCover; } set { HasCover = value; Save(); } }
        public void Save()
        {
            _config.AppSettings.Settings["DuplexSelectMode"].Value = DuplexSelectModeOn.ToString();
            _config.AppSettings.Settings["HasCover"].Value = HasCover.ToString();
            _config.Save(ConfigurationSaveMode.Modified);
        }
        public void Dispose()
        {
            _config.Save(ConfigurationSaveMode.Modified);
        }
    }
