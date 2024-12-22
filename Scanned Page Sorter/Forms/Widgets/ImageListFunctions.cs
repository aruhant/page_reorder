using System;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;

namespace Scanned_Page_Sorter
{
    partial class pageSorterForm
    {


        private void loadImages(string inputFolder)
        {
            DirectoryInfo path = new DirectoryInfo(inputFolder);
            statusMessage.Text = "One moment....";
            Application.DoEvents();
            inImageListView.Items.Clear();
            outImageListView.Items.Clear();
            inImageListView.SuspendLayout();
            int index = 1;
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
                    if (sourceDocument.PageMetadataMap[title]==null)
                    {
                        sourceDocument.PageMetadataMap[title] = new PageMetadata(p.FullName, title, PageType.Content, index, index);
                        Console.WriteLine("Creating " + sourceDocument.PageMetadataMap[title]);
                    }
                    else {
                        Console.WriteLine("Exists " + sourceDocument.PageMetadataMap[title]);
                    }
                    inImageListView.Items.Add(item);
                }
            }
            inImageListView.ResumeLayout();
            statusMessage.Text = "Ready...";
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
                sourceDocument.PageMetadataMap[(string)item.Text].Rotate += angle;
                Console.WriteLine("Rotating + " + item.FileName);
                item.Update();
            }
        }
        private void rotateLayout(ImageListView imageListView, int angle)
        {
            for (int i = 0; i < imageListView.SelectedItems.Count; i++)
            {
                ImageListViewItem item = imageListView.SelectedItems[i];
                sourceDocument.PageMetadataMap[(string)item.Text].Orientation = (sourceDocument.PageMetadataMap[(string)item.Text].Orientation + angle) % 360;
                item.Update();
            }
        }


    }


}
