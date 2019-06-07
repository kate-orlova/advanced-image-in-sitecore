﻿using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;

namespace AdvancedImage.Fields
{
    public class AdvancedImage : LinkBase
    {
        private const string THUMBNAIL_FOLDER_FIELD_NAME = "ThumbnailsFolderID";
        private const string IMAGES_SOURCE_FOLDER_FIELD_NAME = "ImagesSourceFolderID";
        private const string IS_DEBUG_FIELD_NAME = "IsDebug";
        public string ItemVersion
        {
            get
            {
                return GetViewStateString("Version");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Version", value);
            }
        }
        public override string GetValue()
        {
            throw new System.NotImplementedException();
        }

        public override void SetValue(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}