﻿namespace AdvancedImage.Fields.Editor
{
    public class AdvancedImageEditorUpdateModel
    {
        public AdvancedImageEditorThumbnailsModel Thumbnails { get; set; }

        public AdvancedImageEditorDetailsModel Details { get; set; }
        public AdvancedImageEditorUpdateModel()
        {
            Details = new AdvancedImageEditorDetailsModel();
            Thumbnails = new AdvancedImageEditorThumbnailsModel();
        }
    }
}