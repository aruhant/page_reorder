using System;
using System.IO;
using System.Windows.Forms;

namespace Scanned_Page_Sorter.Lib.Utils
{
    /// <summary>
    /// Service for handling file and folder operations with proper error handling
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Opens a file dialog to select a PDF file
        /// </summary>
        /// <returns>Selected file path or null if cancelled</returns>
        public string SelectPdfFile()
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = Constants.PDF_FILTER;
                    openFileDialog.Title = Constants.PDF_FILE_DIALOG_TITLE;
                    
                    return openFileDialog.ShowDialog() == DialogResult.OK 
                        ? openFileDialog.FileName 
                        : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file dialog: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Opens a folder browser dialog
        /// </summary>
        /// <returns>Selected folder path or null if cancelled</returns>
        public string SelectFolder()
        {
            try
            {
                using (var folderBrowser = new OpenFileDialog())
                {
                    folderBrowser.ValidateNames = false;
                    folderBrowser.CheckFileExists = false;
                    folderBrowser.CheckPathExists = true;
                    folderBrowser.FileName = Constants.FOLDER_SELECTION_FILENAME;
                    
                    return folderBrowser.ShowDialog() == DialogResult.OK 
                        ? Path.GetDirectoryName(folderBrowser.FileName) 
                        : null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder dialog: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Creates a directory and clears its contents if it already exists
        /// </summary>
        /// <param name="directoryPath">Path to the directory</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool CreateOrClearDirectory(string directoryPath)
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