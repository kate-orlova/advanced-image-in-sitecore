using Sitecore.Data.Items;
using System.IO;

namespace AdvancedImage.Processors
{
    public class CropProcessor
    {
        private static readonly string[] IMAGE_EXTENSIONS = { "bmp", "jpeg", "jpg", "png", "gif" };

        private Stream GetCroppedImage(string extension, int width, int height, float cx, float cy, MediaItem mediaItem)
        {
            var outputStrm = new MemoryStream();

            return outputStrm;
        }
    }
}