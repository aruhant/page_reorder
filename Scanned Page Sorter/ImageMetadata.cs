using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scanned_Page_Sorter
{
    internal class ImageMetadata
    {
        public string Comment { get; set; }
        public float Rotate { get; set; }
        public int Orientation { get; set; }
        public string Title { get; }

        private string parentFolder;
        public ImageMetadata(string parentFolder)
        {
            this.parentFolder = parentFolder;
        }

        public ImageMetadata(string parentFolder, string title) : this(parentFolder)
        {
            Title = title;
            Comment = "";
            Rotate = 0;
            Orientation = 0;
        }

        //public Bitmap getRoatatedThumbnail() {        }

    }
    internal class ImageMetadataMap
    {
        private Dictionary<string, ImageMetadata> map = new Dictionary<string, ImageMetadata>();
        public ImageMetadata this[string key]
        {
            get
            {
                if (!map.ContainsKey(key))    map[key] = new ImageMetadata(key);                
                return map[key];
            }
            set
            {
                map[key] = value;
            }
        }

    }
}
