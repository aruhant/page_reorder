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

                                    // Convert image.Bounds to pixels from pdf units
                                    Rectangle cropRect = new Rectangle((int)image.Bounds.Left, (int)image.Bounds.Top, (int)image.Bounds.Width, (int)image.Bounds.Height) ;

                                    img = CropToBounds(img, cropRect);
                                    img.Save(Path.Combine(outputFolder, $"{i:D3}.jpg"), ImageFormat.Jpeg);
                                    i++;
                                    Console.WriteLine("Wrote " + outputFolder + i.ToString("D3") + ".jpg");
                                    Console.WriteLine(image.Bounds);
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

        private Image CropToBounds(Image img, Rectangle cropRect)
        {
            Bitmap src = img as Bitmap;
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);

            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height), cropRect, GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}