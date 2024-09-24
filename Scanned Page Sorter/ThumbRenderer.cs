using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using Manina.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using static Manina.Windows.Forms.ImageListView;

namespace Manina.Windows.Forms.ImageListViewRenderers
{
    public class ThumbnailRenderer : ImageListView.ImageListViewRenderer
    {
        int padding = 6;

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
                        // rotation angle
                        int a = (item.Tag == null) ? 0 : Convert.ToInt32(item.Tag);
                        // rotate image by angle a if a!=0
                        if (a != 0)
                        {
                            img = RotateImage(img, a);
                        }
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

        private Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                g.RotateTransform(rotationAngle);
                g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                g.DrawImage(img, new Point(0, 0));
            }
            return bmp;
        }
    }
        }

