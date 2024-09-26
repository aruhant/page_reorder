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

using Manina.Windows.Forms.ImageListViewRenderers;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        #region private variables
        private string currentlyOpenPDFfile;
        private string currentlyOpenImageFolder;
        #endregion

        #region intialize properties
        public pageSorterForm() =>    InitializeComponent();

        

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
            list.SetRenderer(new ThumbnailRenderer());
        }


        private void dropComplete_Handler(object sender, DropCompleteEventArgs e)
        {
            ImageListView dropTarget = sender as ImageListView;
            ImageListView dragSource = sender == inImageListView ? outImageListView : inImageListView;
            foreach (ImageListViewItem item in dragSource.SelectedItems)
            {
                dragSource.Items.Remove(item);
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
                currentlyOpenImageFolder = Path.GetDirectoryName(folderBrowser.FileName);
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

        // Replace the following method in the pageSorterForm class
        public void extractImageFromPDF(string sourcePdf, string outputFolder)
        {
            PdfReader reader = new PdfReader(sourcePdf);

            try
            {
                PdfDocument pdfDoc = new PdfDocument(reader);
                PdfCanvasProcessor parser = new PdfCanvasProcessor(new ImageRenderListener(outputFolder));
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    parser.ProcessPageContent(pdfDoc.GetPage(i));
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
        
        private void setVerticalThumbs(object sender, EventArgs e)        =>            setSplitterLayout(SplitterPanelLayout.VerticalThumbs);
        

        private void setHorizontalThumbs(object sender, EventArgs e)        => setSplitterLayout(SplitterPanelLayout.HorizontalThumbs); 

        private void setVertical(object sender, EventArgs e)=>        setSplitterLayout(SplitterPanelLayout.Vertical);

        private void setHorizontal(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Horizontal);
        //{
        //    setupImageListViews(Manina.Windows.Forms.View.HorizontalStrip);
        //    int i = inImageListView.ClientSize.Height < inImageListView.ClientSize.Width ? inImageListView.ClientSize.Height : inImageListView.ClientSize.Width;
        //    int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
        //    inImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //    i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
        //    outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //}

        private void verticalStripToolStripButton_Click(object sender, EventArgs e) => setSplitterLayout(SplitterPanelLayout.Vertical);
        //{
        //    setupImageListViews(Manina.Windows.Forms.View.VerticalStrip);
        //    int scrollbarWidth = SystemInformation.VerticalScrollBarWidth + 32;
        //    int i = inImageListView.ClientSize.Height < inImageListView.ClientSize.Width ? inImageListView.ClientSize.Height : inImageListView.ClientSize.Width;
        //    inImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //    i = outImageListView.ClientSize.Height < outImageListView.ClientSize.Width ? outImageListView.ClientSize.Height : outImageListView.ClientSize.Width;
        //    outImageListView.ThumbnailSize = new Size(i - scrollbarWidth, i - scrollbarWidth);
        //}
        #endregion



 


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

        private void rotateLeft_Click(object sender, EventArgs e)
        {
            rotate(inImageListView , -10);
        }

        private void rotateRight_Click(object sender, EventArgs e)
        {
            rotate(inImageListView, 10);
        }

        private void rotate(ImageListView inImageListView, int angle)
        {
            for (int i = 0; i < inImageListView.SelectedItems.Count; i++)
            {
                ImageListViewItem item = inImageListView.SelectedItems[i];
                int a = item.Tag == null ? 0 : Convert.ToInt32(item.Tag);
                item.Tag = a + angle;
                Console.WriteLine("Rotating + " + item.FileName);
            }
        }
         
    }


  public class ImageRenderListener : IEventListener
    {
    static int i =0;
        private readonly string outputFolder;

        public ImageRenderListener(string outputFolder)
        {
            this.outputFolder = outputFolder;
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            if (type == EventType.RENDER_IMAGE)
            {
                var renderInfo = (iText.Kernel.Pdf.Canvas.Parser.Data.ImageRenderInfo)data;
                PdfImageXObject image = renderInfo.GetImage();
                var imageBytes = image.GetImageBytes(true);
                var croppedImage = CropToBounds(image, new Rectangle((int)renderInfo.GetImageCtm().Get(6), (int)renderInfo.GetImageCtm().Get(7), (int)renderInfo.GetImageCtm().Get(0), (int)renderInfo.GetImageCtm().Get(4)));
                var fileName = Path.Combine(outputFolder, $"{i++:D3}.jpg");
                croppedImage.Save(fileName, ImageFormat.Png);
            }
        }

        public ICollection<EventType> GetSupportedEvents()
        {
            return new HashSet<EventType> { EventType.RENDER_IMAGE };
        }

        private System.Drawing.Image CropToBounds(PdfImageXObject img, Rectangle cropRect)
        {
            using (MemoryStream ms = new MemoryStream(img.GetImageBytes(true)))
            {
                Bitmap src = new Bitmap(ms);
                Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                }

                return target;
            }
        }

    }

};
