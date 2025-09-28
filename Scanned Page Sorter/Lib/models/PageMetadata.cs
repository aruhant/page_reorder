using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using Scanned_Page_Sorter.Lib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Scanned_Page_Sorter
{
    /// <summary>
    /// Defines the type of page in the document
    /// </summary>
    public enum PageType
    {
        Cover,
        Content,
        MissingContent,
        BackCover,
        Unknown
    }

    /// <summary>
    /// Represents metadata for a single page in the document
    /// </summary>
    public class PageMetadata
    {
        #region Private Fields
        private string _comment;
        private float _rotate;
        private string _title;
        private Rectangle _clipRect;
        private Rectangle _mediaRect;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the comment associated with this page
        /// </summary>
        public string Comment
        {
            get => _comment ?? string.Empty;
            set => _comment = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the rotation angle for this page in degrees
        /// </summary>
        public float Rotate
        {
            get => _rotate;
            set => _rotate = NormalizeRotation(value);
        }

        /// <summary>
        /// Gets or sets the orientation of the page
        /// </summary>
        public int Orientation { get; set; }

        /// <summary>
        /// Gets or sets the title of the page
        /// </summary>
        public string Title
        {
            get => _title ?? string.Empty;
            set => _title = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the clipping rectangle for this page
        /// </summary>
        public Rectangle ClipRect
        {
            get => _clipRect;
            set => _clipRect = ValidateRectangle(value);
        }

        /// <summary>
        /// Gets or sets the media rectangle for this page
        /// </summary>
        public Rectangle MediaRect
        {
            get => _mediaRect;
            set => _mediaRect = ValidateRectangle(value);
        }

        /// <summary>
        /// Gets or sets the type of this page
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// Gets or sets the current page number in the document
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the original page number from the source document
        /// </summary>
        public int OriginalPageNumber { get; set; }

        /// <summary>
        /// Gets or sets the blur indicator (0 = not blurred, 1 = blurred)
        /// </summary>
        public int Blurred { get; set; }

        /// <summary>
        /// Gets a formatted string containing flags and comments for this page
        /// </summary>
        public string Flags
        {
            get
            {
                var flags = new List<string>();
                
                if (Blurred != 0)
                {
                    flags.Add("Blurred");
                }
                
                if (!string.IsNullOrWhiteSpace(Comment))
                {
                    flags.Add(Comment);
                }
                
                return string.Join(" ", flags);
            }
        }

        /// <summary>
        /// Gets the filename associated with this page
        /// </summary>
        public string FileName { get; }
        #endregion

        #region PDF-related Properties
        /// <summary>
        /// Gets the page size without rotation applied
        /// </summary>
        public iText.Kernel.Geom.PageSize PageSizeWithoutRotation
        {
            get
            {
                var width = ClipRect.Width == 0 ? MediaRect.Width : ClipRect.Width;
                var height = ClipRect.Height == 0 ? MediaRect.Height : ClipRect.Height;
                
                return new iText.Kernel.Geom.PageSize(width, height);
            }
        }

        /// <summary>
        /// Gets the page size with rotation applied
        /// </summary>
        public iText.Kernel.Geom.PageSize PageSize
        {
            get
            {
                var pageSize = PageSizeWithoutRotation;
                var sourceRect = new Rectangle(
                    (int)pageSize.GetLeft(),
                    (int)pageSize.GetBottom(),
                    (int)pageSize.GetWidth(),
                    (int)pageSize.GetHeight());

                var rotatedRect = ImageUtils.GetBoundingRectangleAfterRotation(sourceRect, -Rotate);
                return new iText.Kernel.Geom.PageSize(rotatedRect.Width, rotatedRect.Height);
            }
        }

        /// <summary>
        /// Gets the clip box for PDF operations
        /// </summary>
        public iText.Kernel.Geom.Rectangle ClipBox =>
            ClipRect.Width == 0 ? MediaBox : new iText.Kernel.Geom.Rectangle(ClipRect.Width, ClipRect.Height);

        /// <summary>
        /// Gets the media box for PDF operations
        /// </summary>
        public iText.Kernel.Geom.Rectangle MediaBox =>
            new iText.Kernel.Geom.Rectangle(MediaRect.Width, MediaRect.Height);
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of PageMetadata
        /// </summary>
        /// <param name="fileName">The filename of the page</param>
        /// <param name="title">Optional title for the page</param>
        /// <param name="pageType">Type of the page</param>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="originalPageNumber">Original page number from source</param>
        /// <exception cref="ArgumentException">Thrown when fileName is null or empty</exception>
        public PageMetadata(string fileName, string title = null, PageType pageType = PageType.Content, 
            int pageNumber = -1, int originalPageNumber = -1)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Filename cannot be null or empty", nameof(fileName));
            }

            FileName = fileName;
            Title = title ?? string.Empty;
            Comment = string.Empty;
            Rotate = 0;
            Orientation = 0;
            PageType = pageType;
            PageNumber = pageNumber;
            OriginalPageNumber = originalPageNumber;
            Blurred = 0;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Normalizes rotation angle to 0-360 degrees range
        /// </summary>
        /// <param name="angle">The angle to normalize</param>
        /// <returns>Normalized angle between 0 and 360 degrees</returns>
        private float NormalizeRotation(float angle)
        {
            while (angle >= 360) angle -= 360;
            while (angle < 0) angle += 360;
            return angle;
        }

        /// <summary>
        /// Validates that a rectangle has non-negative dimensions
        /// </summary>
        /// <param name="rectangle">The rectangle to validate</param>
        /// <returns>Valid rectangle</returns>
        private Rectangle ValidateRectangle(Rectangle rectangle)
        {
            if (rectangle.Width < 0 || rectangle.Height < 0)
            {
                Console.WriteLine($"Warning: Invalid rectangle dimensions - Width: {rectangle.Width}, Height: {rectangle.Height}");
                return Rectangle.Empty;
            }
            return rectangle;
        }

        /// <summary>
        /// Creates a copy of this PageMetadata instance
        /// </summary>
        /// <returns>A new PageMetadata instance with the same values</returns>
        public PageMetadata Clone()
        {
            return new PageMetadata(FileName, Title, PageType, PageNumber, OriginalPageNumber)
            {
                Comment = Comment,
                Rotate = Rotate,
                Orientation = Orientation,
                ClipRect = ClipRect,
                MediaRect = MediaRect,
                Blurred = Blurred
            };
        }

        /// <summary>
        /// Validates the current state of the PageMetadata
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(FileName) &&
                   MediaRect.Width >= 0 && MediaRect.Height >= 0 &&
                   ClipRect.Width >= 0 && ClipRect.Height >= 0;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns a string representation of this PageMetadata
        /// </summary>
        /// <returns>Formatted string with key properties</returns>
        public override string ToString()
        {
            return $"FileName: {FileName}, Title: {Title}, Comment: {Comment}, " +
                   $"Rotate: {Rotate}, Orientation: {Orientation}, PageNumber: {PageNumber}, " +
                   $"OriginalPageNumber: {OriginalPageNumber}, ClipRect: {ClipRect}, MediaRect: {MediaRect}";
        }

        /// <summary>
        /// Determines whether the specified object is equal to this instance
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is PageMetadata other)
            {
                return FileName.Equals(other.FileName, StringComparison.OrdinalIgnoreCase) &&
                       PageNumber == other.PageNumber;
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (FileName?.ToLowerInvariant()?.GetHashCode() ?? 0);
                hash = hash * 23 + PageNumber.GetHashCode();
                return hash;
            }
        }
        #endregion
    }

    /// <summary>
    /// Thread-safe collection for managing page metadata mappings
    /// </summary>
    public class PageMetadataMap
    {
        #region Private Fields
        private readonly string _parentFolder;
        private readonly Dictionary<string, PageMetadata> _map = new Dictionary<string, PageMetadata>();
        private readonly object _lock = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets all keys in the metadata map
        /// </summary>
        public IEnumerable<string> Keys
        {
            get
            {
                lock (_lock)
                {
                    return new List<string>(_map.Keys);
                }
            }
        }

        /// <summary>
        /// Gets all values in the metadata map
        /// </summary>
        public IEnumerable<PageMetadata> Values
        {
            get
            {
                lock (_lock)
                {
                    return new List<PageMetadata>(_map.Values);
                }
            }
        }

        /// <summary>
        /// Gets the number of items in the map
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _map.Count;
                }
            }
        }
        #endregion

        #region Indexer
        /// <summary>
        /// Gets or sets page metadata by filename
        /// </summary>
        /// <param name="fileName">The filename key</param>
        /// <returns>PageMetadata if found, null otherwise</returns>
        public PageMetadata this[string fileName]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    return null;
                }

                lock (_lock)
                {
                    if (!_map.TryGetValue(fileName, out PageMetadata metadata))
                    {
                        Console.WriteLine($"PageMetadata not found for: {fileName}");
                        return null;
                    }
                    return metadata;
                }
            }
            set
            {
                if (string.IsNullOrWhiteSpace(fileName) || value == null)
                {
                    return;
                }

                lock (_lock)
                {
                    _map[fileName] = value;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new PageMetadataMap
        /// </summary>
        /// <param name="parentFolder">The parent folder path</param>
        public PageMetadataMap(string parentFolder)
        {
            _parentFolder = parentFolder ?? throw new ArgumentNullException(nameof(parentFolder));
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Synchronizes page numbers based on the provided file name order
        /// </summary>
        /// <param name="fileNames">Ordered list of file names</param>
        /// <exception cref="ArgumentNullException">Thrown when fileNames is null</exception>
        public void SyncPageNumbers(List<string> fileNames)
        {
            if (fileNames == null)
            {
                throw new ArgumentNullException(nameof(fileNames));
            }

            lock (_lock)
            {
                for (int i = 0; i < fileNames.Count; i++)
                {
                    var fileName = fileNames[i];
                    if (_map.TryGetValue(fileName, out PageMetadata metadata))
                    {
                        metadata.PageNumber = i;
                    }
                }
            }
        }

        /// <summary>
        /// Adds or updates metadata for a file
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <param name="metadata">The metadata to add or update</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool AddOrUpdate(string fileName, PageMetadata metadata)
        {
            if (string.IsNullOrWhiteSpace(fileName) || metadata == null)
            {
                return false;
            }

            try
            {
                lock (_lock)
                {
                    _map[fileName] = metadata;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding/updating metadata for {fileName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Removes metadata for a file
        /// </summary>
        /// <param name="fileName">The filename to remove</param>
        /// <returns>True if removed, false if not found</returns>
        public bool Remove(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            lock (_lock)
            {
                return _map.Remove(fileName);
            }
        }

        /// <summary>
        /// Checks if metadata exists for a given filename
        /// </summary>
        /// <param name="fileName">The filename to check</param>
        /// <returns>True if exists, false otherwise</returns>
        public bool ContainsKey(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            lock (_lock)
            {
                return _map.ContainsKey(fileName);
            }
        }

        /// <summary>
        /// Clears all metadata from the map
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _map.Clear();
            }
        }
        #endregion
    }
}
