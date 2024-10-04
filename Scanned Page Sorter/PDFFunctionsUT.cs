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
        private void extractImagesFromPDF_UT(string sourcePdf, string outputFolder)
        {
            int i = 0;
            using (PdfDocument document = PdfDocument.Open(sourcePdf))
            {
                foreach (Page page in document.GetPages())
                {
                    Console.WriteLine("\n" + page.ToString() );
                    IEnumerable<IPdfImage> images = page.GetImages();
                    foreach (IPdfImage image in images)
                    {
                        Console.WriteLine( "\n" + page.Number +  "\n" + image.ToString());
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
                                    Console.WriteLine("Height " + img.Height + " samples " + image.Bounds.Height + " Crop " + cropRect.Height);
                                    Console.WriteLine("Width " + img.Width + " samples " + image.Bounds.Width + " Crop " + cropRect.Width);
                                    /// convert  croprect from pdf units to pixels
                                    double rotation = (double)page.Rotation.Radians;
                                    Console.WriteLine("Scale "+ scale + " Rot " + rotation + "\n");

                                    //img = CropToBoundsAndRotate(img, cropRect, rotation);
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

        private Image CropToBoundsAndRotate(Image img, Rectangle cropRect, Rectangle mediaRect, double rotation)
        {
            Bitmap src = img as Bitmap;
            Console.WriteLine($"\n\n**************************\nRotation {rotation}\nCrop:{cropRect}\nmedia:{mediaRect}");
            double scalex = (double)src.Width / mediaRect.Width;
            double scaley = (double)src.Height / mediaRect.Height;
            Console.WriteLine($"Scalex {scalex} Scaley {scaley}");
            if (cropRect.Height == 0 || cropRect.Width == 0) cropRect = new Rectangle(0, 0, img.Width, img.Height);
            else cropRect = new Rectangle((int)((cropRect.X) * scalex), (int)((cropRect.Height - cropRect.Y) * scaley),
                (int)(cropRect.Width * scalex), (int)(cropRect.Height * scaley));

            // Create a new bitmap for the target image
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            Console.WriteLine($"src:{src.Width},{src.Height} \nTargetrect: {cropRect}");
            // Rotate and crop the image
            using (Graphics g = Graphics.FromImage(target))
            {
                g.TranslateTransform((float)cropRect.Width / 2, (float)cropRect.Height / 2);
                g.RotateTransform((float)rotation);
                g.TranslateTransform(-(float)cropRect.Width / 2, -(float)cropRect.Height / 2);
                g.DrawImage(src, new Rectangle(0, 0, cropRect.Width, cropRect.Height), new Rectangle(cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height), GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}