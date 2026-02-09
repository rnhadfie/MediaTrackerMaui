using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using IImage = Microsoft.Maui.Graphics.IImage;
using ImageFormat = Microsoft.Maui.Graphics.ImageFormat;

namespace MauiApp1.BackEnd.Service
{
    public static class SharedService
    {
        public static byte[] CompressImage(byte[] imageData, float maxSize, int quality)
        {
            var stream = new MemoryStream(imageData);

            IImage image = PlatformImage.FromStream(stream);

            if (image == null)
            {
                return null;
            }

            IImage newImage = image.Downsize(maxSize, true); 

            if (newImage == null)
            {
                return null;
            }
            MemoryStream compressedStream = new MemoryStream();
            newImage.AsStream(ImageFormat.Jpeg, quality).CopyTo(compressedStream);

            return compressedStream.ToArray();
        }
    }
}
