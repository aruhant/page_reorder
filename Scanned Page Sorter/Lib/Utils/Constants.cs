using System;

namespace Scanned_Page_Sorter.Lib.Utils
{
    /// <summary>
    /// Application constants to avoid magic numbers and strings
    /// </summary>
    public static class Constants
    {
        // File formats
        public const string PDF_FILTER = "PDF Files|*.pdf";
        public const string PDF_FILE_DIALOG_TITLE = "Select a PDF File";
        public const string FOLDER_SELECTION_FILENAME = "Folder Selection.";
        
        // Image settings
        public const string JPEG_EXTENSION = ".jpg";
        public const string PDF_EXTENSION = ".pdf";
        public const string TXT_EXTENSION = ".txt";
        public const string DEFAULT_IMAGE_FORMAT = "jpg";
        
        // Directory paths
        public const string DEFAULT_IMAGES_FOLDER = "../../images/";
        public const string REORDERED_PREFIX = "Reordered-";
        
        // UI constants
        public const string DEFAULT_FONT_FAMILY = "Arial";
        public const float DEFAULT_FONT_SIZE = 6f;
        public const int SCROLLBAR_OFFSET = 32;
        
        // File naming
        public const string IMAGE_FILE_FORMAT = "{0:D3}.jpg";
        public const int DEFAULT_PAGE_NUMBER = -1;
        
        // Configuration keys
        public const string DUPLEX_SELECT_MODE_KEY = "DuplexSelectMode";
        public const string HAS_COVER_KEY = "HasCover";
        public const string DEFAULT_DUPLEX_MODE = "false";
        public const string DEFAULT_HAS_COVER = "true";
        
        // Error messages
        public const string INVALID_FILE_PATH_MESSAGE = "Invalid file path";
        public const string INVALID_MEDIABOX_ARRAY_MESSAGE = "Invalid MediaBox array size.";
        public const int EXPECTED_MEDIABOX_SIZE = 4;
    }
}