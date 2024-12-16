using System.IO;

namespace Scanned_Page_Sorter.Lib.PDF
{
    internal class CommentsExporter
    {
        private readonly string _saveLocation;
        private readonly PageMetadataMap _imageMetadataMap;
        public CommentsExporter(string saveLocation, PageMetadataMap imageMetadataMap)
        {
            _saveLocation = saveLocation;
            _imageMetadataMap = imageMetadataMap;
        }
        public void export()
        {


            using (StreamWriter sw = new StreamWriter(_saveLocation))
            {
                foreach (var item in _imageMetadataMap.Values)
                {
                    sw.WriteLine(item.ToString());
                    if (item.Comment != null && item.Comment.Length > 0)
                    {
                        string page = item.Title.Contains(".") ? item.Title.Split('.')[0] : item.Title;
                        sw.WriteLine($"Page: {page} : {item.Comment}");
                    }
                }
            }
        }

    }
}
