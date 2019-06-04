namespace AdvancedImage.Fields.Editor
{
    public class AdvancedImageEditorModel
    {
        public string ControlId { get; set; }

        public AdvancedImageEditorThumbnailsModel Thumbnails { get; set; }

        public string IsDebug { get; set; }

        public AdvancedImageEditorItem Image { get; set; }

        public AdvancedImageEditorDetailsModel Details { get; set; }
    }
}