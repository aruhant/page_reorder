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
using Scanned_Page_Sorter.Lib.Utils;

namespace Scanned_Page_Sorter
{
    partial class pageSorterForm
    {
        #region Preview Update Methods
        /// <summary>
        /// Updates the preview for the input image list view
        /// </summary>
        private void updateInPreview(object sender, ItemHoverEventArgs e) => updatePreview(inPreview, e.Item);

        /// <summary>
        /// Updates the preview for the output image list view
        /// </summary>
        private void updateOutPreview(object sender, ItemHoverEventArgs e) => updatePreview(outPreview, e.Item);

        /// <summary>
        /// Updates the preview image in the specified PictureBox
        /// </summary>
        private void updatePreview(PictureBox preview, ImageListViewItem item)
        {
            if (item == null) return;
            PageMetadata metadata = sourceDocument.PageMetadataMap[(string)item.Text];
            if (metadata == null || metadata.PageType == PageType.MissingContent) return;
            preview.Tag = item;
            string path = Path.Combine(item.FilePath, item.FileName);
            preview.Image = ImageUtils.RotateImage(Image.FromFile(path), metadata.Orientation, metadata.Rotate);
        }
        #endregion

        #region Context Menu Handlers
        /// <summary>
        /// Handles context menu actions for the input image list view
        /// </summary>
        private void inContextMenuItem_Click(object sender, EventArgs e)
        {
            string operation = sender.ToString().Replace("&", string.Empty);
            if (inImageListView.SelectedItems.Count > 0 && inImageListView.Focused)
            {
                HandleContextMenuAction(operation, inImageListView, inImageListView.SelectedItems);
            }
            else
            {
                HandleContextMenuAction(operation, inImageListView, new List<ImageListViewItem> { (ImageListViewItem)inPreview.Tag });
            }
        }

        /// <summary>
        /// Handles context menu actions for the output image list view
        /// </summary>
        private void outContextMenuItem_Click(object sender, EventArgs e)
        {
            string operation = sender.ToString().Replace("&", string.Empty);
            if (outImageListView.SelectedItems.Count > 0 && outImageListView.Focused)
            {
                HandleContextMenuAction(operation, outImageListView, outImageListView.SelectedItems);
            }
            else
            {
                HandleContextMenuAction(operation, outImageListView, new List<ImageListViewItem> { (ImageListViewItem)outPreview.Tag });
            }
        }

        /// <summary>
        /// Handles context menu actions for image list view items
        /// </summary>
        private void HandleContextMenuAction(string operation, ImageListView imageListView, IEnumerable<ImageListViewItem> imageListViewItems)
        {
            if (operation.ToLower().Contains("comment"))
            {
                string comment = Prompt.ShowDialog("Enter Comment", "Comment");
                foreach (var item in imageListViewItems) setComment(item, comment);
            }
            else if (operation.ToLower().Contains("rotate"))
            {
                foreach (var item in imageListViewItems)
                {
                    sourceDocument.PageMetadataMap[(string)item.Text].Orientation = (sourceDocument.PageMetadataMap[(string)item.Text].Orientation + 90) % 360;
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
                // Cover logic placeholder
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
                var missingPage = CreateNewPage("Missing Page ~" + new Random().Next(), PageType.MissingContent, pagesAfterInsertion.First().PageNumber);
                imageListView.Items.Insert(pos, missingPage);

                foreach (var page in pagesAfterInsertion)
                {
                    page.PageNumber++;
                }
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Creates a new ImageListViewItem for a missing or cover page
        /// </summary>
        private ImageListViewItem CreateNewPage(string v, PageType content, int pageNumber)
        {
            ImageListViewItem item = new ImageListViewItem(v);
            sourceDocument.PageMetadataMap[v] = new PageMetadata("", v, content, pageNumber: pageNumber);
            return item;
        }

        /// <summary>
        /// Sets a comment for the specified image list view item
        /// </summary>
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
        #endregion

        #region Deskew Button Handler
        /// <summary>
        /// Deskews all images in the document
        /// </summary>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            foreach (var item in sourceDocument.PageMetadataMap.Values)
            {
                item.Rotate = -ImageUtils.AutoDeskew(item.FileName);
                statusMessage.Text = "Deskewed " + item.FileName + " to " + item.Rotate.ToString();
            }
            inImageListView.Refresh();
            outImageListView.Refresh();
        }
        #endregion
    }
}
