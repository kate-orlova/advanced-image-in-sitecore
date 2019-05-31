using System.Drawing;
using Sitecore.Data.Items;
using System.IO;
using ImageProcessor.Imaging;

namespace AdvancedImage.Processors
{
    public class CropProcessor
    {
        private static readonly string[] IMAGE_EXTENSIONS = { "bmp", "jpeg", "jpg", "png", "gif" };

        private Stream GetCroppedImage(string extension, int width, int height, float cx, float cy, MediaItem mediaItem)
        {
            var outputStrm = new MemoryStream();
            var mediaStrm = mediaItem.GetMediaStream();
            var img = Image.FromStream(mediaStrm);
            var proc = new ImageProcessor.ImageFactory();
            proc.Load(img);

            var axis = new float[] { cy, cx };
            proc = proc.Resize(new ResizeLayer(new Size(width, height), ResizeMode.Crop, AnchorPosition.Center, true, centerCoordinates: axis));
            proc.Save(outputStrm);

            return outputStrm;
        }
    }
}