using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Manina.Windows.Forms;

namespace Scanned_Page_Sorter.Services
{
    /// <summary>
    /// Service to handle duplex selection logic for image list views
    /// </summary>
    public class DuplexSelectionService
    {
        /// <summary>
        /// Handles duplex selection for an image list view
        /// </summary>
        /// <param name="imageListView">The image list view to process</param>
        /// <param name="enableDuplexMode">Whether duplex mode is enabled</param>
        /// <param name="enableCoverMode">Whether cover mode is enabled</param>
        public void HandleDuplexSelection(ImageListView imageListView, bool enableDuplexMode, bool enableCoverMode)
        {
            if (imageListView == null)
                throw new ArgumentNullException(nameof(imageListView));

            if (!enableDuplexMode || imageListView.SelectedItems.Count == 0)
                return;

            try
            {
                var selectedIndices = GetSelectedIndices(imageListView);
                var coverOffset = CalculateCoverOffset(imageListView, enableCoverMode);

                ProcessDuplexSelections(imageListView, selectedIndices, coverOffset);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in duplex selection handling: {ex.Message}");
            }
        }

        private static List<int> GetSelectedIndices(ImageListView imageListView)
        {
            var indices = new List<int>();
            for (int i = 0; i < imageListView.Items.Count; i++)
            {
                if (imageListView.Items[i].Selected)
                {
                    indices.Add(i);
                }
            }
            return indices;
        }

        private static int CalculateCoverOffset(ImageListView imageListView, bool enableCoverMode)
        {
            if (!enableCoverMode || imageListView.Items.Count == 0)
                return 1;

            // Check if first item is a cover page (assumes cover pages are named "000.jpg")
            return imageListView.Items[0].Text == "000.jpg" ? 0 : 1;
        }

        private static void ProcessDuplexSelections(ImageListView imageListView, List<int> selectedIndices, int coverOffset)
        {
            foreach (int index in selectedIndices.ToList()) // ToList to avoid modification during iteration
            {
                var adjustedIndex = coverOffset + index;
                
                if (IsOddPage(adjustedIndex))
                {
                    SelectNextPage(imageListView, index);
                }
                else if (IsEvenPage(adjustedIndex))
                {
                    SelectPreviousPage(imageListView, index);
                }
            }
        }

        private static bool IsOddPage(int adjustedIndex) => adjustedIndex % 2 == 1;
        
        private static bool IsEvenPage(int adjustedIndex) => adjustedIndex % 2 == 0;

        private static void SelectNextPage(ImageListView imageListView, int currentIndex)
        {
            int nextIndex = currentIndex + 1;
            if (nextIndex < imageListView.Items.Count && !imageListView.Items[nextIndex].Selected)
            {
                imageListView.Items[nextIndex].Selected = true;
            }
        }

        private static void SelectPreviousPage(ImageListView imageListView, int currentIndex)
        {
            int previousIndex = currentIndex - 1;
            if (previousIndex >= 0 && !imageListView.Items[previousIndex].Selected)
            {
                imageListView.Items[previousIndex].Selected = true;
            }
        }
    }
}