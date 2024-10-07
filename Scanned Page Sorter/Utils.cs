using System.Windows.Forms;

namespace Scanned_Page_Sorter
{
    using System;
    using System.Drawing;
    using System.Threading;

    public class Debouncer : IDisposable
    {
        private Thread thread;
        private volatile Action action;
        private volatile int delay = 0;

        public void Debounce(Action action, int delay = 1250)
        {
            this.action = action;
            this.delay = delay;

            if (this.thread == null)
            {
                this.thread = new Thread(() => this.RunThread());
                this.thread.IsBackground = true;
                this.thread.Start();
            }
        }

        private void RunThread()
        {
            while (true)
            {
                int d = this.delay;
                this.delay = 0;
                Thread.Sleep(d);
                if (this.delay == 0 && this.action != null)
                {
                    this.action();
                    this.action = null;
                }
            }
        }

        public void Dispose()
        {
            if (this.thread != null)
            {
                this.thread.Abort();
                this.thread = null;
            }
        }
    }

    public static class ImageUtils
    {
        public static Image RotateImage(Image img, int orientation, float rotation)
        {
            Bitmap src = img as Bitmap;
            Rectangle cropRect = new Rectangle(0, 0, img.Width, img.Height);

            // Create a new bitmap for the bmp image
            Bitmap bmp = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(src, cropRect, cropRect, GraphicsUnit.Pixel);
            }
            if (orientation + rotation == 0) return bmp;

            double radianAngle = orientation / 180.0 * Math.PI;
            double cosA = Math.Abs(Math.Cos(radianAngle));
            double sinA = Math.Abs(Math.Sin(radianAngle));

            int newWidth = (int)(cosA * bmp.Width + sinA * bmp.Height);
            int newHeight = (int)(cosA * bmp.Height + sinA * bmp.Width);

            var rotatedBitmap = new Bitmap(newWidth, newHeight);
            rotatedBitmap.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedBitmap))
            {
                g.TranslateTransform(rotatedBitmap.Width / 2, rotatedBitmap.Height / 2);
                g.RotateTransform((float)(orientation + rotation));
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                g.DrawImage(bmp, new Point(0, 0));
            }

            bmp.Dispose(); // Remove if you want to keep original bitmap

            return rotatedBitmap;
        }
    }
}
