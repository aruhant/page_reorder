using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Scanned_Page_Sorter.Lib;

namespace Scanned_Page_Sorter
{
    partial class pageSorterForm
    {

        private void updateInPreview(object sender, ItemHoverEventArgs e) => updatePreview(inPreview, e.Item);

        private void updateOutPreview(object sender, ItemHoverEventArgs e) => updatePreview(outPreview, e.Item);


        private void updatePreview(PictureBox preview, ImageListViewItem item)
        {
            if (item == null) return;
            PageMetadata metadata = sourceDocument.PageMetadataMap[(string)item.Text];
            if (metadata.PageType == PageType.MissingContent) return;
            preview.Tag = item.Text;
            string path = Path.Combine(item.FilePath, item.FileName);
            preview.Image = ImageUtils.RotateImage(Image.FromFile(path), metadata.Orientation, metadata.Rotate);
        }



        private void outContextMenuItem_Click(object sender, EventArgs e)
        {
            string comment = sender.ToString().Replace("&", string.Empty);
            // If Comment is selected, prompt for comment
            if (comment.ToLower().Contains("comment"))
            {
                comment = Prompt.ShowDialog("Enter Comment", "Comment");
            }
            // Find the source of the event
            ImageListView imageListView = null;
            IEnumerable<ImageListViewItem> selectedItems = new List<ImageListViewItem>();
            PictureBox preview = null;

            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            {
                imageListView = inImageListView;
                selectedItems = inImageListView.SelectedItems;
                preview = inPreview;
            }
            else if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            {
                imageListView = outImageListView;
                selectedItems = outImageListView.SelectedItems;
                preview = outPreview;
            }
            else if (inPreview.Focused)
            {
                imageListView = inImageListView;
                selectedItems = new List<ImageListViewItem> { inPreview.Tag as ImageListViewItem };
                preview = inPreview;
            }
            else if (outPreview.Focused)
            {
                imageListView = outImageListView;
                selectedItems = new List<ImageListViewItem> { outPreview.Tag as ImageListViewItem };
                preview = outPreview;
            }

            setComments(imageListView, selectedItems, comment);
            //updatePreview(preview, selectedItems.First());
        }

        private void setComments(ImageListView imageListView, IEnumerable<ImageListViewItem> imageListViewItems, string comment)
        {
            if (comment.ToLower().Contains("missing"))
            {
                imageListView.Items.Add(CreateNewPage("Missing Page ~" + new Random().Next(), PageType.MissingContent));

            }
            else
                foreach (var item in imageListViewItems) setComment(item, comment);
        }

        private ImageListViewItem CreateNewPage(string v, PageType content)
        {
            ImageListViewItem item = new ImageListViewItem(v);
            sourceDocument.PageMetadataMap[v] = new PageMetadata("", v, content);
            return item;

        }

        private void setComment(ImageListViewItem item, string comment)
        {
            if (comment.ToLower().Contains("blurred"))
            {
                sourceDocument.PageMetadataMap[(string)item.Text].Blurred = 1 - sourceDocument.PageMetadataMap[(string)item.Text].Blurred;
            }
            else if (comment.Contains("Cover"))
            {
                PageMetadata imageMetadata = new PageMetadata("Cover", "Cover");
                imageMetadata.Comment = comment;
                sourceDocument.PageMetadataMap["Cover"] = imageMetadata;
            }
            else
            {
                sourceDocument.PageMetadataMap[(string)item.Text].Comment = comment;
            }
            item.Update();

        }

    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
