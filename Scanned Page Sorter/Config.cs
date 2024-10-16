using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    using System;
    using System.Configuration;
    using System.Drawing;
    using System.Threading;

    public class AppConfig
    {
        private static AppConfig instance;
        public Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private bool duplexSelectMode = false;
        private bool hasCover = true;        

        private AppConfig()
        {
            if (Config.AppSettings.Settings["DuplexSelectMode"] == null)
            {
                Config.AppSettings.Settings.Add("DuplexSelectMode", "false");
            }
            if (Config.AppSettings.Settings["HasCover"] == null)
            {
                Config.AppSettings.Settings.Add("HasCover", "true");
            }

            duplexSelectMode =  bool.Parse(Config.AppSettings.Settings["DuplexSelectMode"].Value);
            hasCover =  bool.Parse(Config.AppSettings.Settings["HasCover"].Value);
            instance = this;
        }
        public static AppConfig Instance { get { return instance ?? (instance = new AppConfig()); } }
        public bool DuplexSelectMode { get { return duplexSelectMode; } set { duplexSelectMode = value; Save();  } }
        public bool HasCover { get { return hasCover; } set { hasCover = value; Save(); } }
        public void Save()
        {
            Config.AppSettings.Settings["DuplexSelectMode"].Value = duplexSelectMode.ToString();
            Config.AppSettings.Settings["HasCover"].Value = hasCover.ToString();
            Config.Save(ConfigurationSaveMode.Modified);
        }
        public void Dispose()
        {
            Config.Save(ConfigurationSaveMode.Modified);
        }
    }
}