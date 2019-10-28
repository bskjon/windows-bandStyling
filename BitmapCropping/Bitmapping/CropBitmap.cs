using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bitmapping
{
    public class CropBitmap
    {
        public static async Task<ImageSource> GetCroppedBitmap(StorageFile originalImageFile, Point startPoint, Size corpSize, double scale)
        {
            if (double.IsNaN(scale) || double.IsInfinity(scale))
            {
                scale = 1;
            }

            // Convert start point and size to integer.
            uint startPointX = (uint)Math.Floor(startPoint.X * scale);
            uint startPointY = (uint)Math.Floor(startPoint.Y * scale);
            uint height = (uint)Math.Floor(corpSize.Height * scale);
            uint width = (uint)Math.Floor(corpSize.Width * scale);

            using (IRandomAccessStream stream = await originalImageFile.OpenReadAsync())
            {

                // Create a decoder from the stream. With the decoder, we can get 
                // the properties of the image.
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                // The scaledSize of original image.
                uint scaledWidth = (uint)Math.Floor(decoder.PixelWidth * scale);
                uint scaledHeight = (uint)Math.Floor(decoder.PixelHeight * scale);


                // Refine the start point and the size. 
                if (startPointX + width > scaledWidth)
                {
                    startPointX = scaledWidth - width;
                }

                if (startPointY + height > scaledHeight)
                {
                    startPointY = scaledHeight - height;
                }

                // Get the cropped pixels.
                byte[] pixels = await GetPixelData(decoder, startPointX, startPointY, width, height,
                    scaledWidth, scaledHeight);

                // Stream the bytes into a WriteableBitmap
                WriteableBitmap cropBmp = new WriteableBitmap((int)width, (int)height);
                Stream pixStream = cropBmp.PixelBuffer.AsStream();
                pixStream.Write(pixels, 0, (int)(width * height * 4));

                return cropBmp;
            }
        }

        private static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY, uint width, uint height)
        {
            return await GetPixelData(decoder, startPointX, startPointY, width, height, decoder.PixelWidth, decoder.PixelHeight);
        }

        public static async Task<byte[]> GetPixelData(BitmapDecoder decoder, uint startPointX, uint startPointY, uint width, uint height, uint scaleWidth, uint scaledHeight)
        {
            BitmapTransform transform = new BitmapTransform();
            BitmapBounds bounds = new BitmapBounds();
            bounds.X = startPointX;
            bounds.Y = startPointY;
            bounds.Height = height;
            bounds.Width = width;
            transform.Bounds = bounds;

            transform.ScaledWidth = scaleWidth;
            transform.ScaledHeight = scaledHeight;

            PixelDataProvider pix = await decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.ColorManageToSRgb);
            byte[] pixels = pix.DetachPixelData();
            return pixels;
        }


        public static async Task SaveCroppedBitmap(StorageFile origintalImageFile, StorageFile croppedImageFile, Point startPoint, Size cropSize)
        {
            uint startPointX = (uint)Math.Floor(startPoint.X);
            uint startPointY = (uint)Math.Floor(startPoint.Y);
            uint height = (uint)Math.Floor(cropSize.Height);
            uint width = (uint)Math.Floor(cropSize.Width);

            using (IRandomAccessStream originalImageFileStream = await origintalImageFile.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(originalImageFileStream);

                if (startPointX + width > decoder.PixelWidth)
                {
                    startPointX = decoder.PixelWidth - width;
                }
                if (startPointY + height > decoder.PixelHeight)
                {
                    startPointY = decoder.PixelHeight - height;
                }

                byte[] pixels = await GetPixelData(decoder, startPointX, startPointY, width, height, decoder.PixelWidth, decoder.PixelHeight);

                using (IRandomAccessStream newImageFileStream = await croppedImageFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Guid encoderID = Guid.Empty;

                    switch (croppedImageFile.FileType.ToLower())
                    {
                        case ".png":
                            encoderID = BitmapEncoder.PngEncoderId;
                            break;

                        case ".bmp":
                            encoderID = BitmapEncoder.BmpEncoderId;
                            break;

                        default:
                            encoderID = BitmapEncoder.JpegEncoderId;
                            break;
                    }

                    BitmapEncoder bmpEncoder = await BitmapEncoder.CreateAsync( encoderID, newImageFileStream);

                    bmpEncoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, width, height, decoder.DpiX, decoder.DpiY, pixels);
                    await bmpEncoder.FlushAsync();


                }


            }

        }


    }
}
