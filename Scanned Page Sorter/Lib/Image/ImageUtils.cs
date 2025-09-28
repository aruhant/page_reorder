using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using iText.Layout.Element;
using Scanned_Page_Sorter;

namespace Scanned_Page_Sorter.Lib
{
    /// <summary>
    /// Utility class for image processing operations
    /// </summary>
    public static class ImageUtils
    {
        #region Image Rotation Methods
        /// <summary>
        /// Rotates an image by the specified orientation and rotation angles
        /// </summary>
        /// <param name="img">The image to rotate</param>
        /// <param name="orientation">Orientation angle in degrees</param>
        /// <param name="rotation">Additional rotation angle in degrees</param>
        /// <returns>Rotated image or original if no rotation needed</returns>
        /// <exception cref="ArgumentNullException">Thrown when img is null</exception>
        public static System.Drawing.Image RotateImage(System.Drawing.Image img, int orientation, float rotation)
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            try
            {
                var totalRotation = orientation + rotation;
                if (Math.Abs(totalRotation) < 0.01) // No rotation needed
                    return CloneImage(img);

                return CreateRotatedImage(img, totalRotation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rotating image: {ex.Message}");
                return CloneImage(img); // Return original on error
            }
        }

        /// <summary>
        /// Creates a rotated copy of the image
        /// </summary>
        /// <param name="sourceImage">Source image to rotate</param>
        /// <param name="angle">Rotation angle in degrees</param>
        /// <returns>Rotated image</returns>
        private static Bitmap CreateRotatedImage(System.Drawing.Image sourceImage, double angle)
        {
            // Create initial bitmap copy
            using (var sourceBitmap = CloneImage(sourceImage) as Bitmap)
            {
                if (sourceBitmap == null)
                    return new Bitmap(sourceImage.Width, sourceImage.Height);

                var radianAngle = angle * Math.PI / 180.0;
                var cosA = Math.Abs(Math.Cos(radianAngle));
                var sinA = Math.Abs(Math.Sin(radianAngle));

                var newWidth = (int)(cosA * sourceBitmap.Width + sinA * sourceBitmap.Height);
                var newHeight = (int)(cosA * sourceBitmap.Height + sinA * sourceBitmap.Width);

                var rotatedBitmap = new Bitmap(newWidth, newHeight, sourceBitmap.PixelFormat);
                
                try
                {
                    rotatedBitmap.SetResolution(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);

                    using (var graphics = Graphics.FromImage(rotatedBitmap))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                        graphics.TranslateTransform(newWidth / 2f, newHeight / 2f);
                        graphics.RotateTransform((float)angle);
                        graphics.TranslateTransform(-sourceBitmap.Width / 2f, -sourceBitmap.Height / 2f);
                        graphics.DrawImage(sourceBitmap, Point.Empty);
                    }

                    return rotatedBitmap;
                }
                catch
                {
                    rotatedBitmap?.Dispose();
                    throw;
                }
            }
        }
        #endregion

        #region Crop and Rotate Methods
        /// <summary>
        /// Crops an image to specified bounds and rotates it
        /// </summary>
        /// <param name="img">Source image</param>
        /// <param name="cropRect">Rectangle to crop to</param>
        /// <param name="mediaRect">Media rectangle for scaling calculations</param>
        /// <param name="rotation">Rotation angle in degrees</param>
        /// <returns>Cropped and rotated image</returns>
        /// <exception cref="ArgumentNullException">Thrown when img is null</exception>
        public static System.Drawing.Image CropToBoundsAndRotate(System.Drawing.Image img, Rectangle cropRect, 
            Rectangle mediaRect, double rotation)
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img));

            try
            {
                Console.WriteLine($"\n**************************\nRotation: {rotation}\nCrop: {cropRect}\nMedia: {mediaRect}");
                
                var adjustedCropRect = CalculateAdjustedCropRect(img, cropRect, mediaRect);
                var croppedImage = CreateCroppedImage(img, adjustedCropRect);
                
                if (Math.Abs(rotation) < 0.01) // No rotation needed
                    return croppedImage;

                using (croppedImage)
                {
                    return CreateRotatedImage(croppedImage, rotation);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CropToBoundsAndRotate: {ex.Message}");
                return CloneImage(img);
            }
        }

        /// <summary>
        /// Calculates the adjusted crop rectangle based on scaling factors
        /// </summary>
        /// <param name="sourceImage">Source image</param>
        /// <param name="cropRect">Original crop rectangle</param>
        /// <param name="mediaRect">Media rectangle</param>
        /// <returns>Adjusted crop rectangle</returns>
        private static Rectangle CalculateAdjustedCropRect(System.Drawing.Image sourceImage, Rectangle cropRect, Rectangle mediaRect)
        {
            if (cropRect.Width == 0 || cropRect.Height == 0)
            {
                return new Rectangle(0, 0, sourceImage.Width, sourceImage.Height);
            }

            var scaleX = (double)sourceImage.Width / Math.Max(mediaRect.Width, 1);
            var scaleY = (double)sourceImage.Height / Math.Max(mediaRect.Height, 1);
            
            Console.WriteLine($"Scale factors - X: {scaleX}, Y: {scaleY}");

            var adjustedX = (int)(cropRect.X * scaleX);
            var adjustedY = (int)((mediaRect.Height - cropRect.Height - cropRect.Y) * scaleY);
            var adjustedWidth = (int)(cropRect.Width * scaleX);
            var adjustedHeight = (int)(cropRect.Height * scaleY);

            // Ensure the rectangle is within image bounds
            adjustedX = Math.Max(0, Math.Min(adjustedX, sourceImage.Width));
            adjustedY = Math.Max(0, Math.Min(adjustedY, sourceImage.Height));
            adjustedWidth = Math.Min(adjustedWidth, sourceImage.Width - adjustedX);
            adjustedHeight = Math.Min(adjustedHeight, sourceImage.Height - adjustedY);

            return new Rectangle(adjustedX, adjustedY, adjustedWidth, adjustedHeight);
        }

        /// <summary>
        /// Creates a cropped image from the source
        /// </summary>
        /// <param name="sourceImage">Source image</param>
        /// <param name="cropRect">Rectangle to crop</param>
        /// <returns>Cropped bitmap</returns>
        private static Bitmap CreateCroppedImage(System.Drawing.Image sourceImage, Rectangle cropRect)
        {
            var croppedBitmap = new Bitmap(cropRect.Width, cropRect.Height, PixelFormat.Format24bppRgb);
            
            try
            {
                Console.WriteLine($"Source: {sourceImage.Width}x{sourceImage.Height}, Target: {cropRect}");
                
                using (var graphics = Graphics.FromImage(croppedBitmap))
                {
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    
                    var destRect = new Rectangle(0, 0, cropRect.Width, cropRect.Height);
                    graphics.DrawImage(sourceImage, destRect, cropRect, GraphicsUnit.Pixel);
                }

                return croppedBitmap;
            }
            catch
            {
                croppedBitmap?.Dispose();
                throw;
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Calculates the bounding rectangle after rotation
        /// </summary>
        /// <param name="rectangle">Original rectangle</param>
        /// <param name="angle">Rotation angle in degrees</param>
        /// <returns>Bounding rectangle after rotation</returns>
        public static Rectangle GetBoundingRectangleAfterRotation(Rectangle rectangle, double angle)
        {
            try
            {
                var radianAngle = angle * Math.PI / 180.0;
                var cosA = Math.Abs(Math.Cos(radianAngle));
                var sinA = Math.Abs(Math.Sin(radianAngle));

                var newWidth = (int)(cosA * rectangle.Width + sinA * rectangle.Height);
                var newHeight = (int)(cosA * rectangle.Height + sinA * rectangle.Width);

                return new Rectangle(rectangle.X, rectangle.Y, newWidth, newHeight);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating bounding rectangle: {ex.Message}");
                return rectangle; // Return original on error
            }
        }

        /// <summary>
        /// Automatically detects and returns the skew angle of an image
        /// </summary>
        /// <param name="fileName">Path to the image file</param>
        /// <returns>Detected skew angle in degrees, 0 if detection fails</returns>
        public static float AutoDeskew(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !System.IO.File.Exists(fileName))
            {
                Console.WriteLine($"Invalid file path for deskew: {fileName}");
                return 0;
            }

            try
            {
                Console.WriteLine($"Processing deskew for: {fileName}");
                var output = RunUtils.RunExternalExe("deskew.exe", fileName);
                Console.WriteLine($"Deskew output: {output}");

                return ParseDeskewOutput(output);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AutoDeskew for {fileName}: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Parses the output from deskew executable
        /// </summary>
        /// <param name="output">Output string from deskew.exe</param>
        /// <returns>Parsed angle or 0 if parsing fails</returns>
        private static float ParseDeskewOutput(string output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return 0;

            try
            {
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("Skew angle found [deg]:"))
                    {
                        var parts = line.Split(':');
                        if (parts.Length >= 2)
                        {
                            var angleString = parts[1].Trim();
                            if (float.TryParse(angleString, out float parsedAngle))
                            {
                                return parsedAngle;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing deskew output '{output}': {ex.Message}");
            }

            return 0;
        }

        /// <summary>
        /// Gets the dimensions of an image file safely
        /// </summary>
        /// <param name="fullPath">Full path to the image file</param>
        /// <returns>Rectangle representing image dimensions, Empty if error</returns>
        public static Rectangle GetImageRect(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath) || !System.IO.File.Exists(fullPath))
            {
                Console.WriteLine($"Invalid image path: {fullPath}");
                return Rectangle.Empty;
            }

            try
            {
                using (var image = System.Drawing.Image.FromFile(fullPath))
                {
                    return new Rectangle(0, 0, image.Width, image.Height);
                }
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine($"Invalid image format: {fullPath}");
                return Rectangle.Empty;
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine($"Image file not found: {fullPath}");
                return Rectangle.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing image {fullPath}: {ex.Message}");
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// Creates a safe clone of an image
        /// </summary>
        /// <param name="original">Original image to clone</param>
        /// <returns>Cloned image</returns>
        private static Bitmap CloneImage(System.Drawing.Image original)
        {
            if (original == null)
                return null;

            try
            {
                return new Bitmap(original);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cloning image: {ex.Message}");
                return new Bitmap(1, 1); // Return minimal bitmap on error
            }
        }

        /// <summary>
        /// Validates image dimensions
        /// </summary>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <returns>True if dimensions are valid</returns>
        public static bool ValidateImageDimensions(int width, int height)
        {
            const int maxDimension = 65535; // Maximum dimension for most image formats
            const int minDimension = 1;

            return width >= minDimension && height >= minDimension &&
                   width <= maxDimension && height <= maxDimension;
        }

        /// <summary>
        /// Checks if an image format is supported
        /// </summary>
        /// <param name="filePath">Path to the image file</param>
        /// <returns>True if format is supported</returns>
        public static bool IsSupportedImageFormat(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
            var supportedFormats = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".tiff", ".tif", ".gif" };
            
            return Array.Exists(supportedFormats, format => format == extension);
        }
        #endregion
    }
}
