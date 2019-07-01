﻿using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;

namespace AdvancedImage.Fields
{
    public class AdvancedImageGallery : LinkBase
    {
        private const string THUMBNAIL_FOLDER_FIELD_NAME = "ThumbnailsFolderID";
        private const string IMAGES_SOURCE_FOLDER_FIELD_NAME = "ImagesSourceFolderID";
        private const string IS_DEBUG_FIELD_NAME = "IsDebug";
        private const string ASSETS_FOLDER_ID = "xxx-yyyy-zzzz";

        protected XmlValue XmlValue
        {
            get
            {
                XmlValue viewStateProperty = this.GetViewStateProperty("XmlValue", null) as XmlValue;
                if (viewStateProperty == null)
                {
                    viewStateProperty = new XmlValue(string.Empty, "gallery");
                    XmlValue = viewStateProperty;
                }

                return viewStateProperty;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateProperty("XmlValue", value, null);
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