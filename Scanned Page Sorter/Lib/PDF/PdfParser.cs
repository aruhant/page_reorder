using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using System.Windows.Forms;
using Scanned_Page_Sorter.Lib;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
 
namespace Scanned_Page_Sorter.Lib.PDF
{
    internal class PdfParser
    {
        public PdfParser(PageMetadataMap imageMetadataMap, string sourcePdf, string outputFolder) {
            _imageMetadataMap = imageMetadataMap;
            _sourcePdf = sourcePdf;
            _outputFolder = outputFolder;
        }
        private string _sourcePdf, _outputFolder;
        private PageMetadataMap _imageMetadataMap;

        private Hashtable processedObjects = new Hashtable();
        int rotation = 0;
        int imageNumber = 0;
        private Rectangle clip = Rectangle.Empty;
        private Rectangle mediabox = Rectangle.Empty;
        public void ExtractImages()
        {
            PdfReader reader = new PdfReader(_sourcePdf);
            try
            {
                PdfDocument pdfDoc = new PdfDocument(reader);
                imageNumber = 0;
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var currentPage = pdfDoc.GetPage(i);
                    rotation = currentPage.GetRotation();
                    clip = Rectangle.Empty;
                    Console.WriteLine("GetPageSizeWithRotation " + currentPage.GetPageSizeWithRotation());
                    Console.WriteLine("GetPageSize " + currentPage.GetPageSize());
                    Console.WriteLine("GetRotation " + currentPage.GetRotation());
                    iText.Kernel.Pdf.PdfDictionary currentPageObjects = currentPage.GetPdfObject();
                    pdfDoc.GetNumberOfPdfObjects();
                    int numberOfPdfObject = currentPageObjects.Size();
                    foreach (PdfObject currentPageObject in currentPageObjects.Values())
                    {
                        ProcessPDFObject(currentPageObject, _outputFolder);
                    }


                    //imageListner.SetCurrentPage(i, pdfDoc.GetPage(i)); // Corrected method name                    
                    //parser.ProcessPageContent(pdfDoc.GetPage(i));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        private void ProcessPDFObject(PdfObject obj, string outputFolder, String name = null) // optional name parameter
        {
            if (obj == null || (obj.GetIndirectReference() != null && processedObjects.ContainsKey(obj.GetIndirectReference()))) return;
            if (name != null) ProcessName(name, obj);
            if (obj.GetIndirectReference() != null) processedObjects.Add(obj.GetIndirectReference(), obj);
            switch (obj.GetObjectType())
            {
                case PdfObject.ARRAY:
                    var a = (PdfArray)obj;
                    foreach (var child in a) ProcessPDFObject(child, outputFolder);
                    break;
                case PdfObject.DICTIONARY:
                    foreach (var key in ((PdfDictionary)obj).KeySet())
                    {
                        if (((PdfDictionary)obj).Get(key).IsNumber())
                            ProcessPDFObject(((PdfDictionary)obj).Get(key), outputFolder, key.ToString());
                    }
                    foreach (var key in ((PdfDictionary)obj).KeySet())
                    {
                        if (!((PdfDictionary)obj).Get(key).IsNumber())
                            ProcessPDFObject(((PdfDictionary)obj).Get(key), outputFolder, key.ToString());
                    }
                    break;
                case PdfObject.INDIRECT_REFERENCE:
                    break;
                case PdfObject.STREAM:
                    var PDFStremObj = (PdfStream)obj;
                    PdfObject subtype = PDFStremObj.Get(PdfName.Subtype);
                    if ((subtype == null) || subtype.ToString() != PdfName.Image.ToString()) break;
                    byte[] data = (obj as PdfStream).GetBytes();
                    string title = $"{imageNumber++:D3}.jpg";
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        var fileName = Path.Combine(outputFolder, title);
                        using (Image img = Image.FromStream(ms))
                        {
                            var croppedImg = ImageUtils.CropToBoundsAndRotate(img, clip, mediabox, 0);
                            croppedImg.Save(fileName, ImageFormat.Jpeg);
                        }
                    }
                    PageMetadata metadata = new PageMetadata(outputFolder, title);
                    _imageMetadataMap[title] = metadata;
                    metadata.clipRect = clip;
                    metadata.mediaRect = mediabox;
                    metadata.orientation = rotation;

                    Console.WriteLine(name + " image: " + imageNumber + "Rotation: " + rotation + "Mediabox " + mediabox + " clipRect " + clip + " r ");
                    break;
                case PdfObject.NAME:
                    break;
                case PdfObject.NUMBER:
                    break;
                default:
                    Console.WriteLine("-->" + obj.GetType());
                    break;
            }
        }



        private int ProcessName(string name, PdfObject obj)
        {
            switch (name)
            {
                case "/CropBox":
                    clip = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/MediaBox":
                    mediabox = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/Type":
                    if (obj.ToString() == "/Page")
                    {
                        mediabox = Rectangle.Empty;
                        rotation = 0;
                        clip = Rectangle.Empty;
                    }
                    return 1;
                case "/Rotate":
                    rotation = int.Parse(obj.ToString());
                    return 1;
                default:
                    return 0;

            }
        }

        private Rectangle ConvertToRectangle(PdfArray array)
        {
            if (array.Size() != 4)
                throw new ArgumentException("Invalid MediaBox array size.");

            float x = ((PdfNumber)array.Get(0)).FloatValue();
            float y = ((PdfNumber)array.Get(1)).FloatValue();
            float width = ((PdfNumber)array.Get(2)).FloatValue() - x;
            float height = ((PdfNumber)array.Get(3)).FloatValue() - y;

            return new Rectangle((int)x, (int)y, (int)width, (int)height);
        }

    }
}
