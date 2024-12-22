using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using iText.Kernel.Pdf;

namespace Scanned_Page_Sorter.Lib.PDF
{
    internal class PdfParser
    {
        public PdfParser(PageMetadataMap imageMetadataMap, string sourcePdf, string outputFolder)
        {
            _imageMetadataMap = imageMetadataMap;
            _sourcePdf = sourcePdf;
            _outputFolder = outputFolder;
        }
        private readonly string _sourcePdf, _outputFolder;
        private readonly PageMetadataMap _imageMetadataMap;

        private readonly Hashtable _processedObjects = new Hashtable();
        private int _rotation = 0;
        private int _imageNumber = 0;
        private Rectangle _clip = Rectangle.Empty;
        private Rectangle _mediabox = Rectangle.Empty;
        public void ExtractImages()
        {
            try
            {
                PdfReader reader = new PdfReader(_sourcePdf);
                PdfDocument pdfDoc = new PdfDocument(reader);
                _imageNumber = 0;
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var currentPage = pdfDoc.GetPage(i);
                    _rotation = currentPage.GetRotation();
                    _clip = Rectangle.Empty;
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
            if (obj == null || (obj.GetIndirectReference() != null && _processedObjects.ContainsKey(obj.GetIndirectReference()))) return;
            if (name != null) ProcessName(name, obj);
            if (obj.GetIndirectReference() != null) _processedObjects.Add(obj.GetIndirectReference(), obj);
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
                    string title = $"{_imageNumber++:D3}.jpg";
                    string fileName;
                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        fileName = Path.Combine(outputFolder, title);
                        using (Image img = Image.FromStream(ms))
                        {
                            var croppedImg = ImageUtils.CropToBoundsAndRotate(img, _clip, _mediabox, 0);
                            croppedImg.Save(fileName, ImageFormat.Jpeg);
                        }
                    }
                    PageMetadata metadata = new PageMetadata(fileName, title, originalPageNumber: _imageNumber);
                    _imageMetadataMap[title] = metadata;
                    metadata.ClipRect = _clip;
                    metadata.MediaRect = _mediabox;
                    metadata.Orientation = _rotation;

                    Console.WriteLine(name + " image: " + _imageNumber + "Rotation: " + _rotation + "Mediabox " + _mediabox + " clipRect " + _clip + " r ");
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
                    _clip = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/MediaBox":
                    _mediabox = ConvertToRectangle(obj as PdfArray);
                    return 1;
                case "/Type":
                    if (obj.ToString() == "/Page")
                    {
                        _mediabox = Rectangle.Empty;
                        _rotation = 0;
                        _clip = Rectangle.Empty;
                    }
                    return 1;
                case "/Rotate":
                    _rotation = int.Parse(obj.ToString());
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
