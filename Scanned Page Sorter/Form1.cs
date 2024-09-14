using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        public pageSorterForm()
        {
            InitializeComponent();
        }

        private void pageSorterForm_Load(object sender, EventArgs e)
        {
            loadImages(inputFolder: "../../images/");
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

        private void loadImages(string inputFolder)
        {
            // loads images from input folder to image list, and then from imageList to inList
            inList.LargeImageList = thumbnailImageList;
            outList.LargeImageList = thumbnailImageList;
            string[] files = System.IO.Directory.GetFiles(inputFolder);
            foreach (string file in files)
            {
                Image img = Image.FromFile(file);
                thumbnailImageList.Images.Add(img);
                inList.Items.Add(file, thumbnailImageList.Images.Count - 1);
            }
        }
        private class ListViewIndexComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return ((ListViewItem)x).Index - ((ListViewItem)y).Index;
            }
        }
    }
}
