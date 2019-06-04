using System.Collections.Generic;
using Sitecore.Data.Items;

namespace AdvancedImage.Fields.Editor
{
    public class AdvancedImageEditorThumbnailsModel
    {
        public string ControlId { get; set; }
        public string ImageSrc { get; set; }
        public IEnumerable<Item> Thumbnails { get; set; }
    }
}