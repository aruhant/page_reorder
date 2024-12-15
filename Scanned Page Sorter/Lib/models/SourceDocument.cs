using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanned_Page_Sorter.Lib.models
{
    enum DocumentType
    {
        PDF,
        ImageFolder
    }
    internal class SourceDocument
    {
         private string _source;
        private DocumentType _documentType;
         public string SourcePath { get => _source; }
        public DocumentType DocumentType { get => _documentType; }
        public PageMetadataMap pageMetadataMap { get; }

        public string Title { get => Path.GetFileNameWithoutExtension(_source); }
        public string PDFSaveAs { get => _documentType == DocumentType.PDF ? Path.GetDirectoryName(_source) + "\\Reordered-" + Path.GetFileName(_source) : _source + ".pdf"; }
        public string TXTSaveAs { get => _documentType == DocumentType.PDF ? Path.GetDirectoryName(_source) + "\\Reordered-" + Path.GetFileNameWithoutExtension(_source) + ".txt" : _source + ".txt"; }

        public SourceDocument(string file)
        {
            if (System.IO.File.Exists(file))
            {
                _source = file;
                _documentType = DocumentType.PDF;
            }
            else if (System.IO.Directory.Exists(file))
            {
                _source = file;
                _documentType = DocumentType.ImageFolder;
            }
            else
            {
                MessageBox.Show("Invalid file path");
            }
                pageMetadataMap = new PageMetadataMap(Path.GetDirectoryName(_source));
        }
        } 
}