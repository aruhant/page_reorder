using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        #region private variables
        private string currentlyOpenPDFfile;
        private string currentlyOpenImageFolder;
        #endregion

        #region intialize properties
        public pageSorterForm()
        {
            InitializeComponent();

        }

        private void pageSorterForm_Load(object sender, EventArgs e)
        {
            setupImageListStyles(inImageListView);
            setupImageListStyles(inImageListView);
            Application.DoEvents();
            loadLayout();
        }

        #endregion

        #region imagelist event handlers and properties
        private void setupImageListStyles(ImageListView list)
        {
            list.AllowDrop = true;
            //list.DragEnter += new DragEventHandler(dragEnterHandler);
            //list.DragDrop += new DragEventHandler(dragDropHandler);
            //list.DragOver += new DragEventHandler(dragOverHandler);
            //list.DragLeave += new EventHandler(dragLeaveHandler);
        }


        private void dropComplete_Handler(object sender, DropCompleteEventArgs e)
        {
            ImageListView dropTarget = sender as ImageListView;
            ImageListView dragSource = sender == inImageListView ? outImageListView : inImageListView;
            foreach (ImageListViewItem item in dragSource.SelectedItems)
            {
                dragSource.Items.Remove(item);
                //dropTarget.Items.Remove(item);
            }
        }


        private void itemDragHandler(object sender, ItemDragEventArgs e)
        {
            ListView list = sender as ListView;
            ListViewItem[] selectedItems = list.SelectedItems.Cast<ListViewItem>().ToArray();
            DoDragDrop((items: selectedItems, source: sender), DragDropEffects.Move);
        }
        private void dragEnterHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }
        private void dragDropHandler(object sender, DragEventArgs e)
        {
            ListView list = sender as ListView;
            var draggedItems = ((ListViewItem[], object))e.Data.GetData(typeof((ListViewItem[], object)));

            int dropIndex = list.InsertionMark.Index > 0 ? list.InsertionMark.Index : 0;
            if (list.InsertionMark.AppearsAfterItem) dropIndex++;


            ListView sourceList = draggedItems.Item2 as ListView;
            foreach (ListViewItem draggedItem in draggedItems.Item1)
            {
                if (draggedItem.ListView == list) if (draggedItem.Index < dropIndex) dropIndex--;
                draggedItem.ListView.Items.Remove(draggedItem);
            }

            Console.WriteLine("Dropped to " + dropIndex);

            foreach (ListViewItem draggedItem in draggedItems.Item1)
            {
                list.Items.Insert(dropIndex, draggedItem);
                dropIndex++; // Increment the dropIndex after inserting the draggedItem
            }
        }
        // Moves the insertion mark as the item is dragged.
        private void dragOverHandler(object sender, DragEventArgs e)
        {
            ListView listView = sender as ListView;
            // Retrieve the client coordinates of the mouse pointer.
            Point targetPoint =
                listView.PointToClient(new Point(e.X, e.Y));

            // Retrieve the index of the item closest to the mouse pointer.
            int targetIndex = listView.InsertionMark.NearestIndex(targetPoint);

            // Confirm that the mouse pointer is not over the dragged item.
            if (targetIndex > -1)
            {
                // Determine whether the mouse pointer is to the left or
                // the right of the midpoint of the closest item and set
                // the InsertionMark.AppearsAfterItem property accordingly.
                Rectangle itemBounds = listView.GetItemRect(targetIndex);
                if (targetPoint.X > itemBounds.Left + (itemBounds.Width / 2))
                {
                    listView.InsertionMark.AppearsAfterItem = true;
                }
                else
                {
                    listView.InsertionMark.AppearsAfterItem = false;
                }
            }

            // Set the location of the insertion mark. If the mouse is
            // over the dragged item, the targetIndex value is -1 and
            // the insertion mark disappears.
            listView.InsertionMark.Index = targetIndex;
        }

        // Removes the insertion mark when the mouse leaves the control.
        private void dragLeaveHandler(object sender, EventArgs e)
        {
            ListView list = sender as ListView;
            list.InsertionMark.Index = -1;
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
                currentlyOpenPDFfile = openFileDialog.FileName;
                extractImages(currentlyOpenPDFfile);
                this.Text = System.IO.Path.GetFileName(currentlyOpenPDFfile);
            }
        }

        private void openFolder_Handler(object sender, EventArgs e)
        {
            // show folder open dialog to select a folder containing images
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select a folder containing images";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                currentlyOpenImageFolder = folderBrowserDialog.SelectedPath;
                loadImages(currentlyOpenImageFolder);
                currentlyOpenPDFfile = currentlyOpenImageFolder + ".pdf";
                this.Text = currentlyOpenImageFolder;
            }
        }

        private void loadImages(string inputFolder)
        {
            DirectoryInfo path = new DirectoryInfo(inputFolder);
            statusMessage.Text = "One moment....";
            Application.DoEvents();
            inImageListView.Items.Clear();
            outImageListView.Items.Clear();
            inImageListView.SuspendLayout();
            FileInfo[] files = new FileInfo[0];
            try
            {

                files = path.GetFiles("*.*");

            }
            catch
            {
                files = new FileInfo[0];
            }
            foreach (FileInfo p in files)
            {
                if (
                    p.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                    p.Name.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    // filename without extension
                    string title = Path.GetFileNameWithoutExtension(p.FullName);
                    ImageListViewItem item = new ImageListViewItem(p.FullName, title);
                    inImageListView.Items.Add(item);
                }
            }
            inImageListView.ResumeLayout();
            statusMessage.Text = "Ready...";
        }


        private void extractImages(string pdfFile)
        {
            // extract images from pdf file to input folder, and then load images to inList
            string inputFolder = "../../images/";
            System.IO.Directory.CreateDirectory(inputFolder);
            string pdfFileName = System.IO.Path.GetFileNameWithoutExtension(pdfFile);
            currentlyOpenImageFolder = inputFolder + pdfFileName + "/";
            // empty the output folder if it already exists else create it
            if (System.IO.Directory.Exists(currentlyOpenImageFolder))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(currentlyOpenImageFolder);
                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(currentlyOpenImageFolder);
            }
            extractImageFromPDF(pdfFile, currentlyOpenImageFolder);
            loadImages(currentlyOpenImageFolder);
        }

        public void extractImageFromPDF(string sourcePdf, string outputFolder)
        {
            PdfReader reader = new PdfReader(sourcePdf);

            try
            {
                PdfDocument document = new PdfDocument(reader);
                for (int i = 1; i <= document.GetNumberOfPages(); i++)
                {
                    statusMessage.Text = "Extracting image " + i;
                    Application.DoEvents();

                    PdfPage page = document.GetPage(i);
                    PdfResources resources = page.GetResources();
                    PdfDictionary xObjects = resources.GetResource(PdfName.XObject);
                    if (xObjects == null)
                    {
                        continue;
                    }
                    foreach (PdfName key in xObjects.KeySet())
                    {
                        PdfStream stream = (PdfStream)xObjects.GetAsStream(key);
                        PdfImageXObject image = new PdfImageXObject(stream);
                        using (MemoryStream ms = new MemoryStream(image.GetImageBytes()))
                        {
                            try
                            {
                                Image img = Image.FromStream(ms);
                                img.Save(outputFolder + ("000" + i).Substring(("000" + i).Length - 3) + ".jpg", ImageFormat.Jpeg);
                            }
                            catch (Exception)
                            {
                                //MessageBox.Show(e.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void exportPDF_Handler(object sender, EventArgs e)
        {
            saveImagesToPDF(outImageListView, currentlyOpenPDFfile);
        }
        private void saveImagesToPDF(ImageListView outImageListView, string inputPdf)
        {
            string outputPdf = System.IO.Path.GetDirectoryName(inputPdf) + "/Reordered - " + System.IO.Path.GetFileNameWithoutExtension(inputPdf) + ".pdf";
            using (PdfWriter writer = new PdfWriter(outputPdf))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf); // Create a Document instance
                    foreach (ImageListViewItem item in outImageListView.Items)
                    {
                        string path = Path.Combine(item.FilePath, item.FileName);
                        ImageData imageData = ImageDataFactory.Create(path);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData); // Use the correct Image class
                        doc.Add(image);
                    }
                }
            }
        }

        #endregion

        #region toolbox event handlers
        private void thumbnailsToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.Thumbnails, new Size(128, 128));

        }

        private void galleryToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.Gallery, new Size(128, 128));

        }

        private void paneToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.Pane, new Size(128, 128));
        }

        private void detailsToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.Details, new Size(128, 128));
        }

        private void horizontalStripToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.HorizontalStrip);
            int i = inImageListView.ClientSize.Height < inImageListView.ClientSize.Width ? inImageListView.ClientSize.Height : inImageListView.ClientSize.Width;
            int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
            inImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
            i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
            outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        }

        private void verticalStripToolStripButton_Click(object sender, EventArgs e)
        {
            setupImageListViews(Manina.Windows.Forms.View.VerticalStrip);
            int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
            int i = inImageListView.ClientSize.Height < inImageListView.ClientSize.Width ? inImageListView.ClientSize.Height : inImageListView.ClientSize.Width;
            inImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
            i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
            outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        }
        #endregion




        private void setOneBelowAnotherLayout(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.OneBelowAnother);

        private void setSideBySideLayout(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.SideBySide);

        private void setThumbnailLayout(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Thumbnails);


        private void mainFormResized(object sender, EventArgs e) => loadLayout();

        private void splitterMoved(object sender, SplitterEventArgs e) => saveLayout();

        private void updateInThumbnail(object sender, ItemHoverEventArgs e)
        {
            if (e.Item == null) return;
            string path = Path.Combine(e.Item.FilePath, e.Item.FileName);
            inPreview.Image = Image.FromFile(path);
        }


        private void updateOutThumbnail(object sender, ItemHoverEventArgs e)
        {
            if (e.Item == null) return;
            string path = Path.Combine(e.Item.FilePath, e.Item.FileName);
            outPreview.Image = Image.FromFile(path);

        }
    }
};
