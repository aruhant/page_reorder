using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iText.Layout.Font;
using iText.StyledXmlParser.Jsoup.Nodes;
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
            preview.Tag = item;
            string path = Path.Combine(item.FilePath, item.FileName);
            preview.Image = ImageUtils.RotateImage(Image.FromFile(path), metadata.Orientation, metadata.Rotate);
        }

        private void inContextMenuItem_Click(object sender, EventArgs e)
        {
            string operation = sender.ToString().Replace("&", string.Empty);
            //var selectedItems = (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused) ?
            //    new List<ImageListViewItem>(inImageListView.SelectedItems) :
            //    new List<ImageListViewItem> { (ImageListViewItem)inPreview.Tag };
            //contextMenuItem_Click(comment, inImageListView, selectedItems);
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            {
                contextMenuItem_Click(operation, inImageListView, inImageListView.SelectedItems);
            }
            else
            {
                contextMenuItem_Click(operation, inImageListView, new List<ImageListViewItem> { (ImageListViewItem)inPreview.Tag });
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            foreach (var item in sourceDocument.PageMetadataMap.Values)
            {
                item.Rotate = ImageUtils.AutoDeskew(item.FileName);
                statusMessage.Text = "Deskewed " + item.FileName + " to " + item.Rotate.ToString();
            }
            inImageListView.Refresh();
            outImageListView.Refresh();
        }

        private void outContextMenuItem_Click(object sender, EventArgs e)
        {
            string operation = sender.ToString().Replace("&", string.Empty);
            if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            {
                contextMenuItem_Click(operation, outImageListView, outImageListView.SelectedItems);
            }
            else
            {
                contextMenuItem_Click(operation, outImageListView, new List<ImageListViewItem> { (ImageListViewItem)outPreview.Tag });
            }
        }
        private void contextMenuItem_Click(string operation, ImageListView imageListView,  IEnumerable<ImageListViewItem> imageListViewItems)
        {
            

            if (operation.ToLower().Contains("comment"))
            {
                string comment  = Prompt.ShowDialog("Enter Comment", "Comment");
                foreach (var item in imageListViewItems) setComment(item, comment);
            } else if (operation.ToLower().Contains("rotate"))
            {
                foreach (var item in imageListViewItems)
                {
                    sourceDocument.PageMetadataMap[(string)item.Text].Orientation = ( sourceDocument.PageMetadataMap[(string)item.Text].Orientation +90 ) % 360;
                    item.Update();
                }
            }
            else if (operation.ToLower().Contains("blur"))
            {
                foreach (var item in imageListViewItems)
                {
                    sourceDocument.PageMetadataMap[(string)item.Text].Blurred = 1 - sourceDocument.PageMetadataMap[(string)item.Text].Blurred;
                    item.Update();
                }
            }
            else if (operation.ToLower().Contains("cover"))
            {
                
            }
            else if (operation.ToLower().Contains("missing"))
            {
                bool found = false;
                int pos = 0;
                List<PageMetadata> pagesAfterInsertion = new List<PageMetadata>();
                foreach (var item in imageListView.Items)
                {
                    if (item == outPreview.Tag)
                    {
                        found = true;
                    }
                    if (found)
                    {
                        pagesAfterInsertion.Add(sourceDocument.PageMetadataMap[(string)item.Text]);
                    }
                    else
                    {
                        pos++;
                    }
                }
                var missingPage = CreateNewPage("Missing Page ~" + new Random().Next(), PageType.MissingContent, pagesAfterInsertion.First().PageNumber );
                imageListView.Items.Insert(pos, missingPage);

                foreach (var page in pagesAfterInsertion)
                {
                    page.PageNumber++;
                }
                }
           //updatePreview(preview, selectedItems.First());
        }


        private ImageListViewItem CreateNewPage(string v, PageType content, int pageNumber)
        {
            ImageListViewItem item = new ImageListViewItem(v);
            sourceDocument.PageMetadataMap[v] = new PageMetadata("", v, content, pageNumber: pageNumber);
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
