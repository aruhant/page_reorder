using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Scanned_Page_Sorter.Lib.models;
using Scanned_Page_Sorter.Lib.PDF;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        #region private variables
        private SourceDocument sourceDocument;
        #endregion

        #region intialize properties
        public pageSorterForm() => InitializeComponent();


        private void pageSorterForm_Load(object sender, EventArgs e)
        {

            coverToggle.Checked = AppConfig.Instance.enableDocumentWithCoverMode;
            duplexToggle.Checked = AppConfig.Instance.enableDuplexSelectionMode;
            duplexToolStripMenuItem.Checked = AppConfig.Instance.enableDuplexSelectionMode;
            missingCoverToolStripMenuItem.Checked = !AppConfig.Instance.enableDocumentWithCoverMode;
            missingCoverToolStripMenuItem1.Checked = !AppConfig.Instance.enableDocumentWithCoverMode;
            Application.DoEvents();
            loadLayout();
        }

        #endregion

        #region imagelist event handlers and properties
        private void setupImageListStyles(ImageListView list)
        {
            list.SetRenderer(new ThumbnailRenderer(sourceDocument.PageMetadataMap));
            list.Font = new Font("Ariel", 6);
        }


        private void dropComplete_Handler(object sender, DropCompleteEventArgs e)
        {
            ImageListView dropTarget = sender as ImageListView;
            ImageListView dragSource = sender == inImageListView ? outImageListView : inImageListView;
            var selectedItems = new List<ImageListViewItem>();
            foreach (var d in dragSource.SelectedItems) selectedItems.Add(d);
            dragSource.ClearSelection();
            foreach (ImageListViewItem item in selectedItems)
            {
                dragSource.Items.Remove(item);
            }

            var tagList = new List<string>();
            foreach (var item in outImageListView.Items)
            {
                tagList.Add(item.Text.ToString());
            }

            sourceDocument.PageMetadataMap.SyncPageNumbers(tagList);
        }



        #endregion

        #region File and Folder event handlers


        private void openFile_Handler(object sender, EventArgs e)
        {
            // open a file dialog to select a pdf file.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";
            openFileDialog.Title = "Select a PDF File";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // load the pdf file to the inList
                sourceDocument = new SourceDocument(openFileDialog.FileName);
                extractImages(sourceDocument.SourcePath);
                setupImageListStyles(inImageListView);
                setupImageListStyles(outImageListView);
                Text = sourceDocument.Title;
            }
        }

        private void openFolder_Handler(object sender, EventArgs e)
        {
            OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                //currentlyOpenImageFolder = Path.GetDirectoryName(folderBrowser.FileName);
                //loadImages(currentlyOpenImageFolder);
                //currentlyOpenPDFfile = currentlyOpenImageFolder + ".pdf";
                //this.Text = currentlyOpenImageFolder;
                sourceDocument = new SourceDocument(Path.GetDirectoryName(folderBrowser.FileName));
                loadImages(sourceDocument.SourcePath);
                setupImageListStyles(inImageListView);
                setupImageListStyles(outImageListView);
                Text = sourceDocument.Title;

            }
        }


        private void extractImages(string pdfFile)
        {
            // extract images from pdf file to input folder, and then load images to inList
            string inputFolder = "../../images/";
            System.IO.Directory.CreateDirectory(inputFolder);
            string pdfFileName = System.IO.Path.GetFileNameWithoutExtension(pdfFile);
            string currentlyOpenImageFolder = inputFolder + pdfFileName + "/";
            // empty the output folder if it already exists else create it
            if (System.IO.Directory.Exists(currentlyOpenImageFolder))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(currentlyOpenImageFolder);
                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    try { file.Delete(); }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(currentlyOpenImageFolder);
            }
            PdfParser pdfParser = new PdfParser(sourceDocument.PageMetadataMap, pdfFile, currentlyOpenImageFolder);
            pdfParser.ExtractImages();
            loadImages(currentlyOpenImageFolder);
        }


        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void exportPDF_Handler(object sender, EventArgs e)
        {
            PdfExporter pdfExporter = new PdfExporter(outImageListView, sourceDocument.PDFSaveAs, sourceDocument.PageMetadataMap);
            CommentsExporter commentsExporter = new CommentsExporter(sourceDocument.TXTSaveAs, sourceDocument.PageMetadataMap);
            pdfExporter.export();
            commentsExporter.export();
        }





        #endregion

        #region toolbox event handlers

        private void setVerticalThumbs(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.VerticalThumbs);


        private void setHorizontalThumbs(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.HorizontalThumbs);

        private void setVertical(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Vertical);

        private void setHorizontal(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Horizontal);
        //{
        //    setupImageListViews(Manina.Windows.Forms.View.HorizontalStrip);
        //    int i = imageListView.ClientSize.Height < imageListView.ClientSize.Width ? imageListView.ClientSize.Height : imageListView.ClientSize.Width;
        //    int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
        //    imageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //    i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
        //    outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //}

        private void verticalStripToolStripButton_Click(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Vertical);
        //{
        //    setupImageListViews(Manina.Windows.Forms.View.VerticalStrip);
        //    int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
        //    int i = imageListView.ClientSize.Height < imageListView.ClientSize.Width ? imageListView.ClientSize.Height : imageListView.ClientSize.Width;
        //    imageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //    i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
        //    outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //}
        #endregion


        private void mainFormResized(object sender, EventArgs e) => loadLayout();

        private void splitterMoved(object sender, SplitterEventArgs e) => saveLayout();


        private void tooggleLayout_Click(object sender, EventArgs e)
        {
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            { rotateLayout(inImageListView, 90); updatePreview(inPreview, inImageListView.SelectedItems[0]); }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            { rotateLayout(outImageListView, 90); updatePreview(outPreview, outImageListView.SelectedItems[0]); }
        }

        private void duplexToggle_Click(object sender, EventArgs e)
        {
            AppConfig.Instance.enableDuplexSelectionMode = !AppConfig.Instance.enableDuplexSelectionMode;
            duplexToggle.Checked = AppConfig.Instance.enableDuplexSelectionMode;
            duplexToolStripMenuItem.Checked = AppConfig.Instance.enableDuplexSelectionMode;
        }

        private void coverToggle_Click(object sender, EventArgs e)
        {
            AppConfig.Instance.enableDocumentWithCoverMode = !AppConfig.Instance.enableDocumentWithCoverMode;
            coverToggle.Checked = AppConfig.Instance.enableDocumentWithCoverMode;
            missingCoverToolStripMenuItem.Checked = !AppConfig.Instance.enableDocumentWithCoverMode;
            missingCoverToolStripMenuItem1.Checked = !AppConfig.Instance.enableDocumentWithCoverMode;
        }

        private void inImageListView_SelectionChanged(object sender, EventArgs e)
        {
            if ((inImageListView.SelectedItems.Count > 0) && AppConfig.Instance.enableDuplexSelectionMode)
            {
                int coverNotSeen = 1;
                Console.WriteLine($"{coverNotSeen}");
                for (int i = 0; i < inImageListView.Items.Count; i++)
                {
                    if (i == 0 && AppConfig.Instance.enableDocumentWithCoverMode /*&& coverNotSeen !=0*/ && inImageListView.Items[i].Text == "000.jpg") { coverNotSeen = 0; if (AppConfig.Instance.enableDocumentWithCoverMode) continue; }
                    if (!inImageListView.Items[i].Selected) continue;
                    if (((coverNotSeen + i) % 2 == 1))
                    {
                        if (i < inImageListView.Items.Count - 1 && !inImageListView.Items[i + 1].Selected) inImageListView.Items[i + 1].Selected = true;
                        i++;
                    }
                    else if ((coverNotSeen + i) % 2 == 0)
                    {
                        if (i >= 1) inImageListView.Items[i - 1].Selected = true;
                    }
                }


            }

        }
        private void outImageListView_SelectionChanged(object sender, EventArgs e)
        {
            if ((outImageListView.SelectedItems.Count > 0) && AppConfig.Instance.enableDuplexSelectionMode)
            {
                int coverNotSeen = 1;
                Console.WriteLine($"{coverNotSeen}");
                for (int i = 0; i < outImageListView.Items.Count; i++)
                {
                    if (i == 0 && AppConfig.Instance.enableDocumentWithCoverMode /*&& coverNotSeen !=0*/ && outImageListView.Items[i].Text == "000.jpg") { coverNotSeen = 0; if (AppConfig.Instance.enableDocumentWithCoverMode) continue; }
                    if (!outImageListView.Items[i].Selected) continue;
                    if (((coverNotSeen + i) % 2 == 1))
                    {
                        if (i < outImageListView.Items.Count - 1 && !outImageListView.Items[i + 1].Selected) outImageListView.Items[i + 1].Selected = true;
                        i++;
                    }
                    else if ((coverNotSeen + i) % 2 == 0)
                    {
                        if (i >= 1) outImageListView.Items[i - 1].Selected = true;
                    }
                }
            }
        }


    }
}