using System;
using System.Collections.Generic;
using System.Drawing;

namespace Scanned_Page_Sorter
{

    enum PageType
    {
        Cover,
        Content,
        BackCover,
        Unknown
    }
    internal class PageMetadata
    {
        public string Comment;
        public float Rotate;
        public int Orientation;
        public string Title;
        public Rectangle ClipRect;
        public Rectangle MediaRect;
        public PageType PageType;
        public int PageNumber, OriginalPageNumber;
        public string FileName { get; }


        public iText.Kernel.Geom.PageSize PageSize => ClipRect.Width == 0 ? new iText.Kernel.Geom.PageSize(MediaRect.Width, MediaRect.Height) : new iText.Kernel.Geom.PageSize(ClipRect.Width, ClipRect.Height);
        public iText.Kernel.Geom.Rectangle ClipBox => ClipRect.Width == 0 ? MediaBox : new iText.Kernel.Geom.Rectangle(ClipRect.Width, ClipRect.Height);
        public iText.Kernel.Geom.Rectangle MediaBox => new iText.Kernel.Geom.Rectangle(MediaRect.Width, MediaRect.Height);


        public PageMetadata(string fileName, string title = null, PageType pageType = PageType.Content, int pageNumber = -1, int originalPageNumber = -1

            )
        {
            Title = title;
            FileName = fileName;
            Comment = "";
            Rotate = 0;
            Orientation = 0;
            PageType = pageType;
            PageNumber = pageNumber;

            OriginalPageNumber = originalPageNumber;
        }

        public override string ToString()
        {
            return $"FileName: {FileName}, Title: {Title}, Comment: {Comment}, Rotate: {Rotate}, Orientation: {Orientation}, PageNumber: {PageNumber}, OriginalPageNumber: {OriginalPageNumber}, ClipRect: {ClipRect}, MediaRect: {MediaRect}";
        }

        //public Bitmap getRoatatedThumbnail() {        }

    }
    internal class PageMetadataMap
    {
        private readonly string _parentFolder;
        public PageMetadataMap(string parentFolder)
        {
            _parentFolder = parentFolder;
        }
        internal IEnumerable<int> Keys { get => map.Keys; }
        internal IEnumerable<PageMetadata> Values { get => map.Values; }
        private readonly Dictionary<int, PageMetadata> map = new Dictionary<int, PageMetadata>();
        public PageMetadata this[int key]
        {
            get
            {
                if (!map.ContainsKey(key)) map[key] = new PageMetadata(key.ToString());
                return map[key];
            }
            set
            {
                map[key] = value;
            }
        }

        internal void SyncPageNumbers(List<int> pageNumbers)
        {
            int i = 0;
            foreach (int p  in pageNumbers)
            {
                map[p].PageNumber = i++;
            }
        }
    }
}
