using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanned_Page_Sorter
{
    internal class PageMetadata
    {
        public string comment;
        public float rotate;
        public int orientation;
        public string title;
        public Rectangle clipRect;
        public Rectangle mediaRect;
        public int pageNumber, originalPageNumber;


        public iText.Kernel.Geom.PageSize pageSize => clipRect.Width == 0 ? new iText.Kernel.Geom.PageSize(mediaRect.Width, mediaRect.Height) : new iText.Kernel.Geom.PageSize(clipRect.Width, clipRect.Height);
        public iText.Kernel.Geom.Rectangle clipBox => clipRect.Width == 0 ? mediaBox : new iText.Kernel.Geom.Rectangle(clipRect.Width, clipRect.Height);
        public iText.Kernel.Geom.Rectangle mediaBox => new iText.Kernel.Geom.Rectangle(mediaRect.Width, mediaRect.Height);

        private string parentFolder;


        public PageMetadata(string parentFolder)
        {
            this.parentFolder = parentFolder;
        }

        public PageMetadata(string parentFolder, string title) : this(parentFolder)
        {
            this.title = title;
            comment = "";
            rotate = 0;
            orientation = 0;
        }

        //public Bitmap getRoatatedThumbnail() {        }

    }
    internal class PageMetadataMap
    {
        internal IEnumerable<string> Keys { get => map.Keys; }
        internal IEnumerable<PageMetadata> Values { get => map.Values; }
        private Dictionary<string, PageMetadata> map = new Dictionary<string, PageMetadata>();
        internal void Clear()
        {
            map.Clear();
        }
        public PageMetadata this[string key]
        {
            get
            {
                if (!map.ContainsKey(key)) map[key] = new PageMetadata(key);
                return map[key];
            }
            set
            {
                map[key] = value;
            }
        }
    }
}
