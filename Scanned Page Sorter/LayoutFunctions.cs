using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private SplitterPanelLayout? splitterPanelLayout = null;
        private Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private Debouncer debouncer = new Debouncer();


        private void saveLayout() => debouncer.Debounce(() => _SaveLayout());

        private void _SaveLayout()
        {
            Console.WriteLine("**********************Saving ");
            if (splitterPanelLayout == null) return;
            Console.WriteLine("splitter panel layout: " + splitterPanelLayout);
            config.AppSettings.Settings["SplitterPanelLayout"].Value = splitterPanelLayout.ToString();
            if (splitterPanelLayout == SplitterPanelLayout.OneBelowAnother)
            {
                config.AppSettings.Settings[mainSplitContainer.Name + splitterPanelLayout.ToString()].Value = mainSplitContainer.SplitterDistance / (double)this.Height + "";
                config.AppSettings.Settings[inSplitContainer.Name + splitterPanelLayout.ToString()].Value = inSplitContainer.SplitterDistance / (double)this.Height + "";
                config.AppSettings.Settings[outSplitContainer.Name + splitterPanelLayout.ToString()].Value = outSplitContainer.SplitterDistance / (double)this.Height + "";
            }
            else
            {
                config.AppSettings.Settings[mainSplitContainer.Name + splitterPanelLayout.ToString()].Value = mainSplitContainer.SplitterDistance / (double)this.Width + "";
                config.AppSettings.Settings[inSplitContainer.Name + splitterPanelLayout.ToString()].Value = inSplitContainer.SplitterDistance / (double)this.Width + "";
                config.AppSettings.Settings[outSplitContainer.Name + splitterPanelLayout.ToString()].Value = outSplitContainer.SplitterDistance / (double)this.Width + "";
            }
            printConfig();
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void printConfig()
        {
            foreach (KeyValueConfigurationElement setting in config.AppSettings.Settings)
            {
                Console.WriteLine("Key: " + setting.Key + " Value: " + setting.Value);
            }
        }



        private void loadLayout()
        {
            printConfig();
            if (splitterPanelLayout != null)
            {
                setSplitterLayout(splitterPanelLayout.Value);
            }
            else
            {
                SplitterPanelLayout layout = (config.AppSettings.Settings["SplitterPanelLayout"] == null) ? SplitterPanelLayout.OneBelowAnother : (SplitterPanelLayout)Enum.Parse(typeof(SplitterPanelLayout), config.AppSettings.Settings["SplitterPanelLayout"].Value);
                setSplitterLayout(layout);
            }
        }
        #region splitpane layouts
        private enum SplitterPanelLayout { OneBelowAnother, SideBySide, Thumbnails }
        private void setSplitterLayout(SplitterPanelLayout layout)
        {
            splitterPanelLayout = layout;
            mainSplitContainer.SuspendLayout();
            inSplitContainer.SuspendLayout();
            outSplitContainer.SuspendLayout();
            mainSplitContainer.SplitterDistance = 26;
            inSplitContainer.Panel1Collapsed = true;
            outSplitContainer.Panel2Collapsed = true;
            switch (layout)
            {
                case SplitterPanelLayout.OneBelowAnother:
                    mainSplitContainer.Orientation = Orientation.Horizontal;
                    outSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    break;
                case SplitterPanelLayout.SideBySide:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    break;
                case SplitterPanelLayout.Thumbnails:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = true;
                    outSplitContainer.Panel2Collapsed = true;

                    break;
            }

            mainSplitContainer.ResumeLayout();
            inSplitContainer.ResumeLayout();
            outSplitContainer.ResumeLayout();
            mainSplitContainer.PerformLayout();
            inSplitContainer.PerformLayout();
            outSplitContainer.PerformLayout();

            Application.DoEvents();
            restoreSplitPaneLayout(layout, mainSplitContainer);
            restoreSplitPaneLayout(layout, inSplitContainer);
            restoreSplitPaneLayout(layout, outSplitContainer);
            config.AppSettings.Settings["SplitterPanelLayout"].Value = layout.ToString();
            config.Save(ConfigurationSaveMode.Modified);
        }
        private void restoreSplitPaneLayout(SplitterPanelLayout layout, SplitContainer container)
        {
            string panelRatioString = config.AppSettings.Settings[container.Name + layout.ToString()]?.Value;
            double defaults = container == mainSplitContainer ? 0.5 : 0.25;
            double panelRatio = string.IsNullOrEmpty(panelRatioString) ? defaults : double.Parse(panelRatioString);
            if (panelRatio < 0 || panelRatio > 1) panelRatio = defaults;
            container.SplitterDistance = (int)(layout == SplitterPanelLayout.OneBelowAnother ? this.Height * panelRatio : this.Width * panelRatio);
            Console.WriteLine(container.Name + " -- > " + panelRatio);
        }

        #endregion
    }
}