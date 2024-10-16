namespace Scanned_Page_Sorter
{
    partial class pageSorterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(pageSorterForm));
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.topToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.rightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.leftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.inSplitContainer = new System.Windows.Forms.SplitContainer();
            this.inPreview = new System.Windows.Forms.PictureBox();
            this.commentsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.blurredImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.missingCoverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.missingPageToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.previousPageMissingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inImageListView = new Manina.Windows.Forms.ImageListView();
            this.outSplitContainer = new System.Windows.Forms.SplitContainer();
            this.outImageListView = new Manina.Windows.Forms.ImageListView();
            this.outPreview = new System.Windows.Forms.PictureBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.openFileToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.openFolderToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.exportToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.rendererToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.verticalTButton = new System.Windows.Forms.ToolStripButton();
            this.horizontalTButton = new System.Windows.Forms.ToolStripButton();
            this.verticalButton = new System.Windows.Forms.ToolStripButton();
            this.horizontalButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.clearThumbsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tooggleLayout = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rotateLeft = new System.Windows.Forms.ToolStripButton();
            this.rotateRight = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.duplexToggle = new System.Windows.Forms.ToolStripButton();
            this.coverToggle = new System.Windows.Forms.ToolStripButton();
            this.mainMenu.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inSplitContainer)).BeginInit();
            this.inSplitContainer.Panel1.SuspendLayout();
            this.inSplitContainer.Panel2.SuspendLayout();
            this.inSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inPreview)).BeginInit();
            this.commentsContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outSplitContainer)).BeginInit();
            this.outSplitContainer.Panel1.SuspendLayout();
            this.outSplitContainer.Panel2.SuspendLayout();
            this.outSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outPreview)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mainMenu.Size = new System.Drawing.Size(1333, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem,
            this.openFolderToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exitMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openMenuItem
            // 
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openMenuItem.Text = "&Open File";
            this.openMenuItem.Click += new System.EventHandler(this.openFile_Handler);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.openFolderToolStripMenuItem.Text = "Open Fol&der";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolder_Handler);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.exportPDF_Handler);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(139, 22);
            this.exitMenuItem.Text = "&Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // topToolStripPanel
            // 
            this.topToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.topToolStripPanel.Name = "topToolStripPanel";
            this.topToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.topToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.topToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // rightToolStripPanel
            // 
            this.rightToolStripPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.rightToolStripPanel.Name = "rightToolStripPanel";
            this.rightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.rightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.rightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // leftToolStripPanel
            // 
            this.leftToolStripPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.leftToolStripPanel.Name = "leftToolStripPanel";
            this.leftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.leftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.leftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.mainSplitContainer);
            this.toolStripContainer1.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1333, 570);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 24);
            this.toolStripContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1333, 617);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMessage});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1333, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // statusMessage
            // 
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(39, 17);
            this.statusMessage.Text = "Ready";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.inSplitContainer);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.outSplitContainer);
            this.mainSplitContainer.Size = new System.Drawing.Size(1333, 570);
            this.mainSplitContainer.SplitterDistance = 306;
            this.mainSplitContainer.SplitterWidth = 5;
            this.mainSplitContainer.TabIndex = 2;
            this.mainSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterMoved);
            // 
            // inSplitContainer
            // 
            this.inSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.inSplitContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.inSplitContainer.Name = "inSplitContainer";
            this.inSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // inSplitContainer.Panel1
            // 
            this.inSplitContainer.Panel1.Controls.Add(this.inPreview);
            // 
            // inSplitContainer.Panel2
            // 
            this.inSplitContainer.Panel2.Controls.Add(this.inImageListView);
            this.inSplitContainer.Size = new System.Drawing.Size(1333, 306);
            this.inSplitContainer.SplitterDistance = 159;
            this.inSplitContainer.SplitterWidth = 5;
            this.inSplitContainer.TabIndex = 0;
            this.inSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterMoved);
            // 
            // inPreview
            // 
            this.inPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inPreview.ContextMenuStrip = this.commentsContextMenu;
            this.inPreview.Location = new System.Drawing.Point(0, 0);
            this.inPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.inPreview.Name = "inPreview";
            this.inPreview.Size = new System.Drawing.Size(1331, 157);
            this.inPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.inPreview.TabIndex = 0;
            this.inPreview.TabStop = false;
            // 
            // commentsContextMenu
            // 
            this.commentsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blurredImageToolStripMenuItem,
            this.commentToolStripMenuItem,
            this.missingCoverToolStripMenuItem,
            this.missingPageToolStripMenuItem1,
            this.previousPageMissingToolStripMenuItem});
            this.commentsContextMenu.Name = "contextMenuStrip1";
            this.commentsContextMenu.Size = new System.Drawing.Size(193, 114);
            // 
            // blurredImageToolStripMenuItem
            // 
            this.blurredImageToolStripMenuItem.Name = "blurredImageToolStripMenuItem";
            this.blurredImageToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.blurredImageToolStripMenuItem.Text = "&Blurred Image";
            this.blurredImageToolStripMenuItem.Click += new System.EventHandler(this.commentsContextMenuItem_Click);
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.commentToolStripMenuItem.Text = "&Comment...";
            this.commentToolStripMenuItem.Click += new System.EventHandler(this.commentsContextMenuItem_Click);
            // 
            // missingCoverToolStripMenuItem
            // 
            this.missingCoverToolStripMenuItem.Name = "missingCoverToolStripMenuItem";
            this.missingCoverToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.missingCoverToolStripMenuItem.Text = "M&issing Cover";
            this.missingCoverToolStripMenuItem.Click += new System.EventHandler(this.commentsContextMenuItem_Click);
            // 
            // missingPageToolStripMenuItem1
            // 
            this.missingPageToolStripMenuItem1.Name = "missingPageToolStripMenuItem1";
            this.missingPageToolStripMenuItem1.Size = new System.Drawing.Size(192, 22);
            this.missingPageToolStripMenuItem1.Text = "&Next Page Missing";
            this.missingPageToolStripMenuItem1.Click += new System.EventHandler(this.commentsContextMenuItem_Click);
            // 
            // previousPageMissingToolStripMenuItem
            // 
            this.previousPageMissingToolStripMenuItem.Name = "previousPageMissingToolStripMenuItem";
            this.previousPageMissingToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.previousPageMissingToolStripMenuItem.Text = "&Previous Page Missing";
            this.previousPageMissingToolStripMenuItem.Click += new System.EventHandler(this.commentsContextMenuItem_Click);
            // 
            // inImageListView
            // 
            this.inImageListView.AllowDrag = true;
            this.inImageListView.AllowDrop = true;
            this.inImageListView.AutoRotateThumbnails = false;
            this.inImageListView.CacheLimit = "0";
            this.inImageListView.CheckBoxAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.inImageListView.ContextMenuStrip = this.commentsContextMenu;
            this.inImageListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inImageListView.Location = new System.Drawing.Point(0, 0);
            this.inImageListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.inImageListView.Name = "inImageListView";
            this.inImageListView.PersistentCacheDirectory = "";
            this.inImageListView.PersistentCacheSize = ((long)(0));
            this.inImageListView.Size = new System.Drawing.Size(1333, 142);
            this.inImageListView.TabIndex = 3;
            this.inImageListView.UseWIC = true;
            this.inImageListView.DropComplete += new Manina.Windows.Forms.DropCompleteEventHandler(this.dropComplete_Handler);
            this.inImageListView.ItemHover += new Manina.Windows.Forms.ItemHoverEventHandler(this.updateInPreview);
            // 
            // outSplitContainer
            // 
            this.outSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.outSplitContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.outSplitContainer.Name = "outSplitContainer";
            this.outSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // outSplitContainer.Panel1
            // 
            this.outSplitContainer.Panel1.Controls.Add(this.outImageListView);
            // 
            // outSplitContainer.Panel2
            // 
            this.outSplitContainer.Panel2.Controls.Add(this.outPreview);
            this.outSplitContainer.Size = new System.Drawing.Size(1333, 259);
            this.outSplitContainer.SplitterDistance = 52;
            this.outSplitContainer.SplitterWidth = 5;
            this.outSplitContainer.TabIndex = 0;
            this.outSplitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitterMoved);
            // 
            // outImageListView
            // 
            this.outImageListView.AllowDrag = true;
            this.outImageListView.AllowDrop = true;
            this.outImageListView.CheckBoxAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.outImageListView.ContextMenuStrip = this.commentsContextMenu;
            this.outImageListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outImageListView.Location = new System.Drawing.Point(0, 0);
            this.outImageListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.outImageListView.Name = "outImageListView";
            this.outImageListView.PersistentCacheDirectory = "";
            this.outImageListView.PersistentCacheSize = ((long)(100));
            this.outImageListView.Size = new System.Drawing.Size(1333, 52);
            this.outImageListView.TabIndex = 3;
            this.outImageListView.UseWIC = true;
            this.outImageListView.DropComplete += new Manina.Windows.Forms.DropCompleteEventHandler(this.dropComplete_Handler);
            this.outImageListView.ItemHover += new Manina.Windows.Forms.ItemHoverEventHandler(this.updateOutPreview);
            // 
            // outPreview
            // 
            this.outPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outPreview.ContextMenuStrip = this.commentsContextMenu;
            this.outPreview.Location = new System.Drawing.Point(3, 0);
            this.outPreview.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.outPreview.Name = "outPreview";
            this.outPreview.Size = new System.Drawing.Size(1331, 202);
            this.outPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.outPreview.TabIndex = 0;
            this.outPreview.TabStop = false;
            // 
            // toolStrip
            // 
            this.toolStrip.AllowDrop = true;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.openFileToolStripButton,
            this.openFolderToolStripButton,
            this.exportToolStripButton,
            this.toolStripSeparator2,
            this.rendererToolStripLabel,
            this.verticalTButton,
            this.horizontalTButton,
            this.verticalButton,
            this.horizontalButton,
            this.toolStripSeparator3,
            this.clearThumbsToolStripButton,
            this.toolStripSeparator4,
            this.tooggleLayout,
            this.toolStripSeparator1,
            this.rotateLeft,
            this.rotateRight,
            this.toolStripSeparator5,
            this.duplexToggle,
            this.coverToggle});
            this.toolStrip.Location = new System.Drawing.Point(3, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(446, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel1.Text = "Open:";
            // 
            // openFileToolStripButton
            // 
            this.openFileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFileToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openFileToolStripButton.Image")));
            this.openFileToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFileToolStripButton.Name = "openFileToolStripButton";
            this.openFileToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openFileToolStripButton.Text = "File";
            this.openFileToolStripButton.Click += new System.EventHandler(this.openFile_Handler);
            // 
            // openFolderToolStripButton
            // 
            this.openFolderToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFolderToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openFolderToolStripButton.Image")));
            this.openFolderToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFolderToolStripButton.Name = "openFolderToolStripButton";
            this.openFolderToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.openFolderToolStripButton.Text = "Folder";
            this.openFolderToolStripButton.Click += new System.EventHandler(this.openFolder_Handler);
            // 
            // exportToolStripButton
            // 
            this.exportToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("exportToolStripButton.Image")));
            this.exportToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportToolStripButton.Name = "exportToolStripButton";
            this.exportToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.exportToolStripButton.Text = "Folder";
            this.exportToolStripButton.Click += new System.EventHandler(this.exportPDF_Handler);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // rendererToolStripLabel
            // 
            this.rendererToolStripLabel.Name = "rendererToolStripLabel";
            this.rendererToolStripLabel.Size = new System.Drawing.Size(35, 22);
            this.rendererToolStripLabel.Text = "View:";
            // 
            // verticalTButton
            // 
            this.verticalTButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.verticalTButton.Image = ((System.Drawing.Image)(resources.GetObject("verticalTButton.Image")));
            this.verticalTButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.verticalTButton.Name = "verticalTButton";
            this.verticalTButton.Size = new System.Drawing.Size(23, 22);
            this.verticalTButton.Text = "verticalNoPreview";
            this.verticalTButton.Click += new System.EventHandler(this.setVerticalThumbs);
            // 
            // horizontalTButton
            // 
            this.horizontalTButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.horizontalTButton.Image = ((System.Drawing.Image)(resources.GetObject("horizontalTButton.Image")));
            this.horizontalTButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.horizontalTButton.Name = "horizontalTButton";
            this.horizontalTButton.Size = new System.Drawing.Size(23, 22);
            this.horizontalTButton.Text = "horizontalNoPreview";
            this.horizontalTButton.Click += new System.EventHandler(this.setHorizontalThumbs);
            // 
            // verticalButton
            // 
            this.verticalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.verticalButton.Image = ((System.Drawing.Image)(resources.GetObject("verticalButton.Image")));
            this.verticalButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.verticalButton.Name = "verticalButton";
            this.verticalButton.Size = new System.Drawing.Size(23, 22);
            this.verticalButton.Text = "Vertical";
            this.verticalButton.Click += new System.EventHandler(this.setVertical);
            // 
            // horizontalButton
            // 
            this.horizontalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.horizontalButton.Image = ((System.Drawing.Image)(resources.GetObject("horizontalButton.Image")));
            this.horizontalButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.horizontalButton.Name = "horizontalButton";
            this.horizontalButton.Size = new System.Drawing.Size(23, 22);
            this.horizontalButton.Text = "Horizontal";
            this.horizontalButton.Click += new System.EventHandler(this.setHorizontal);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // clearThumbsToolStripButton
            // 
            this.clearThumbsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearThumbsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("clearThumbsToolStripButton.Image")));
            this.clearThumbsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearThumbsToolStripButton.Name = "clearThumbsToolStripButton";
            this.clearThumbsToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.clearThumbsToolStripButton.Text = "Clear Thumbnail Cache";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tooggleLayout
            // 
            this.tooggleLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tooggleLayout.Image = global::Scanned_Page_Sorter.Properties.Resources._85194_200;
            this.tooggleLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tooggleLayout.Name = "tooggleLayout";
            this.tooggleLayout.Size = new System.Drawing.Size(23, 22);
            this.tooggleLayout.Text = "rotateLeft";
            this.tooggleLayout.Click += new System.EventHandler(this.tooggleLayout_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // rotateLeft
            // 
            this.rotateLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateLeft.Image = ((System.Drawing.Image)(resources.GetObject("rotateLeft.Image")));
            this.rotateLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateLeft.Name = "rotateLeft";
            this.rotateLeft.Size = new System.Drawing.Size(23, 22);
            this.rotateLeft.Text = "rotateLeft";
            this.rotateLeft.Click += new System.EventHandler(this.rotateLeft_Click);
            // 
            // rotateRight
            // 
            this.rotateRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateRight.Image = ((System.Drawing.Image)(resources.GetObject("rotateRight.Image")));
            this.rotateRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateRight.Name = "rotateRight";
            this.rotateRight.Size = new System.Drawing.Size(23, 22);
            this.rotateRight.Text = "rotateRight";
            this.rotateRight.Click += new System.EventHandler(this.rotateRight_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // duplexToggle
            // 
            this.duplexToggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.duplexToggle.Image = ((System.Drawing.Image)(resources.GetObject("duplexToggle.Image")));
            this.duplexToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.duplexToggle.Name = "duplexToggle";
            this.duplexToggle.Size = new System.Drawing.Size(23, 22);
            this.duplexToggle.Text = "Toggle Duplex Mode";
            this.duplexToggle.Click += new System.EventHandler(this.duplexToggle_Click);
            // 
            // coverToggle
            // 
            this.coverToggle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.coverToggle.Image = ((System.Drawing.Image)(resources.GetObject("coverToggle.Image")));
            this.coverToggle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.coverToggle.Name = "coverToggle";
            this.coverToggle.Size = new System.Drawing.Size(23, 22);
            this.coverToggle.Text = "Toggle Missing Cover";
            this.coverToggle.Click += new System.EventHandler(this.coverToggle_Click);
            // 
            // pageSorterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 641);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.mainMenu);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "pageSorterForm";
            this.Text = "Page Sorter";
            this.Load += new System.EventHandler(this.pageSorterForm_Load);
            this.Resize += new System.EventHandler(this.mainFormResized);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.inSplitContainer.Panel1.ResumeLayout(false);
            this.inSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inSplitContainer)).EndInit();
            this.inSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inPreview)).EndInit();
            this.commentsContextMenu.ResumeLayout(false);
            this.outSplitContainer.Panel1.ResumeLayout(false);
            this.outSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outSplitContainer)).EndInit();
            this.outSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outPreview)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel topToolStripPanel;
        private System.Windows.Forms.ToolStripPanel rightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel leftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusMessage;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel rendererToolStripLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton verticalTButton;
        private System.Windows.Forms.ToolStripButton horizontalTButton;
        private System.Windows.Forms.ToolStripButton verticalButton;
        private System.Windows.Forms.ToolStripButton horizontalButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton clearThumbsToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton openFileToolStripButton;
        private System.Windows.Forms.ToolStripButton openFolderToolStripButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton exportToolStripButton;
        private System.Windows.Forms.SplitContainer inSplitContainer;
        private Manina.Windows.Forms.ImageListView inImageListView;
        private System.Windows.Forms.SplitContainer outSplitContainer;
        private Manina.Windows.Forms.ImageListView outImageListView;
        private System.Windows.Forms.PictureBox inPreview;
        private System.Windows.Forms.PictureBox outPreview;
        private System.Windows.Forms.ToolStripButton rotateLeft;
        private System.Windows.Forms.ToolStripButton rotateRight;
        private System.Windows.Forms.ContextMenuStrip commentsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem blurredImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem missingCoverToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tooggleLayout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem missingPageToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem previousPageMissingToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton duplexToggle;
        private System.Windows.Forms.ToolStripButton coverToggle;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}

