using System;
using System.IO;
using System.Windows.Forms;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using Manina.Windows.Forms;

namespace Scanned_Page_Sorter.Lib.PDF
{
    internal class PdfExporter
    {
        private readonly ImageListView _outImageListView;
        private readonly string _saveLocation;
        private readonly PageMetadataMap _imageMetadataMap;
        public PdfExporter(ImageListView outImageListView, string saveLocation, PageMetadataMap imageMetadataMap)
        {
            _outImageListView = outImageListView;
            _saveLocation = saveLocation;
            _imageMetadataMap = imageMetadataMap;
        }
        public void export()
        {
            using (PdfWriter writer = new PdfWriter(_saveLocation))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document doc = new Document(pdf); // Create a Document instance
                    doc.SetMargins(0, 0, 0, 0);
                    if (_imageMetadataMap["Cover"] != null)
                        addPagewithText(pdf, doc, _imageMetadataMap["Cover"].Comment);
                    foreach (ImageListViewItem item in _outImageListView.Items)
                    {
                        string path = Path.Combine(item.FilePath, item.FileName);
                        PageMetadata metadata = _imageMetadataMap[(string)item.Text];
                        if (metadata.Comment.Contains("Previous")) addPagewithText(pdf, doc, "Missing Page");
                        PdfPage page = pdf.AddNewPage(metadata.PageSize);
                        ImageData imageData = ImageDataFactory.Create(path);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                        page.SetMediaBox(metadata.MediaBox);
                        page.SetCropBox(metadata.ClipBox);
                        page.SetRotation(metadata.Orientation);
                        image.SetRotationAngle(-metadata.Rotate * Math.PI / 180);
                        doc.Add(image);
                        Console.WriteLine($"--->>>> {metadata.Orientation} {metadata.ClipRect} {metadata.MediaRect} {metadata.Title}");
                        if (metadata.Comment.Contains("Next")) addPagewithText(pdf, doc, "Missing Page");
                    }

                }
                writer.Close();
                MessageBox.Show("PDF saved successfully!", "Save PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(_saveLocation);
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
    }
}
