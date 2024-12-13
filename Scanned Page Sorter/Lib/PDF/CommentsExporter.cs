using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Pdf;
using Manina.Windows.Forms;
using System.Windows.Forms;
using iText.Layout;
using System.IO;

namespace Scanned_Page_Sorter.Lib.PDF
{
    internal class CommentsExporter
    {
        private string _saveLocation;
        private PageMetadataMap _imageMetadataMap;
        public CommentsExporter(  string saveLocation, PageMetadataMap imageMetadataMap)
        {
             this._saveLocation = saveLocation;
            _imageMetadataMap = imageMetadataMap;
        }
        public void export()
        {


             using (StreamWriter sw = new StreamWriter(_saveLocation))
            {
                foreach (var item in _imageMetadataMap.Values)
                {
                    if (item.comment != null && item.comment.Length > 0)
                    {
                        string page = item.title.Contains(".") ? item.title.Split('.')[0] : item.title;
                        sw.WriteLine($"Page: {page} : {item.comment}");
                    }
                }
            }
        }

    }
}
