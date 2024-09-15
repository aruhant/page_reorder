using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private string pdfFile;
        private string imageFolder;
        public pageSorterForm()
        {
            InitializeComponent();
        }

        private void pageSorterForm_Load(object sender, EventArgs e)
        {
            enableReorderOnList(inList);
            enableReorderOnList(outList);
        }

        private void enableReorderOnList(ListView list)
        {
            list.AllowDrop = true;
            list.DragEnter += new DragEventHandler(dragEnterHandler);
            list.DragDrop += new DragEventHandler(dragDropHandler);
            list.ItemDrag += new ItemDragEventHandler(itemDragHandler);
            list.DragOver += new DragEventHandler(dragOverHandler);
            list.DragLeave += new EventHandler(dragLeaveHandler);
            list.ListViewItemSorter = new ListViewIndexComparer();
        }

        private void List_DragLeave(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
                if (draggedItem.ListView == list )  if (draggedItem.Index < dropIndex) dropIndex--;
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
        private void dragLeaveHandler (object sender, EventArgs e)
        {
            ListView list = sender as ListView;
            list.InsertionMark.Index = -1;
        }

        private class ListViewIndexComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
            }
        }

 

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            // open a file dialog to select a pdf file.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF Files|*.pdf";
            openFileDialog.Title = "Select a PDF File";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // load the pdf file to the inList
                pdfFile = openFileDialog.FileName;
                                extractImages(pdfFile);
                this.Text = System.IO.Path.GetFileName(pdfFile);
            }
        }


        private void loadImages(string inputFolder)
        {
            statusMessage.Text = "One moment....";
             Application.DoEvents();
            thumbnailImageList.Images.Clear();
            inList.Items.Clear();
            outList.Items.Clear();
            inList.LargeImageList = thumbnailImageList;
            outList.LargeImageList = thumbnailImageList;
            string[] files = System.IO.Directory.GetFiles(inputFolder);
            foreach (string file in files)
            {
                Image img = Image.FromFile(file);
                string fileName = Path.GetFileNameWithoutExtension(file); // Fix: Added this line to get the file name
                thumbnailImageList.Images.Add(img);
                inList.Items.Add(fileName, thumbnailImageList.Images.Count - 1); // Fix: Added fileName as the first parameter
            }
            statusMessage.Text = "Ready...";
        }


        private void extractImages(string pdfFile)
        {
            // extract images from pdf file to input folder, and then load images to inList
            string inputFolder = "../../images/";
            System.IO.Directory.CreateDirectory(inputFolder);
            string pdfFileName = System.IO.Path.GetFileNameWithoutExtension(pdfFile);
            imageFolder = inputFolder + pdfFileName + "/";
            // empty the output folder if it already exists else create it
            if (System.IO.Directory.Exists(imageFolder))
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(imageFolder);
                foreach (System.IO.FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
            {
                System.IO.Directory.CreateDirectory(imageFolder);
            }
            extractImageFromPDF (pdfFile, imageFolder);
            loadImages(imageFolder);
        }

        public void extractImageFromPDF(string sourcePdf, string outputFolder)
        {
            PdfReader reader = new PdfReader(sourcePdf);

            try
            {
                PdfDocument document = new PdfDocument(reader);
                for (int i = 1; i <= document.GetNumberOfPages(); i++)
                {
                    statusMessage.Text = "Extracting image " + i ;
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
                            catch (Exception e)
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

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveImagesToPDF (thumbnailImageList.Images,  outList, pdfFile  );
        }
        // Inside the saveImagesToPDF method
        private void saveImagesToPDF(ImageList.ImageCollection images, ListView listView, string inputPdf)
        {
            string outputPdf = System.IO.Path.GetDirectoryName(inputPdf) + "/" + System.IO.Path.GetFileNameWithoutExtension(inputPdf) + "_sorted.pdf";
            using (PdfWriter writer = new PdfWriter(outputPdf))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf); // Create a Document instance
                    foreach (ListViewItem item in listView.Items)
                    {
                        Image img = images[item.ImageIndex];
                        string imageFilePath = imageFolder+ "/"+ ( item.Text) + ".jpg";
                        ImageData imageData = ImageDataFactory.Create(imageFilePath);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData); // Use the correct Image class

                        doc.Add(image);
                    }
                }
            }
        }


 
    }
}
