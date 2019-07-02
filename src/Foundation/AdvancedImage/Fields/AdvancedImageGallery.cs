using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
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
        public string LastSelectedItemId
        {
            get => StringUtil.GetString(ViewState["LastSelectedItemID"]);
            set => ViewState["LastSelectedItemID"] = value;
        }

        public override string GetValue()
        {
            return XmlValue.ToString();
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            XmlValue = new XmlValue(value, "gallery");
            Value = GetMediaPath();
        }

        protected void SetValue(MediaItem item)
        {
            Assert.ArgumentNotNull(item, "item");
            XmlValue.SetAttribute("mediaid", item.ID.ToString());
            Value = GetMediaPath();
        }

        private string GetMediaPath()
        {
            return string.Empty;
        }
    }
}