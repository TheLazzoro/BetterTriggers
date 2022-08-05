using BetterTriggers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI.Components.Shared
{
    public class TreeItemHeader : StackPanel
    {
        internal TextBox RenameBox;
        private Rectangle Icon;
        private TextBlock DisplayText;
        private string categoryName = string.Empty;

        public enum TreeItemState
        {
            Normal,
            Disabled,
            HasErrors,
            HasErrorsNoTextColor
        }


        public TreeItemHeader(string text, string categoryName, TreeItemState state = TreeItemState.Normal, bool isInitiallyOn = true)
        {
            this.Orientation = Orientation.Horizontal;
            this.Height = 18;
            this.Margin = new Thickness(-1, -1, 0, -1);

            Icon = new Rectangle();
            Icon.Width = 16;
            Icon.Height = 16;

            DisplayText = new TextBlock();
            DisplayText.Margin = new Thickness(5, 0, 0, 0);
            DisplayText.FontFamily = new FontFamily("Segoe UI");

            RenameBox = new TextBox();
            RenameBox.Margin = new Thickness(5, 0, 0, 0);

            Category category = Category.Get(categoryName);

            if (category.ShouldDisplay)
            {
                ControllerTriggerData controllerTriggerData = new ControllerTriggerData();
                this.categoryName = Locale.Translate(Category.Get(categoryName).Name) + " - ";
            }

            SetIcon(categoryName, state);
            SetDisplayText(text);
            SetTextEnabled(state, isInitiallyOn);

            this.Children.Add(Icon);
            this.Children.Add(DisplayText);

            this.RenameBox.LostFocus += RenameBox_LostFocus;
        }


        public void SetDisplayText(string text)
        {
            DisplayText.Text = categoryName + text;
            RenameBox.Text = text;
        }

        public void ShowRenameBox(bool doShow)
        {
            if (doShow)
            {
                this.Children.Remove(DisplayText);
                this.Children.Add(RenameBox);
                RenameBox.Focus();
                RenameBox.SelectAll();
            }
            else
            {
                this.Children.Remove(RenameBox);
                this.Children.Add(DisplayText);
            }
        }

        public void SetTextEnabled(TreeItemState state, bool isEnabled)
        {
            if (state == TreeItemState.HasErrors)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#C66");
            else if (isEnabled && state == TreeItemState.Normal)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            else if (isEnabled && state != TreeItemState.Normal)
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#CCC");
            else
                DisplayText.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#888");
        }

        public string GetRenameText()
        {
            return RenameBox.Text;
        }


        private void RenameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RenameBox.Parent != null)
            {
                ShowRenameBox(false);
                SetDisplayText(DisplayText.Text);
            }
        }

        public void SetIcon(string category, TreeItemState state)
        {
            var group = new DrawingGroup();
            group.Children.Add(new ImageDrawing(DrawIconImage(Category.Get(category).Icon), new Rect(0, 0, 16, 16)));
            if (state == TreeItemState.Disabled || state == TreeItemState.HasErrors)
                group.Children.Add(new ImageDrawing(DrawIconImage(Category.Get("TC_ERROR").Icon), new Rect(0, 0, 16, 16)));
            else if (state == TreeItemState.HasErrorsNoTextColor)
                group.Children.Add(new ImageDrawing(DrawIconImage(Category.Get("TC_INVALID").Icon), new Rect(0, 0, 16, 16)));

            var drawingImage = new DrawingImage(group);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(drawingImage, new Rect(new Point(0, 0), new Size(drawingImage.Width, drawingImage.Height)));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)drawingImage.Width, (int)drawingImage.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            ImageBrush brush = new ImageBrush(bmp);
            Icon.Fill = brush;
        }

        private static BitmapImage OverlapImage(BitmapImage source, BitmapImage toOverlap)
        {
            //Assume we have two bitmaps
            WriteableBitmap bitmap1, bitmap2;
            bitmap1 = new WriteableBitmap(source);
            bitmap2 = new WriteableBitmap(toOverlap);

            //Get the pixel arrays of both bitmaps
            int width = bitmap1.PixelWidth;
            int height = bitmap1.PixelHeight;
            int stride = bitmap1.BackBufferStride;

            int[] pixels1 = new int[width * height];
            int[] pixels2 = new int[pixels1.Length];

            bitmap1.CopyPixels(pixels1, stride, 0);
            bitmap2.CopyPixels(pixels2, stride, 0);

            //Detect the rectangle that has difference
            int top = 0, bottom = 0, left = 0, right = 0;
            for (int i = 0; i < pixels1.Length; i++)
            {
                if (pixels1[i] != pixels2[i])
                {
                    int row = i / width;
                    int col = i % width;
                    top = Math.Min(top, row);
                    bottom = Math.Max(bottom, row);
                    left = Math.Min(left, col);
                    right = Math.Max(right, col);
                }
            }
            Int32Rect rect = new Int32Rect(left, top, right - left + 1, bottom - top + 1);

            //Copy pixels of the different rectangle in the second bitmap into another array
            int[] pixelDiff = new int[rect.Width * rect.Height];
            for (int i = 0; i < rect.Width; i++)
            {
                for (int j = 0; j < rect.Height; j++)
                {
                    var pixelOfMerger = pixels2[i + j * rect.Width];
                    if (pixelOfMerger > 60000 || pixelOfMerger < -60000) // here is my addition to this atrocious image merger function
                        pixelDiff[i + j * rect.Width] = pixels2[rect.X + i + (rect.Y + j) * width];
                    else
                        pixelDiff[i + j * rect.Width] = pixels1[rect.X + i + (rect.Y + j) * width];
                }
            }

            //Write the new pixels into the first bitmap
            bitmap1.WritePixels(rect, pixelDiff, stride * rect.Width / width, 0);

            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap1));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }

            return bmImage;
        }

        private static BitmapImage DrawIconImage(System.Drawing.Bitmap image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
