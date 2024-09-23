using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private SplitterPanelLayout? splitterPanelLayout = null;
        private void saveLayout()        {
            if (splitterPanelLayout == null) return;
            ConfigurationManager.AppSettings.Set("SplitterPanelLayout", splitterPanelLayout.ToString());
            if (splitterPanelLayout == SplitterPanelLayout.OnTop)
            {
                ConfigurationManager.AppSettings.Set(mainSplitContainer.Name + splitterPanelLayout.ToString(), mainSplitContainer.SplitterDistance / (double)this.Height + "");
                ConfigurationManager.AppSettings.Set(inSplitContainer.Name + splitterPanelLayout.ToString(), inSplitContainer.SplitterDistance / (double)this.Height + "");
                ConfigurationManager.AppSettings.Set(outSplitContainer.Name + splitterPanelLayout.ToString(), outSplitContainer.SplitterDistance / (double)this.Height + "");
            }
            else
            {
                ConfigurationManager.AppSettings.Set(mainSplitContainer.Name + splitterPanelLayout.ToString(), mainSplitContainer.SplitterDistance / (double)this.Width + "");
                ConfigurationManager.AppSettings.Set(inSplitContainer.Name + splitterPanelLayout.ToString(), inSplitContainer.SplitterDistance / (double)this.Width + "");
                ConfigurationManager.AppSettings.Set(outSplitContainer.Name + splitterPanelLayout.ToString(), outSplitContainer.SplitterDistance / (double)this.Width + "");

            }

        }

        private void loadLayout()
        {

            SplitterPanelLayout splitterPanelLayout =
             (ConfigurationManager.AppSettings.Get("SplitterPanelLayout") == null) ? splitterPanelLayout = SplitterPanelLayout.SideByside : splitterPanelLayout = (SplitterPanelLayout)Enum.Parse(typeof(SplitterPanelLayout), ConfigurationManager.AppSettings.Get("SplitterPanelLayout"));
            setSplitterLayout(splitterPanelLayout);
        }
        #region splitpane layouts
        private enum SplitterPanelLayout { SideByside, OnTop, None }
        private void setSplitterLayout(SplitterPanelLayout layout)
        {
            saveLayout();
            splitterPanelLayout = layout;
            mainSplitContainer.SuspendLayout();
            inSplitContainer.SuspendLayout();
            outSplitContainer.SuspendLayout();
            switch (layout)
            {
                case SplitterPanelLayout.SideByside:
                    mainSplitContainer.Orientation = Orientation.Horizontal;
                    outSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Orientation = Orientation.Horizontal;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    break;
                case SplitterPanelLayout.OnTop:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = false;
                    outSplitContainer.Panel2Collapsed = false;
                    break;
                case SplitterPanelLayout.None:
                    mainSplitContainer.Orientation = Orientation.Vertical;
                    outSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Orientation = Orientation.Vertical;
                    inSplitContainer.Panel1Collapsed = true;
                    outSplitContainer.Panel2Collapsed = true;

                    break;
            }
            restoreSplitPaneLayout(layout, mainSplitContainer);
            restoreSplitPaneLayout(layout, inSplitContainer);
            restoreSplitPaneLayout(layout, outSplitContainer);

            mainMenu.ResumeLayout();
            inSplitContainer.ResumeLayout();
            outSplitContainer.ResumeLayout();
        }
        private void restoreSplitPaneLayout(SplitterPanelLayout layout, SplitContainer container)
        {
            string panelRatioString = ConfigurationManager.AppSettings.Get(container.Name + layout.ToString());
            double panelRatio = string.IsNullOrEmpty(panelRatioString) ? 0.25 : double.Parse(panelRatioString);            
            if(panelRatio < 0 || panelRatio > 1) panelRatio = 0.25;
            container.SplitterDistance = (int)( layout == SplitterPanelLayout.OnTop ? this.Height * panelRatio : this.Width * panelRatio);
        }

        #endregion
    }
}