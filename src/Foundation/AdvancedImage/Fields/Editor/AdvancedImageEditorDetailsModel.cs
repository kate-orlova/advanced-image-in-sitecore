using Sitecore.Data.Items;
using Sitecore.Shell.Applications.ContentEditor;

namespace AdvancedImage.Fields.Editor
{
    public class AdvancedImageEditorDetailsModel
    {
        public XmlValue XmlValue { get; set; }

        public MediaItem MediaItem { get; set; }

        public Item InnerItem => this.MediaItem?.InnerItem;

        public string ImageWidth => this.XmlValue?.GetAttribute("width");

        public string ImageHeight => this.XmlValue?.GetAttribute("height");

        public string OriginalDimensions => this.InnerItem?["Dimensions"];

        public string OriginalAlternateText => this.InnerItem?["Alt"];

        public string UpdatedAlternateText => this.XmlValue?.GetAttribute("alt");
    }
}