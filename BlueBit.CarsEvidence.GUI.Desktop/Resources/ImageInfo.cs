using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueBit.CarsEvidence.GUI.Desktop.Resources
{
    public static class ImageInfo
    {
        private static readonly Size _imageSizeSmall = new Size(16, 16);
        private static readonly Size _imageSizeLarge = new Size(26, 26);

        public static BitmapSource AsResized(this BitmapSource @this, double width, double height)
        {
            Contract.Assert(@this != null);
            return new TransformedBitmap(@this, new ScaleTransform(width / @this.Width, height / @this.Height));
        }
        public static BitmapSource AsResized_16x16(this BitmapSource @this)
        {
            return @this.AsResized(16, 16);
        }
        public static Image AsImage_16x16(this ImageSource @this)
        {
            return new Image() { Source = @this, Width = 16, Height = 16 };
        }
    }
}
