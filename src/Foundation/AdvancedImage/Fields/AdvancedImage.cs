using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
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
        public AdvancedImage()
        {
            Visible = false;
            Class = "scContentControlImage";
            Change = "#";
            Activation = true;
        }
        protected void Browse()
        {
            if (Disabled)
            {
                return;
            }
            Sitecore.Context.ClientPage.Start(this, "BrowseImage");
        }
        public override string GetValue()
        {
            return XmlValue.ToString();
        }

        public override void SetValue(string value)
        {
            Assert.ArgumentNotNull(value, "value");
            XmlValue = new XmlValue(value, "image");
            Value = this.GetMediaPath();
        }

        private Item GetMediaItem()
        {
            string attribute = this.XmlValue.GetAttribute("mediaid");
            if (attribute.Length <= 0)
            {
                return null;
            }
            Language language = Language.Parse(this.ItemLanguage);
            return Client.ContentDatabase.GetItem(attribute, language);
        }

        private string GetMediaPath()
        {
            MediaItem mediaItem = this.GetMediaItem();
            if (mediaItem == null)
            {
                return string.Empty;
            }
            return mediaItem.MediaPath;
        }
    }
}