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
                    //if (_imageMetadataMap["Cover"] != null)
                    //    addPagewithText(pdf, doc, _imageMetadataMap["Cover"].Comment);
                    foreach (ImageListViewItem item in _outImageListView.Items)
                    {
                        PageMetadata metadata = _imageMetadataMap[(string)item.Text];
                        if (metadata.PageType == PageType.MissingContent) { addPagewithText(pdf, doc, "Missing Page"); } else{
                        string path = Path.Combine(item.FilePath, item.FileName);
                        var p = metadata.PageSize;
                        PdfPage page = pdf.AddNewPage(p);
                        ImageData imageData = ImageDataFactory.Create(path);
                        iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                        //page.SetMediaBox(p);
                        //page.SetCropBox(metadata.ClipBox);
                        page.SetRotation(metadata.Orientation);
                        image.SetRotationAngle(-metadata.Rotate * Math.PI / 180);
                        doc.Add(image);
                            Console.WriteLine($"--->>>> {metadata.Orientation} {metadata.ClipRect} {metadata.MediaRect} {metadata.Title}");
                        }
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
