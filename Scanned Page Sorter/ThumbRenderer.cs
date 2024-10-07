using Scanned_Page_Sorter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Manina.Windows.Forms.ImageListViewRenderers
{
    public class ThumbnailRenderer : ImageListView.ImageListViewRenderer
    {
        int padding = 6;
        internal ImageMetadataMap imageMetadataMap;

        internal ThumbnailRenderer(ImageMetadataMap imageMetadataMap) => this.imageMetadataMap = imageMetadataMap;

        public override void DrawItem(Graphics g, ImageListViewItem item, ItemState state, Rectangle bounds)
        {
            {
                Size itemPadding = new Size(4, 4);
                bool alternate = (item.Index % 2 == 1);

                // Paint background
                if (ImageListView.Enabled)
                {
                    using (Brush bItemBack = new SolidBrush(alternate && ImageListView.View == View.Details ?
                        ImageListView.Colors.AlternateBackColor : ImageListView.Colors.BackColor))
                    {
                        g.FillRectangle(bItemBack, bounds);
                    }
                }
                else
                {
                    using (Brush bItemBack = new SolidBrush(ImageListView.Colors.DisabledBackColor))
                    {
                        g.FillRectangle(bItemBack, bounds);
                    }
                }

                // Paint background Disabled
                if ((state & ItemState.Disabled) != ItemState.None)
                {
                    using (Brush bDisabled = new LinearGradientBrush(bounds, ImageListView.Colors.DisabledColor1, ImageListView.Colors.DisabledColor2, LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bDisabled, bounds, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                // Paint background Selected
                else if ((ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None)) ||
                    (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None) && ((state & ItemState.Hovered) != ItemState.None)))
                {
                    using (Brush bSelected = new LinearGradientBrush(bounds, ImageListView.Colors.SelectedColor1, ImageListView.Colors.SelectedColor2, LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bSelected, bounds, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                // Paint background unfocused
                else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Brush bGray64 = new LinearGradientBrush(bounds, ImageListView.Colors.UnFocusedColor1, ImageListView.Colors.UnFocusedColor2, LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bGray64, bounds, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                // Paint background Hovered
                if ((state & ItemState.Hovered) != ItemState.None)
                {
                    using (Brush bHovered = new LinearGradientBrush(bounds, ImageListView.Colors.HoverColor1, ImageListView.Colors.HoverColor2, LinearGradientMode.Vertical))
                    {
                        Utility.FillRoundedRectangle(g, bHovered, bounds, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                if (ImageListView.View != View.Details)
                {
                    // Draw the image
                    Image img = item.GetCachedImage(CachedImageType.Thumbnail);
                    if (img != null)
                    {
                        // orientation angle
                        ImageMetadata metadata= imageMetadataMap[item.Text];

                        //float a = metadata.Rotate;
                        //// rotate image by angle a if a!=0
                        //Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(bounds.Location + itemPadding, ImageListView.ThumbnailSize));
                        //if ((a ) != 0) img = RotateImage(img, a );                        
                        //pos = getRotatedRectangle(pos, metadata.Orientation);
                        if ((metadata.Rotate + metadata.Orientation)!=0) img = RotateImage(img, metadata.Orientation, metadata.Rotate);
                        Rectangle pos = Utility.GetSizedImageBounds(img, new Rectangle(bounds.Location + itemPadding, ImageListView.ThumbnailSize));                        
                        g.DrawImage(img, pos);
                        // Draw image border
                        if (Math.Min(pos.Width, pos.Height) > 32)
                        {
                            using (Pen pOuterBorder = new Pen(ImageListView.Colors.ImageOuterBorderColor))
                            {
                                g.DrawRectangle(pOuterBorder, pos);
                            }
                            if (System.Math.Min(ImageListView.ThumbnailSize.Width, ImageListView.ThumbnailSize.Height) > 32)
                            {
                                using (Pen pInnerBorder = new Pen(ImageListView.Colors.ImageInnerBorderColor))
                                {
                                    g.DrawRectangle(pInnerBorder, Rectangle.Inflate(pos, -1, -1));
                                }
                            }
                        }
                    }

                    // Draw item text
                    Color foreColor = ImageListView.Colors.ForeColor;
                    if ((state & ItemState.Disabled) != ItemState.None)
                    {
                        foreColor = ImageListView.Colors.DisabledForeColor;
                    }
                    else if ((state & ItemState.Selected) != ItemState.None)
                    {
                        if (ImageListView.Focused)
                            foreColor = ImageListView.Colors.SelectedForeColor;
                        else
                            foreColor = ImageListView.Colors.UnFocusedForeColor;
                    }
                    Size szt = TextRenderer.MeasureText(item.Text, ImageListView.Font);
                    Rectangle rt = new Rectangle(bounds.Left + itemPadding.Width, bounds.Top + 2 * itemPadding.Height + ImageListView.ThumbnailSize.Height, ImageListView.ThumbnailSize.Width, szt.Height);
                    TextRenderer.DrawText(g, item.Text, ImageListView.Font, rt, foreColor,
                        TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix);
                }
                else base.DrawItem(g, item, state, bounds);

                // Item border
                if (ImageListView.View != View.Details)
                {
                    using (Pen pWhite128 = new Pen(Color.FromArgb(128, ImageListView.Colors.ControlBackColor)))
                    {
                        Utility.DrawRoundedRectangle(g, pWhite128, bounds.Left + 1, bounds.Top + 1, bounds.Width - 3, bounds.Height - 3, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }
                if (((state & ItemState.Disabled) != ItemState.None))
                {
                    using (Pen pHighlight128 = new Pen(ImageListView.Colors.DisabledBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }
                else if (ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen pHighlight128 = new Pen(ImageListView.Colors.SelectedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, pHighlight128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }
                else if (!ImageListView.Focused && ((state & ItemState.Selected) != ItemState.None))
                {
                    using (Pen pGray128 = new Pen(ImageListView.Colors.UnFocusedBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, pGray128, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }
                else if (ImageListView.View != View.Details && (state & ItemState.Selected) == ItemState.None)
                {
                    using (Pen pGray64 = new Pen(ImageListView.Colors.BorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, pGray64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                if (ImageListView.Focused && ((state & ItemState.Hovered) != ItemState.None))
                {
                    using (Pen pHighlight64 = new Pen(ImageListView.Colors.HoverBorderColor))
                    {
                        Utility.DrawRoundedRectangle(g, pHighlight64, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1, (ImageListView.View == View.Details ? 2 : 4));
                    }
                }

                // Focus rectangle
                if (ImageListView.Focused && ((state & ItemState.Focused) != ItemState.None))
                {
                    ControlPaint.DrawFocusRectangle(g, bounds);
                }
            }
        }

        private Image RotateImage(Image img, int orientation, float rotation)
        {
    
                Bitmap src = img as Bitmap;
                 Rectangle   cropRect = new Rectangle(0, 0, img.Width, img.Height);

                // Create a new bitmap for the bmp image

                Bitmap bmp = new Bitmap(cropRect.Width, cropRect.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(src, cropRect, cropRect, GraphicsUnit.Pixel);
                }
                if (orientation +rotation == 0) return bmp;

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
                    g.RotateTransform((float)(orientation+ rotation));
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

