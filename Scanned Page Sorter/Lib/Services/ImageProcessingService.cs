using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Scanned_Page_Sorter.Lib.models;
using Scanned_Page_Sorter.Lib.PDF;

namespace Scanned_Page_Sorter.Lib.Services
{
    /// <summary>
    /// Service responsible for handling image extraction and processing operations
    /// </summary>
    public class ImageProcessingService
    {
        /// <summary>
        /// Extracts images from a PDF file to the specified directory
        /// </summary>
        /// <param name="sourceDocument">Source document containing PDF information</param>
        /// <param name="outputDirectory">Directory where images will be extracted</param>
        /// <returns>True if extraction successful, false otherwise</returns>
        public bool ExtractImagesFromPdf(SourceDocument sourceDocument, string outputDirectory = null)
        {
            try
            {
                if (sourceDocument?.SourcePath == null)
                {
                    MessageBox.Show("Invalid source document", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string targetDirectory = outputDirectory ?? GetDefaultImageDirectory(sourceDocument.Title);
                
                if (!CreateOrClearDirectory(targetDirectory))
                {
                    MessageBox.Show($"Failed to create output directory: {targetDirectory}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                var pdfParser = new PdfParser(sourceDocument.PageMetadataMap, 
                    sourceDocument.SourcePath, targetDirectory);
                    
                pdfParser.ExtractImages();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting images: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Gets the default directory path for extracted images
        /// </summary>
        /// <param name="documentTitle">Title of the document</param>
        /// <returns>Default directory path</returns>
        private string GetDefaultImageDirectory(string documentTitle)
        {
            return Path.Combine("../../images/", documentTitle);
        }

        /// <summary>
        /// Creates a directory and clears its contents if it already exists
        /// </summary>
        /// <param name="directoryPath">Path to the directory</param>
        /// <returns>True if successful, false otherwise</returns>
        private bool CreateOrClearDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    var directoryInfo = new DirectoryInfo(directoryPath);
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // Skip files that can't be deleted
                            Console.WriteLine($"Cannot delete file: {file.Name}");
                        }
                    }
                }
                else
                {
                    Directory.CreateDirectory(directoryPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating/clearing directory {directoryPath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates if the provided file is a valid PDF
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns>True if valid PDF, false otherwise</returns>
        public bool IsValidPdf(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath) &&
                   File.Exists(filePath) &&
                   Path.GetExtension(filePath).ToLowerInvariant() == ".pdf";
        }

        /// <summary>
        /// Rotates an image by the specified angle and updates the metadata
        /// </summary>
        /// <param name="imagePath">Path to the image file</param>
        /// <param name="rotationAngle">Angle to rotate in degrees</param>
        /// <param name="metadata">Page metadata to update</param>
        /// <returns>True if rotation successful, false otherwise</returns>
        public bool RotateImage(string imagePath, float rotationAngle, PageMetadata metadata)
        {
            try
            {
                if (metadata == null)
                {
                    return false;
                }

                metadata.Rotate += rotationAngle;
                
                // Normalize rotation to 0-360 range
                while (metadata.Rotate >= 360) metadata.Rotate -= 360;
                while (metadata.Rotate < 0) metadata.Rotate += 360;

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rotating image {imagePath}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Validates if a path is a valid file or directory
        /// </summary>
        /// <param name="path">Path to validate</param>
        /// <returns>True if path exists as file or directory</returns>
        public bool IsValidPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && 
                   (File.Exists(path) || Directory.Exists(path));
        }
    }
}