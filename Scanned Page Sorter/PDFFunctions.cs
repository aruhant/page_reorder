using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms; 
namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        static void ExportAsPngImage(PdfDictionary image, string outputFolder, ref int count)
        {
            Console.WriteLine(string.Join(", ", image.Elements.KeyNames.Select(k => k.ToString())));
            Console.WriteLine(image.Elements.GetString("/Filter") );
            byte[] stream = image.Stream.Value;
            FileStream fs = new FileStream(  $"{outputFolder}{count++:D3}.jpeg", FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(stream);
            bw.Close();
        }
         
        private void extractImagesFromPDF(string sourcePdf, string outputFolder)
        {
            int imageCount = 0;

            PdfDocument document = PdfReader.Open(sourcePdf);
            foreach (PdfPage page in document.Pages)
            {
                // Get resources dictionary
                PdfDictionary resources = page.Elements.GetDictionary("/Resources");
                if (resources != null)
                {
                    // Get external objects dictionary
                    PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
                    if (xObjects != null)
                    {
                        ICollection<PdfItem> items = xObjects.Elements.Values;
                        // Iterate references to external objects
                        foreach (PdfItem item in items)
                        {
                            PdfReference reference = item as PdfReference;
                            if (reference != null)
                            {
                                PdfDictionary xObject = reference.Value as PdfDictionary;
                                // Is external object an image?
                                if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                                {
                                    ExportAsPngImage(xObject, outputFolder, ref imageCount);
                                }
                            }
                        }
                    }
                }

            }
        }
        private Image CropToBound(Image img, Rectangle cropRect, double rotation)
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