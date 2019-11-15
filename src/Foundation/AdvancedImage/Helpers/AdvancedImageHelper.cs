using AdvancedImage.GlassMapper.Fields;
using Sitecore.Data;
using Sitecore.Resources.Media;
using System;
using System.Globalization;
using System.Xml;

namespace AdvancedImage.Helpers
{
    internal static class AdvancedImageHelper
    {
        internal static AdvancedImageField ConvertMediaItemToField(XmlElement elementNode, Database database)
        {
            var id = elementNode.GetAttribute("mediaid");
            var mediaItem = database.GetItem(new ID(id));
            if (mediaItem == null) return null;

            var cropX = elementNode.HasAttribute("cropx") ? elementNode.GetAttribute("cropx") : string.Empty;
            var cropY = elementNode.HasAttribute("cropy") ? elementNode.GetAttribute("cropy") : string.Empty;
            var focusX = elementNode.HasAttribute("focusx") ? elementNode.GetAttribute("focusx") : string.Empty;
            var focusY = elementNode.HasAttribute("focusy") ? elementNode.GetAttribute("focusy") : string.Empty;
            var showFull = elementNode.HasAttribute("showFull") ? elementNode.GetAttribute("showFull") : "false";

            float.TryParse(cropX, NumberStyles.Any, CultureInfo.InvariantCulture, out var cx);
            float.TryParse(cropY, NumberStyles.Any, CultureInfo.InvariantCulture, out var cy);
            float.TryParse(focusX, NumberStyles.Any, CultureInfo.InvariantCulture, out var fx);
            float.TryParse(focusY, NumberStyles.Any, CultureInfo.InvariantCulture, out var fy);
            bool.TryParse(showFull, out var sf);

            var resultImage = new AdvancedImageField();
            resultImage.ShowFull = sf;
            resultImage.CropX = cx;
            resultImage.CropY = cy;
            resultImage.FocusX = fx;
            resultImage.FocusY = fy;
            resultImage.Alt = mediaItem["Alt"];
            resultImage.Border = mediaItem["Border"];
            resultImage.Class = mediaItem["Class"];
            resultImage.Width = Convert.ToInt32(string.IsNullOrEmpty(mediaItem["Width"]) ? "0" : mediaItem["Width"]);
            resultImage.Height = Convert.ToInt32(string.IsNullOrEmpty(mediaItem["Height"]) ? "0" : mediaItem["Height"]);
            resultImage.HSpace = Convert.ToInt32(string.IsNullOrEmpty(mediaItem["HSpace"]) ? "0" : mediaItem["HSpace"]);
            resultImage.Language = mediaItem.Language;
            resultImage.MediaId = mediaItem.ID.ToGuid();
            resultImage.MediaExists = true;
            resultImage.Src = MediaManager.GetMediaUrl(mediaItem);
            resultImage.VSpace = Convert.ToInt32(string.IsNullOrEmpty(mediaItem["VSpace"]) ? "0" : mediaItem["VSpace"]);

            return resultImage;
        }
    }
}