using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private void extractImagesFromPDF(string sourcePdf, string outputFolder)
        {
            int i = 0;
            using (PdfDocument document = PdfDocument.Open(sourcePdf))
            {
                foreach (Page page in document.GetPages())
                {
                    IEnumerable<IPdfImage> images = page.GetImages();
                    foreach (var image in images)
                    {
                        Console.WriteLine("image: " + image.ToString());
                        IReadOnlyList<byte> bytes = image.RawBytes;
                        if (bytes != null && bytes.Count > 0)
                        {
                            using (var ms = new MemoryStream(bytes.ToArray()))
                            {
                                try
                                {
                                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

                                    // Convert PdfRectangle to System.Drawing.Rectangle
                                    PdfRectangle pdfRect = page.CropBox.Bounds;

                                    double scale = img.Height / image.Bounds.Height;
                                    Rectangle cropRect = new Rectangle((int)(pdfRect.Left * scale), (int)(pdfRect.Top * scale), (int)(pdfRect.Width * scale), (int)(pdfRect.Height * scale));
                                    Console.WriteLine("Height " + img.Height + "samples " + image.Bounds.Height + "Crop " + cropRect.Height);
                                    Console.WriteLine("Height " + img.Width + "samples " + image.Bounds.Width + "Crop " + cropRect.Width);
                                    // convert  croprect from pdf units to pixels
                                    double rotation = (double)page.Rotation.Radians;
                                    Console.WriteLine(scale + " Rot " + rotation);

                                    img = CropToBounds(img, cropRect, rotation);
                                    img.Save(Path.Combine(outputFolder, $"{i:D3}.jpg"), ImageFormat.Jpeg);
                                    i++;
                                    Console.WriteLine("Wrote " + outputFolder + i.ToString("D3") + ".jpg");
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error: " + e.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        private Image CropToBounds(Image img, Rectangle cropRect, double rotation)
        {
            Bitmap src = img as Bitmap;

            if (cropRect.Height == 0 || cropRect.Width == 0) cropRect = new Rectangle(0, 0, img.Width, img.Height);

            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
                if (rotation != 0) {
                    g.TranslateTransform(target.Height / 2, target.Width / 2);
                    g.RotateTransform((float)(rotation * (180.0 / Math.PI)));
                    g.TranslateTransform(-target.Height / 2, -target.Width / 2); }
            }
        
    

            return target;
        }
    }
}