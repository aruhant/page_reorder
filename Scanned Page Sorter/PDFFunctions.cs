﻿using iText.Layout.Font;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    public partial class pageSorterForm : Form
    {
        private Image CropToBoundsAndRotate(Image img, Rectangle cropRect, Rectangle mediaRect, double rotation)
        {
            Bitmap src = img as Bitmap;
            Console.WriteLine($"\n\n**************************\nRotation {rotation}\nCrop:{cropRect}\nmedia:{mediaRect}");
            double scalex = (double)src.Width / mediaRect.Width;
            double scaley = (double)src.Height / mediaRect.Height;
            Console.WriteLine($"Scalex {scalex} Scaley {scaley}");
            if (cropRect.Height == 0 || cropRect.Width == 0) cropRect = new Rectangle(0, 0, img.Width, img.Height);
            else cropRect = new Rectangle((int)((cropRect.X) * scalex), (int)((mediaRect.Height- cropRect.Height - cropRect.Y) * scaley),
                (int)(cropRect.Width * scalex), (int)(cropRect.Height * scaley));

            // Create a new bitmap for the bmp image
            
            Bitmap bmp = new Bitmap(cropRect.Width, cropRect.Height);
            Console.WriteLine($"src:{src.Width},{src.Height} \nTargetrect: {cropRect}");
            // Rotate and crop the image
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(src, new Rectangle(0, 0, cropRect.Width, cropRect.Height), new Rectangle(cropRect.X, cropRect.Y, cropRect.Width, cropRect.Height), GraphicsUnit.Pixel);
            }
            if (rotation == 0) return bmp;

            double radianAngle = rotation / 180.0 * Math.PI;
            double cosA = Math.Abs(Math.Cos(radianAngle));
            double sinA = Math.Abs(Math.Sin(radianAngle));

            int newWidth = (int)(cosA * bmp.Width + sinA * bmp.Height);
            int newHeight = (int)(cosA * bmp.Height + sinA * bmp.Width);

            var rotatedBitmap = new Bitmap(newWidth, newHeight);
            rotatedBitmap.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedBitmap))
            {
                g.TranslateTransform(rotatedBitmap.Width / 2, rotatedBitmap.Height / 2);
                g.RotateTransform((float)rotation);
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, new Point(0, 0));
            }

            bmp.Dispose();//Remove if you want to keep oryginal bitmap

            return rotatedBitmap;

        }
        private Rectangle getRotatedRectangle(Rectangle rect, double angle)
        {
            // Calculate the rotated rectangle's bounds
            PointF[] points = new PointF[4];
            points[0] = new PointF(rect.Left, rect.Top);
            points[1] = new PointF(rect.Right, rect.Top);
            points[2] = new PointF(rect.Right, rect.Bottom);
            points[3] = new PointF(rect.Left, rect.Bottom);

            using (Matrix matrix = new Matrix())
            {
                matrix.RotateAt((float)angle, new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2));
                matrix.TransformPoints(points);
            }

            float minX = points.Min(p => p.X);
            float maxX = points.Max(p => p.X);
            float minY = points.Min(p => p.Y);
            float maxY = points.Max(p => p.Y);

            return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));
        }
    }
}