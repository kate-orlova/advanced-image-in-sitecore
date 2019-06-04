using System.Collections.Generic;

namespace AdvancedImage.Fields.Editor
{
    public class AdvancedImageGalleryEditorModel
    {
        public string ControlId { get; set; }

        public AdvancedImageEditorThumbnailsModel Thumbnails { get; set; }

        public string IsDebug { get; set; }

        public IEnumerable<AdvancedImageEditorItem> Images { get; set; }

        public AdvancedImageEditorDetailsModel Details { get; set; }
    }
}