using Manina.Windows.Forms;

namespace ScanSort.Presenters;

/// <summary>
/// Pure-function helper for duplex page pairing logic.
///
/// Physical context: pages are scanned from bound books. Each leaf has recto (front)
/// and verso (back). Consecutive pages alternate front/back of the same leaf.
///
/// With cover:  page 0 = cover (unpaired), then 1+2, 3+4, ... (even + next odd)
/// Without cover: 0+1, 2+3, ... (odd + next even)
/// </summary>
public static class DuplexSelectionHelper
{
    public static void ApplyDuplexSelection(ImageListView listView, bool hasCover)
    {
        if (listView.SelectedItems.Count == 0) return;

        // Determine cover offset: if cover mode is on and first page is "000.jpg", offset is 0
        int coverOffset = 1;
        if (hasCover && listView.Items.Count > 0 && listView.Items[0].Text == "000.jpg")
            coverOffset = 0;

        for (int i = 0; i < listView.Items.Count; i++)
        {
            // Skip the cover page itself — it's unpaired
            if (i == 0 && hasCover && coverOffset == 0)
                continue;

            if (!listView.Items[i].Selected)
                continue;

            int adjusted = coverOffset + i;

            if (adjusted % 2 == 1) // Odd — select next
            {
                if (i + 1 < listView.Items.Count && !listView.Items[i + 1].Selected)
                    listView.Items[i + 1].Selected = true;
                i++; // Skip the paired item to avoid re-processing
            }
            else // Even — select previous
            {
                if (i >= 1 && !listView.Items[i - 1].Selected)
                    listView.Items[i - 1].Selected = true;
            }
        }
    }
}
