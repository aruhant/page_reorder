using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Font;
using Manina.Windows.Forms;
using Manina.Windows.Forms.ImageListViewRenderers;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        #region private variables
        private string currentlyOpenPDFfile;
        private string currentlyOpenImageFolder;
        private ImageMetadataMap imageMetadataMap = new ImageMetadataMap();
        #endregion

        #region intialize properties
        public pageSorterForm() => InitializeComponent();



        private void pageSorterForm_Load(object sender, EventArgs e)
        {
            setupImageListStyles(inImageListView);
            setupImageListStyles(outImageListView);
            coverToggle.Checked = AppConfig.Instance.HasCover;
            duplexToggle.Checked = AppConfig.Instance.DuplexSelectMode;
                duplexToolStripMenuItem.Checked = AppConfig.Instance.DuplexSelectMode;
            missingCoverToolStripMenuItem.Checked = !AppConfig.Instance.HasCover;
            missingCoverToolStripMenuItem1.Checked = !AppConfig.Instance.HasCover;
            Application.DoEvents();
            loadLayout();
        }

        #endregion

        #region imagelist event handlers and properties
        private void setupImageListStyles(ImageListView list)
        {
            list.SetRenderer(new ThumbnailRenderer(imageMetadataMap));
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
                    string title = Path.GetFileName(p.FullName);
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
                imageNumber = 0;
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var currentPage = pdfDoc.GetPage(i);
                    rotation = currentPage.GetRotation();
                    clip = Rectangle.Empty;
                    Console.WriteLine("GetPageSizeWithRotation " + currentPage.GetPageSizeWithRotation());
                    Console.WriteLine("GetPageSize " + currentPage.GetPageSize());
                    Console.WriteLine("GetRotation " + currentPage.GetRotation());
                    iText.Kernel.Pdf.PdfDictionary currentPageObjects = currentPage.GetPdfObject();
                    pdfDoc.GetNumberOfPdfObjects();
                    int numberOfPdfObject = currentPageObjects.Size();
                    foreach (PdfObject currentPageObject in currentPageObjects.Values())
                    {
                        ProcessPDFObject(currentPageObject, outputFolder);
                    }


                    //imageListner.SetCurrentPage(i, pdfDoc.GetPage(i)); // Corrected method name                    
                    //parser.ProcessPageContent(pdfDoc.GetPage(i));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        Hashtable processedObjects = new Hashtable();
        int rotation = 0;
        int imageNumber = 0;
        Rectangle clip = Rectangle.Empty;
        Rectangle mediabox = Rectangle.Empty;

        private void ProcessPDFObject(PdfObject obj, string outputFolder, String name = null) // optional name parameter
        {
            if (obj == null || (obj.GetIndirectReference() != null && processedObjects.ContainsKey(obj.GetIndirectReference()))) return;
            if (name != null) ProcessName(name, obj);
            if (obj.GetIndirectReference() != null) processedObjects.Add(obj.GetIndirectReference(), obj);
            switch (obj.GetObjectType())
            {
                case PdfObject.ARRAY:
                    var a = (PdfArray)obj;
                    foreach (var child in a) ProcessPDFObject(child, outputFolder);
                    break;
                case PdfObject.DICTIONARY:
                    foreach (var key in ((PdfDictionary)obj).KeySet())
                    {
                        if (((PdfDictionary)obj).Get(key).IsNumber())
                            ProcessPDFObject(((PdfDictionary)obj).Get(key), outputFolder, key.ToString());
                    }
                    foreach (var key in ((PdfDictionary)obj).KeySet())
                    {
                        if (!((PdfDictionary)obj).Get(key).IsNumber())
                            ProcessPDFObject(((PdfDictionary)obj).Get(key), outputFolder, key.ToString());
                    }
                    break;
                case PdfObject.INDIRECT_REFERENCE:
                    break;
                case PdfObject.STREAM:
                    var PDFStremObj = (PdfStream)obj;
                    PdfObject subtype = PDFStremObj.Get(PdfName.Subtype);
                    if ((subtype == null) || subtype.ToString() != PdfName.Image.ToString()) break;
                    byte[] data = (obj as PdfStream).GetBytes();
                    string title = $"{imageNumber++:D3}.jpg";
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        var fileName = Path.Combine(outputFolder, title);
                        using (Image img = Image.FromStream(ms))
                        {
                            var croppedImg = CropToBoundsAndRotate(img, clip, mediabox, 0);
                            croppedImg.Save(fileName, ImageFormat.Jpeg);
                        }
                    }
                    ImageMetadata metadata = new ImageMetadata(outputFolder, title);
                    imageMetadataMap[title] = metadata;
                    metadata.clipRect = clip;
                    metadata.mediaRect = mediabox;
                    metadata.Orientation = rotation;

                    Console.WriteLine(name + " image: " + imageNumber + "Rotation: " + rotation + "Mediabox " + mediabox + " clipRect " + clip + " r ");
                    break;
                case PdfObject.NAME:
                    break;
                case PdfObject.NUMBER:
                    break;
                default:
                    Console.WriteLine("-->" + obj.GetType());
                    break;
            }
        }



        private int ProcessName(string name, PdfObject obj)
        {
            switch (name)
            {
                case "/CropBox":
                    clip = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/MediaBox":
                    mediabox = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/Type":
                    if (obj.ToString() == "/Page")
                    {
                        mediabox = Rectangle.Empty;
                        rotation = 0;
                        clip = Rectangle.Empty;
                    }
                    return 1;
                case "/Rotate":
                    rotation = int.Parse(obj.ToString());
                    return 1;
                default:
                    return 0;

            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void exportPDF_Handler(object sender, EventArgs e)
        {
            saveImagesToPDF(outImageListView, currentlyOpenPDFfile);
            saveCommentsToTXT(imageMetadataMap, currentlyOpenPDFfile);
        }

        private void saveCommentsToTXT(ImageMetadataMap imageMetadataMap, string inputPdf)
        {
            string txtFile = System.IO.Path.GetDirectoryName(inputPdf) + "/Comments - " + System.IO.Path.GetFileNameWithoutExtension(inputPdf) + ".txt";
            using (StreamWriter sw = new StreamWriter(txtFile))
            {
                foreach (var item in imageMetadataMap.Values)
                {
                    if (item.Comment != null && item.Comment.Length > 0)
                    {
                        string page = item.Title.Contains(".") ? item.Title.Split('.')[0] : item.Title;
                        sw.WriteLine($"Page: {page} : {item.Comment}");
                    }
                }
            }
        }

        private void saveImagesToPDF(ImageListView outImageListView, string inputPdf)
        {
            string outputPdf = System.IO.Path.GetDirectoryName(inputPdf) + "/Reordered - " + System.IO.Path.GetFileNameWithoutExtension(inputPdf) + ".pdf";
            using (PdfWriter writer = new PdfWriter(outputPdf))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf); // Create a Document instance
                    doc.SetMargins(0, 0, 0, 0);
                    if (imageMetadataMap["Cover"] != null)
                        addPagewithText(pdf, doc, imageMetadataMap["Cover"].Comment);
                    foreach (ImageListViewItem item in outImageListView.Items)
                    {
                        string path = Path.Combine(item.FilePath, item.FileName);
                        ImageMetadata metadata = imageMetadataMap[item.Text];
                        if (metadata.Comment.Contains("Previous")) addPagewithText(pdf, doc, "Missing Page");
                        PdfPage page = pdf.AddNewPage(metadata.pageSize);
                        ImageData imageData = ImageDataFactory.Create(path);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                        page.SetMediaBox(metadata.mediaBox);
                        page.SetCropBox(metadata.clipBox);
                        page.SetRotation(metadata.Orientation);
                        image.SetRotationAngle(-metadata.Rotate * Math.PI / 180);
                        doc.Add(image);
                        Console.WriteLine($"--->>>> {metadata.Orientation} {metadata.clipRect} {metadata.mediaRect} {metadata.Title}");
                        if (metadata.Comment.Contains("Next")) addPagewithText(pdf, doc, "Missing Page");
                    }

                }
                writer.Close();
                MessageBox.Show("PDF saved successfully!", "Save PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(outputPdf);
            }
        }

        private void addPagewithText(PdfDocument pdf, Document doc, string v, bool addBreak = true)
        {
            // Set page background color to cyan
            // and write text with large black letters in the center.
            //PdfPage page =  pdf.AddNewPage();
            doc.Add(new iText.Layout.Element.Paragraph(v)
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.CYAN)
.SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetFontSize(24f));
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

        private void updateInPreview(object sender, ItemHoverEventArgs e) => updatePreview(inPreview, e.Item);

        private void updateOutPreview(object sender, ItemHoverEventArgs e) => updatePreview(outPreview, e.Item);


        private void updatePreview(PictureBox preview, ImageListViewItem item)
        {
            if (item == null) return;
            ImageMetadata metadata = imageMetadataMap[item.Text];
            preview.Tag = item;
            string path = Path.Combine(item.FilePath, item.FileName);
            preview.Image = ImageUtils.RotateImage(Image.FromFile(path), metadata.Orientation, metadata.Rotate);
        }


        private void rotateLeft_Click(object sender, EventArgs e)
        {
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            { rotate(inImageListView, -1); updatePreview(inPreview, inImageListView.SelectedItems[0]); }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            { rotate(outImageListView, -1); updatePreview(outPreview, outImageListView.SelectedItems[0]); }
        }

        private void rotateRight_Click(object sender, EventArgs e)
        {
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            { rotate(inImageListView, 1); updatePreview(inPreview, inImageListView.SelectedItems[0]); }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            { rotate(outImageListView, 1); updatePreview(outPreview, outImageListView.SelectedItems[0]); }

        }

        private void rotate(ImageListView imageListView, float angle)
        {
            for (int i = 0; i < imageListView.SelectedItems.Count; i++)
            {
                ImageListViewItem item = imageListView.SelectedItems[i];
                imageMetadataMap[item.Text].Rotate += angle;
                Console.WriteLine("Rotating + " + item.FileName);
                item.Update();
            }
        }
        private void rotateLayout(ImageListView imageListView, int angle)
        {
            for (int i = 0; i < imageListView.SelectedItems.Count; i++)
            {
                ImageListViewItem item = imageListView.SelectedItems[i];
                imageMetadataMap[item.Text].Orientation = (imageMetadataMap[item.Text].Orientation + angle) % 360;
                item.Update();
            }
        }

        private void tooggleLayout_Click(object sender, EventArgs e)
        {
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            { rotateLayout(inImageListView, 90); updatePreview(inPreview, inImageListView.SelectedItems[0]); }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            { rotateLayout(outImageListView, 90); updatePreview(outPreview, outImageListView.SelectedItems[0]); }
        }

        private void commentsContextMenuItem_Click(object sender, EventArgs e)
        {
            string comment = sender.ToString().Replace("&", string.Empty);
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            {
                setComment(inImageListView, comment);
                updatePreview(inPreview, inImageListView.SelectedItems[0]);
            }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            {
                setComment(outImageListView, comment);
                updatePreview(outPreview, outImageListView.SelectedItems[0]);
            }
            else if (inPreview.Focused)
            {
                setComment(inPreview.Tag as ImageListViewItem, comment);
            }
            else if (outPreview.Focused)
            {
                setComment(outPreview.Tag as ImageListViewItem, comment);
            }
        }

        private void setComment(ImageListView imageListView, string comment)
        {
            foreach (var item in imageListView.SelectedItems) setComment(item, comment);
        }
        private void setComment(ImageListViewItem item, string comment)
        {
            if (comment.Contains("Cover"))
            {
                ImageMetadata imageMetadata = new ImageMetadata("Cover", "Cover");
                imageMetadata.Comment = comment;
                imageMetadataMap["Cover"] = imageMetadata;

            }
            else
            {
                imageMetadataMap[item.Text].Comment = comment;
                item.Update();
            }

        }

        private void duplexToggle_Click(object sender, EventArgs e)
        {
            AppConfig.Instance.DuplexSelectMode = !AppConfig.Instance.DuplexSelectMode;
            duplexToggle.Checked = AppConfig.Instance.DuplexSelectMode;
            duplexToolStripMenuItem.Checked = AppConfig.Instance.DuplexSelectMode;
        }

        private void coverToggle_Click(object sender, EventArgs e)
        {
            AppConfig.Instance.HasCover = !AppConfig.Instance.HasCover;
            coverToggle.Checked = AppConfig.Instance.HasCover;            
            missingCoverToolStripMenuItem.Checked = !AppConfig.Instance.HasCover;
            missingCoverToolStripMenuItem1.Checked = !AppConfig.Instance.HasCover;
        }

        private void inImageListView_SelectionChanged(object sender, EventArgs e)
        {
            if ((inImageListView.SelectedItems.Count > 0) && AppConfig.Instance.DuplexSelectMode)
            {
                int coverNotSeen = 1;
                Console.WriteLine($"{coverNotSeen}");
                for (int i = 0; i < inImageListView.Items.Count; i++)
                {
                    if (i == 0 && AppConfig.Instance.HasCover /*&& coverNotSeen !=0*/ && inImageListView.Items[i].Text == "000.jpg") { coverNotSeen = 0; if (AppConfig.Instance.HasCover) continue; }
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
            if ((outImageListView.SelectedItems.Count > 0) && AppConfig.Instance.DuplexSelectMode)
            {
                int coverNotSeen = 1;
                Console.WriteLine($"{coverNotSeen}");
                for (int i = 0; i < outImageListView.Items.Count; i++)
                {
                    if (i == 0 && AppConfig.Instance.HasCover /*&& coverNotSeen !=0*/ && outImageListView.Items[i].Text == "000.jpg") { coverNotSeen = 0; if (AppConfig.Instance.HasCover) continue; }
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