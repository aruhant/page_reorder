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
        private Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private bool duplexSelectMode = false;
        private bool hasCover = true;
        private AppConfig()
        {
            var str = config.AppSettings.Settings["DuplexSelectMode"];
                duplexSelectMode =  str == null ? false : bool.Parse(str.Value);
           str = config.AppSettings.Settings["HasCover"];
            hasCover = str == null ? true : bool.Parse(str.Value);
instance = this;
        }
        public static AppConfig Instance { get { return instance ?? (instance = new AppConfig()); } }
        public bool DuplexSelectMode { get { return duplexSelectMode; } set { duplexSelectMode = value; } }
        public bool HasCover { get { return hasCover; } set { hasCover = value; } }
        public void Save()
        {
            config.AppSettings.Settings["DuplexSelectMode"].Value = duplexSelectMode.ToString();
            config.AppSettings.Settings["HasCover"].Value = hasCover.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }
        public void Dispose()
        {
            config.Save(ConfigurationSaveMode.Modified);
        }
    }        
    }