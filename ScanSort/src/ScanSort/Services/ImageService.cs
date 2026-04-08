using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ScanSort.Services;

public class ImageService : IImageService
{
    public Bitmap RotateImage(Image source, int orientation, float rotate)
    {
        ArgumentNullException.ThrowIfNull(source);

        double totalAngle = orientation + rotate;
        if (Math.Abs(totalAngle) < 0.01)
            return new Bitmap(source);

        return CreateRotatedBitmap(source, totalAngle);
    }

    public Image CropToBoundsAndRotate(Image source, Rectangle cropRect, Rectangle mediaRect, double rotation)
    {
        ArgumentNullException.ThrowIfNull(source);

        var adjusted = CalculateAdjustedCropRect(source, cropRect, mediaRect);
        var cropped = CreateCroppedBitmap(source, adjusted);

        if (Math.Abs(rotation) < 0.01)
            return cropped;

        using (cropped)
            return CreateRotatedBitmap(cropped, rotation);
    }

    public Rectangle GetImageRect(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            return Rectangle.Empty;

        try
        {
            using var img = Image.FromFile(filePath);
            return new Rectangle(0, 0, img.Width, img.Height);
        }
        catch
        {
            return Rectangle.Empty;
        }
    }

    public Rectangle GetBoundingRectAfterRotation(Rectangle rect, double angleDegrees)
    {
        double rad = angleDegrees * Math.PI / 180.0;
        double cos = Math.Abs(Math.Cos(rad));
        double sin = Math.Abs(Math.Sin(rad));
        int w = (int)(cos * rect.Width + sin * rect.Height);
        int h = (int)(cos * rect.Height + sin * rect.Width);
        return new Rectangle(rect.X, rect.Y, w, h);
    }

    private static Bitmap CreateRotatedBitmap(Image source, double angleDegrees)
    {
        double rad = angleDegrees * Math.PI / 180.0;
        double cos = Math.Abs(Math.Cos(rad));
        double sin = Math.Abs(Math.Sin(rad));

        int newW = (int)(cos * source.Width + sin * source.Height);
        int newH = (int)(cos * source.Height + sin * source.Width);

        var result = new Bitmap(newW, newH, PixelFormat.Format32bppArgb);
        result.SetResolution(source.HorizontalResolution, source.VerticalResolution);

        using var g = Graphics.FromImage(result);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        g.TranslateTransform(newW / 2f, newH / 2f);
        g.RotateTransform((float)angleDegrees);
        g.TranslateTransform(-source.Width / 2f, -source.Height / 2f);
        g.DrawImage(source, Point.Empty);

        return result;
    }

    private static Rectangle CalculateAdjustedCropRect(Image source, Rectangle cropRect, Rectangle mediaRect)
    {
        if (cropRect.Width == 0 || cropRect.Height == 0)
            return new Rectangle(0, 0, source.Width, source.Height);

        double scaleX = (double)source.Width / Math.Max(mediaRect.Width, 1);
        double scaleY = (double)source.Height / Math.Max(mediaRect.Height, 1);

        int x = Math.Max(0, Math.Min((int)(cropRect.X * scaleX), source.Width));
        int y = Math.Max(0, Math.Min((int)((mediaRect.Height - cropRect.Height - cropRect.Y) * scaleY), source.Height));
        int w = Math.Min((int)(cropRect.Width * scaleX), source.Width - x);
        int h = Math.Min((int)(cropRect.Height * scaleY), source.Height - y);

        return new Rectangle(x, y, w, h);
    }

    private static Bitmap CreateCroppedBitmap(Image source, Rectangle cropRect)
    {
        var result = new Bitmap(cropRect.Width, cropRect.Height, PixelFormat.Format24bppRgb);

        using var g = Graphics.FromImage(result);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.DrawImage(source, new Rectangle(0, 0, cropRect.Width, cropRect.Height), cropRect, GraphicsUnit.Pixel);

        return result;
    }
}
