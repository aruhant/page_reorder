using System.Drawing;

namespace ScanSort.Services;

public interface IImageService
{
    /// <summary>
    /// Rotates an image by combining orientation and fine rotation.
    /// Returns a new Bitmap — caller must dispose.
    /// </summary>
    Bitmap RotateImage(Image source, int orientation, float rotate);

    /// <summary>
    /// Crops an image from the source using CropBox/MediaBox scaling, then rotates.
    /// </summary>
    Image CropToBoundsAndRotate(Image source, Rectangle cropRect, Rectangle mediaRect, double rotation);

    /// <summary>
    /// Gets the pixel dimensions of an image file.
    /// </summary>
    Rectangle GetImageRect(string filePath);

    /// <summary>
    /// Returns the bounding rectangle after rotating a rectangle by the given angle.
    /// </summary>
    Rectangle GetBoundingRectAfterRotation(Rectangle rect, double angleDegrees);
}
