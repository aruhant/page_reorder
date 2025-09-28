using System;
using System.Configuration;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private SplitterPanelLayout? splitterPanelLayout = null;
        private readonly Debouncer debouncer = new Debouncer();

        private void saveLayout() => debouncer.Debounce(() => _SaveLayout());

        private void _SaveLayout()
        {
            if (splitterPanelLayout == null) return;
            Console.WriteLine("splitter panel layout: " + splitterPanelLayout);
            
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                
                if (config.AppSettings.Settings["SplitterPanelLayout"] == null)
                    config.AppSettings.Settings.Add("SplitterPanelLayout", splitterPanelLayout.ToString());
                else
                    config.AppSettings.Settings["SplitterPanelLayout"].Value = splitterPanelLayout.ToString();

                if (splitterPanelLayout == SplitterPanelLayout.Horizontal)
                {
                    UpdateOrAddSetting(config, mainSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (mainSplitContainer.SplitterDistance / (double)Height).ToString());
                    UpdateOrAddSetting(config, inSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (inSplitContainer.SplitterDistance / (double)Height).ToString());
                    UpdateOrAddSetting(config, outSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (outSplitContainer.SplitterDistance / (double)Height).ToString());
                }
                else
                {
                    UpdateOrAddSetting(config, mainSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (mainSplitContainer.SplitterDistance / (double)Width).ToString());
                    UpdateOrAddSetting(config, inSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (inSplitContainer.SplitterDistance / (double)Width).ToString());
                    UpdateOrAddSetting(config, outSplitContainer.Name + splitterPanelLayout.ToString(), 
                        (outSplitContainer.SplitterDistance / (double)Width).ToString());
                }
                
                printConfig(config);
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving layout: {ex.Message}");
            }
        }

        private void UpdateOrAddSetting(Configuration config, string key, string value)
        {
            if (config.AppSettings.Settings[key] == null)
                config.AppSettings.Settings.Add(key, value);
            else
                config.AppSettings.Settings[key].Value = value;
        }

        private void printConfig(Configuration config)
        {
            try
            {
                foreach (KeyValueConfigurationElement setting in config.AppSettings.Settings)
                {
                    Console.WriteLine("Key: " + setting.Key + " Value: " + setting.Value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error printing config: {ex.Message}");
            }
        }

        private void setupImageListViews(Manina.Windows.Forms.View view, [Optional] Size? size)
        {
            inImageListView.View = view;
            outImageListView.View = view;
            if (size != null)
            {
                inImageListView.ThumbnailSize = size.Value;
                outImageListView.ThumbnailSize = size.Value;
            }
        }

        private void loadLayout()
        {
            if (WindowState == FormWindowState.Minimized) return;
            
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                printConfig(config);
                
                if (splitterPanelLayout != null)
                {
                    setSplitterLayout(splitterPanelLayout.Value);
                }
                else
                {
                    SplitterPanelLayout layout = SplitterPanelLayout.Horizontal;
                    if (config.AppSettings.Settings["SplitterPanelLayout"]?.Value != null)
                    {
                        if (Enum.TryParse(config.AppSettings.Settings["SplitterPanelLayout"].Value, out SplitterPanelLayout parsedLayout))
                        {
                            layout = parsedLayout;
                        }
                    }
                    setSplitterLayout(layout);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading layout: {ex.Message}");
                setSplitterLayout(SplitterPanelLayout.Horizontal); // Fallback to default
            }
        }

        #region splitpane layouts
        private enum SplitterPanelLayout { Horizontal, Vertical, HorizontalThumbs, VerticalThumbs }
        
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
                case SplitterPanelLayout.Horizontal:
                    mainSplitContainer.Orientation = Orientation.Horizontal;
                    outSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    setupImageListViews(Manina.Windows.Forms.View.HorizontalStrip);
                    break;
                case SplitterPanelLayout.Vertical:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    setupImageListViews(Manina.Windows.Forms.View.VerticalStrip);
                    break;
                case SplitterPanelLayout.HorizontalThumbs:
                    mainSplitContainer.Orientation = Orientation.Horizontal;
                    outSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Panel1Collapsed = true;
                    outSplitContainer.Panel2Collapsed = true;
                    setupImageListViews(Manina.Windows.Forms.View.Thumbnails);
                    break;
                case SplitterPanelLayout.VerticalThumbs:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = true;
                    outSplitContainer.Panel2Collapsed = true;
                    setupImageListViews(Manina.Windows.Forms.View.Thumbnails);
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
            
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                UpdateOrAddSetting(config, "SplitterPanelLayout", layout.ToString());
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving layout setting: {ex.Message}");
            }
        }
        
        private void restoreSplitPaneLayout(SplitterPanelLayout layout, SplitContainer container)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string panelRatioString = config.AppSettings.Settings[container.Name + layout.ToString()]?.Value;
                double defaults = container == mainSplitContainer ? 0.5 : 0.25;
                double panelRatio = string.IsNullOrEmpty(panelRatioString) ? defaults : 
                    (double.TryParse(panelRatioString, out double parsed) ? parsed : defaults);
                
                if (panelRatio < 0 || panelRatio > 1) 
                    panelRatio = defaults;
                    
                container.SplitterDistance = (int)(layout == SplitterPanelLayout.Horizontal ? Height * panelRatio : Width * panelRatio);
                Console.WriteLine(container.Name + " --> " + panelRatio);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error restoring split pane layout: " + ex.Message);
            }
        }
        #endregion
    }
}