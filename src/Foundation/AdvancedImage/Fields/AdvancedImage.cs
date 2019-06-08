using Sitecore.Diagnostics;
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
        protected XmlValue XmlValue
        {
            get
            {
                XmlValue viewStateProperty = this.GetViewStateProperty("XmlValue", null) as XmlValue;
                if (viewStateProperty == null)
                {
                    viewStateProperty = new XmlValue(string.Empty, "image");
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
        protected string ThumbnailsFolderID { get; private set; }
        protected string ImageSourceFolderID { get; private set; }
        protected string IsDebug { get; private set; }

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