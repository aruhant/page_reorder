using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manina.Windows.Forms;
using Scanned_Page_Sorter.Lib.models;
using Scanned_Page_Sorter.Lib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

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
            preview.Tag = item.Text;
            string path = Path.Combine(item.FilePath, item.FileName);
            preview.Image = ImageUtils.RotateImage(Image.FromFile(path), metadata.Orientation, metadata.Rotate);
        }



        private void commentsContextMenuItem_Click(object sender, EventArgs e)
        {
            string comment = sender.ToString().Replace("&", string.Empty);
            if (comment.ToLower().Contains( "comment"))
            {
                comment = Prompt.ShowDialog("Enter Comment", "Comment");
            }
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
            if (comment.ToLower().Contains("blurred"))
            {
                sourceDocument.PageMetadataMap[(string)item.Text].Blurred = 1 - sourceDocument.PageMetadataMap[(string)item.Text].Blurred;
            }
            else if (comment.ToLower().Contains("missing"))
            {
                sourceDocument.PageMetadataMap[(string)item.Text].MissingPage = 1 - sourceDocument.PageMetadataMap[(string)item.Text].MissingPage;
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
